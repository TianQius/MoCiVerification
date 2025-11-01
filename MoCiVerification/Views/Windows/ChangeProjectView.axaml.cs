using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using MoCiVerification.Message;
using MoCiVerification.ViewModels;
using SukiUI.Controls;

namespace MoCiVerification.Views.Windows;

public partial class ChangeProjectView : SukiWindow
{
    public ChangeProjectView()
    {
        InitializeComponent();
        this.Loaded += OnViewLoaded;
    }
    private async void OnViewLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ChangeProjectViewModel vm)
        {
            vm.RequestClose += Close;
            WeakReferenceMessenger.Default.Send(new LoadProjectAnnouncementMessage());
        }
    }
}