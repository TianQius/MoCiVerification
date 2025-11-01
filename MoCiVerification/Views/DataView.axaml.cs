using Avalonia.Controls;
using Avalonia.Interactivity;
using MoCiVerification.Features;

namespace MoCiVerification.Views;

public partial class DataView : UserControl
{
    public DataView()
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