using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using MoCiVerification.Services;
using SukiUI.Dialogs;
using SukiUI.Toasts;

namespace MoCiVerification.ViewModels;

public partial class MainLoginViewModel : ViewModelBase
{
    [ObservableProperty]
    private object? _contentViewModel;
    public ISukiToastManager ToastManager { get; }
    public ISukiDialogManager DialogManager { get; }
    private readonly IServiceProvider _serviceProvider;
    private readonly LoginNavigationService _loginNavigationService;


    public MainLoginViewModel(IServiceProvider serviceProvider,LoginNavigationService  navigationService,ISukiToastManager toastManager, ISukiDialogManager dialogManager)
    {
        DialogManager = dialogManager;
        ToastManager = toastManager;
        _serviceProvider = serviceProvider;
        _loginNavigationService = navigationService;
        _loginNavigationService.NavigationRequested += OnNavigationRequested;
        ContentViewModel = _serviceProvider.GetRequiredService<SplashViewModel>();
        _ = StartSplashSequence();
        
        
        
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

    private async Task StartSplashSequence()
    {
        await Task.Delay(3600);
        ContentViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();
    }
    
}