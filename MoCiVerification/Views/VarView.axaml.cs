using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MoCiVerification.Features;

namespace MoCiVerification.Views;

public partial class VarView : UserControl
{
    public VarView()
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