using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MoCiProxyClient.Features;
using MoCiProxyClient.Models;
using MoCiVerification.Services;
using SukiUI.Dialogs;

namespace MoCiProxyClient.ViewModels;

public partial class LoginViewModel:LoginPage
{
    [ObservableProperty] private bool _isLoggingIn;
    [ObservableProperty] private bool _isChecked;
    [ObservableProperty]
    [MinLength(1,ErrorMessage = "用户名至少1个字符")] [MaxLength(20,ErrorMessage = "用户名最多20个字符")] private string? _username;
    [ObservableProperty]
    [MinLength(1,ErrorMessage = "TOKEN至少1个字符")] private string? _token;
    [ObservableProperty][Required(ErrorMessage = "密码不能为空")]
    [MinLength(3,ErrorMessage = "密码至少3个字符")] [MaxLength(20,ErrorMessage = "密码最多20个字符")] private string? _password;
    private readonly LoginNavigationService _loginNavigationService;
    private readonly ISukiDialogManager _dialogManager;
    private readonly IProxyService _proxyService;
    private readonly ClientSettings _clientSettings;
    private readonly Action avtion;
    public LoginViewModel(ISukiDialogManager dialogManager,LoginNavigationService loginNavigationService,
        IProxyService iProxyService,ClientSettings clientSettings) :base("Login")
    {
        _dialogManager = dialogManager;
        _loginNavigationService = loginNavigationService;
        _proxyService = iProxyService;
        _clientSettings = clientSettings;

    }
    [RelayCommand]
    public async Task LoginAsync()
    {
        IsLoggingIn = true;
        ValidateAllProperties();
        if (HasErrors)
        {
            await _dialogManager.CreateDialog()
                .OfType(NotificationType.Error)
                .WithTitle("登录失败")
                .WithContent(GetErrors().FirstOrDefault().ErrorMessage)
                .WithOkResult("我知道了")
                .Dismiss().ByClickingBackground()
                .TryShowAsync();
            IsLoggingIn = false;
            return;
        }
        var r = await _proxyService.AgentLogin(Token, Username, Password);
        if (r)
        {
            _loginNavigationService.RequestNavigation<ProxyViewModel>();
            IsLoggingIn = false;
        }
        else
        {
            _dialogManager.CreateDialog()
                .WithTitle("登录失败")
                .WithContent(_clientSettings.GlobalMessage)
                .WithActionButton("我知道了", _ => { }, true)
                .OfType(NotificationType.Error)
                .TryShow();
        }
        IsLoggingIn = false;
    }
}