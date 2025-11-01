using System;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using MoCiVerification.Message;
using MoCiVerification.ViewModels;
using SukiUI.Controls;

namespace MoCiVerification.Views;

public partial class MainMoCiView : SukiWindow
{
    public MainMoCiView()
    {
        InitializeComponent();
        if (RuntimeFeature.IsDynamicCodeCompiled == false)
        {
            Title += " (native)";
        }
        Loaded += OnLoaded;
    }
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (Hosts?.Count > 0)
        {
            Dispatcher.UIThread.Post(() =>
            {
            }, DispatcherPriority.Background);
        }
    }
    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        IsMenuVisible = !IsMenuVisible;
    }
    
}