
using Avalonia.Interactivity;
using MoCiProxyClient.ViewModels;
using SukiUI.Controls;

namespace MoCiProxyClient.Views;

public partial class AddCardView : SukiWindow
{
    public AddCardView()
    {
        InitializeComponent();
        
        this.Loaded += OnViewLoaded;
    }
    private async void OnViewLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AddCardViewModel vm)
        {
            vm.RequestClose += Close;
        }
    }
}