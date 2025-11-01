using Avalonia.Controls;
using Avalonia.Interactivity;
using MoCiVerification.Features;

namespace MoCiVerification.Views;

public partial class BlackerView : UserControl
{
    public BlackerView()
    {
        InitializeComponent();
        this.Loaded += OnViewLoaded;
    }
    private async void OnViewLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is PageBase page)
        {
            await page.OnPageLoadedAsync();
        }
    }
}