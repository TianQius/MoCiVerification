using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using MoCiVerification.Message;
using MoCiVerification.ViewModels;
using SukiUI.Controls;

namespace MoCiVerification.Views.Windows;

public partial class ChangeVersionView : SukiWindow
{
    public ChangeVersionView()
    {
        InitializeComponent();
        this.Loaded += OnViewLoaded;
    }
    private async void OnViewLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ChangeVersionViewModel vm)
        {
            vm.RequestClose += Close;
            WeakReferenceMessenger.Default.Send(new LoadVersionMessage());
        }
    }
}