using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MoCiVerification.Features;

namespace MoCiVerification.Views;

public partial class ProjectView : UserControl
{
    public ProjectView()
    {
        InitializeComponent();
        this.Loaded += OnViewLoaded;
    }
    private async void OnViewLoaded(object? sender, RoutedEventArgs e)
    {
        this.Loaded -= OnViewLoaded;
        if (DataContext is PageBase page)
        {
            await page.OnPageLoadedAsync();
        }
    }
    private async void OnSelectedChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is PageBase page)
        {
            await page.OnPageDataChangedAsync();
        }
    }
    
    
}