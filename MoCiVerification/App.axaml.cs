using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System.Net;
using System.Net.Http;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using MoCiVerification.Behaviors;
using MoCiVerification.ViewModels;
using MoCiVerification.Views;
using SukiUI.Controls;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using MoCiVerification.Common;
using MoCiVerification.Message;
using MoCiVerification.Models;
using MoCiVerification.Services;
using MoCiVerification.Views.Windows;


namespace MoCiVerification;

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
            WeakReferenceMessenger.Default.Register<LoginSuccessMessage>(this, (recipient, message) =>
            {
                var old = desktop?.MainWindow;
                
                desktop!.MainWindow = views.CreateView<MainMoCiViewModel>(provider) as Window;
                
                desktop.MainWindow?.Show();
                old?.Close();
                WeakReferenceMessenger.Default.Unregister<LoginSuccessMessage>(this);
            });
            desktop.MainWindow = views.CreateView<MainLoginViewModel>(provider) as Window;
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
            .AddView<MainLoginView, MainLoginViewModel>(services)
            .AddView<FindView,FindViewModel>(services)
            .AddView<AddProjectView, AddProjectViewModel>(services)
            .AddView<AddCardView,AddCardViewModel>(services)
            .AddView<AddCustomDataView,AddCustomDataViewModel>(services)
            .AddView<AddVersionView,AddVersionViewModel>(services)
            .AddView<AddBlackerView, AddBlackerViewModel>(services)
            .AddView<AddAgentView, AddAgentViewModel>(services)
            .AddView<ActiveView, ActiveViewModel>(services)
            .AddView<ChangeProjectView, ChangeProjectViewModel>(services)
            .AddView<ChangeVersionView,ChangeVersionViewModel>(services)
            .AddView<ChangeCustomDataView, ChangeCustomDataViewModel>(services)
            
            // Add pages
            //.AddView<DashboardView, DashboardViewModel>(services)
            .AddView<LoginView, LoginViewModel>(services)
            .AddView<RegisterView, RegisterViewModel>(services)
            .AddView<ProjectView,ProjectViewModel>(services)
            .AddView<VersionView, VersionViewModel>(services)
            .AddView<CardView, CardViewModel>(services)
            .AddView<UserView,UserViewModel>(services)
            .AddView<DataView, DataViewModel>(services)
            .AddView<BlackerView, BlackerViewModel>(services)
            .AddView<VarView, VarViewModel>(services)
            .AddView<AgentView, AgentViewModel>(services)
            .AddView<SettingView,SettingViewModel>(services)
            .AddView<SplashView, SplashViewModel>(services);
        
    }

    private static ServiceProvider ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton<ISukiToastManager, SukiToastManager>();
        services.AddSingleton<ISukiDialogManager, SukiDialogManager>();
        services.AddSingleton<PageNavigationService>();
        services.AddSingleton<LoginNavigationService>();
        services.AddSingleton<IAdminService, AdminService>();
        services.AddSingleton<DataGridBehaviors>();
        services.AddSingleton<ClientSettings>(sp =>
        {
            var settings = new ClientSettings();
            settings.LoadFromJson();
            return settings;
        });
        services.AddScoped<IShowWindowManager, ShowWindowManager>();

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
        services.AddHttpClient("DirectClient", client =>
            {
                client.BaseAddress = new Uri("http://111.231.13.26:82/api.php");
                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestVersion = HttpVersion.Version11;
            })
            .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
            {
                MaxConnectionsPerServer = 100,
                AutomaticDecompression = System.Net.DecompressionMethods.All,
            });

        services.AddSingleton<MoCiRequestService>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var settings = sp.GetRequiredService<ClientSettings>();

            string clientName = settings.UseNodeServer ? "NodeClient" : "DirectClient";

            var httpClient = factory.CreateClient(clientName);
            return new MoCiRequestService(httpClient);
        });
        return services.BuildServiceProvider();
    }
}