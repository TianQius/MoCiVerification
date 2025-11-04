using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MoCiProxyClient.Features;

namespace MoCiProxyClient.Views;

public partial class ProxyView : UserControl
{
    public ProxyView()
    {
        InitializeComponent();
        this.Loaded += OnViewLoaded;
    }
    private async void OnViewLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is LoginPage page)
        {
            await page.OnPageLoadedAsync();
        }
    }
}