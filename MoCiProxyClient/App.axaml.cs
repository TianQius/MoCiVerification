using System;
using System.Data;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Net;
using System.Net.Http;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MoCiProxyClient.Behaviors;
using MoCiProxyClient.Common;
using MoCiProxyClient.Models;
using MoCiProxyClient.Services;
using MoCiProxyClient.ViewModels;
using MoCiProxyClient.Views;
using MoCiVerification.Services;
using SukiUI.Controls;
using SukiUI.Dialogs;
using SukiUI.Toasts;

namespace MoCiProxyClient;

public partial class App : Application
{
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var services = new ServiceCollection();
            services.AddSingleton(desktop);
            var views = ConfigureViews(services);
            var provider = ConfigureServices(services);
            DataTemplates.Add(new ViewLocator(views));
            desktop.MainWindow = views.CreateView<MainMoCiViewModel>(provider) as Window;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            var services = new ServiceCollection();
            services.AddSingleton(singleView);
            var views = ConfigureViews(services);
            var provider = ConfigureServices(services);
            DataTemplates.Add(new ViewLocator(views));

            // Ideally, we want to create a MainView that host app content
            // and use it for both IClassicDesktopStyleApplicationLifetime and ISingleViewApplicationLifetime
            singleView.MainView = new SukiMainHost()
            {
                Hosts = [
                    new SukiDialogHost
                    {
                        Manager = new SukiDialogManager()
                    }
                ],
              Content = views.CreateView<MainMoCiViewModel>(provider)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static MainViews ConfigureViews(ServiceCollection services)
    {
        return new MainViews()

            // Add main view
            .AddView<MainMoCiView, MainMoCiViewModel>(services)
            .AddView<LoginView, LoginViewModel>(services)
            .AddView<AddCardView, AddCardViewModel>(services)
            .AddView<ProxyView, ProxyViewModel>(services);
            



        // Add pages
        //.AddView<DashboardView, DashboardViewModel>(services)


    }

    private static ServiceProvider ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton<ISukiToastManager, SukiToastManager>();
        services.AddSingleton<ISukiDialogManager, SukiDialogManager>();
        services.AddSingleton<IProxyService, ProxyService>();
        services.AddSingleton<IShowWindowManager, ShowWindowManager>();
        services.AddSingleton<LoginNavigationService>();
        services.AddSingleton<DataGridBehaviors>();
        services.AddSingleton<ClientSettings>();
        services.AddHttpClient(); 

        services.AddHttpClient("NodeClient", client =>
            {
                client.BaseAddress = new Uri("https://ver-cnode.niansir.com/v2/api.php");
                client.Timeout = TimeSpan.FromSeconds(8);
                client.DefaultRequestVersion = HttpVersion.Version20;
            })
            .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
            {
                MaxConnectionsPerServer = 100,
                AutomaticDecompression = System.Net.DecompressionMethods.All,
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            });
        services.AddSingleton<MoCiRequestService>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = factory.CreateClient("NodeClient");
            return new MoCiRequestService(httpClient);
        });
        return services.BuildServiceProvider();
    }

}