
using Avalonia.Interactivity;
using MoCiVerification.ViewModels;
using SukiUI.Controls;

namespace MoCiVerification.Views.Windows;

public partial class FindView : SukiWindow
{
    public FindView()
    {
        InitializeComponent();
        
        this.Loaded += OnViewLoaded;
    }
    private async void OnViewLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is FindViewModel vm)
        {
            vm.RequestClose += Close;
        }
    }
}