using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using MoCiVerification.Services;
using SukiUI.Dialogs;
using SukiUI.Toasts;

namespace MoCiProxyClient.ViewModels;

public partial class MainMoCiViewModel : ViewModelBase
{
    [ObservableProperty]
    private object? _contentViewModel;
    public ISukiToastManager ToastManager { get; }
    public ISukiDialogManager DialogManager { get; }
    private readonly IServiceProvider _serviceProvider;
    private readonly LoginNavigationService _loginNavigationService;

    public MainMoCiViewModel(IServiceProvider serviceProvider,LoginNavigationService  navigationService,ISukiToastManager toastManager, ISukiDialogManager dialogManager)
    {
        _serviceProvider = serviceProvider;
        _loginNavigationService = navigationService;
        ToastManager = toastManager;
        DialogManager = dialogManager;
        ContentViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();
        _loginNavigationService.NavigationRequested += OnNavigationRequested;
    }
    private void OnNavigationRequested(Type pageType)
    {
        var pageInstance = _serviceProvider.GetService(pageType);
        if (pageInstance != null)
        {
            ContentViewModel = pageInstance;
            ToastManager.CreateSimpleInfoToast()
                .WithTitle("界面已切换")
                .WithContent($"界面切换成功")
                .Dismiss().After(TimeSpan.FromSeconds(1))
                .Queue();
        }
    }
}