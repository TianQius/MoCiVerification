using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MoCiVerification.ViewModels;
using SukiUI.Controls;

namespace MoCiVerification.Views.Windows;

public partial class AddBlackerView : SukiWindow
{
    public AddBlackerView()
    {
        InitializeComponent();
        this.Loaded += OnViewLoaded;
    }
    private async void OnViewLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AddBlackerViewModel vm)
        {
            vm.RequestClose += Close;
        }
    }
}