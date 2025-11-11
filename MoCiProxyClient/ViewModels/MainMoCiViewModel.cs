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
                .WithTitle("登录成功！")
                .WithContent($"欢迎使用陌辞验证代理端")
                .Dismiss().After(TimeSpan.FromSeconds(3))
                .Queue();
        }
    }
}