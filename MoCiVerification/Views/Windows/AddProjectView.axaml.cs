
using Avalonia.Interactivity;
using MoCiVerification.ViewModels;
using SukiUI.Controls;

namespace MoCiVerification.Views.Windows;

public partial class AddProjectView : SukiWindow
{
    public AddProjectView()
    {
        InitializeComponent();
        this.Loaded += OnViewLoaded;
    }
    private async void OnViewLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AddProjectViewModel vm)
        {
            vm.RequestClose += Close;
        }
    }
}