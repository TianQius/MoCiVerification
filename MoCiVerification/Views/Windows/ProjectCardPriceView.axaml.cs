
using Avalonia.Interactivity;
using MoCiVerification.ViewModels;
using SukiUI.Controls;

namespace MoCiVerification.Views.Windows;

public partial class ProjectCardPriceView : SukiWindow
{
    public ProjectCardPriceView()
    {
        InitializeComponent();
        this.Loaded += OnViewLoaded;
    }
    private async void OnViewLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ProjectCardPriceViewModel vm)
        {
            vm.RequestClose += Close;
            vm.GetOptions();
        }
    }
}