using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MoCiVerification.Features;
using MoCiVerification.Message;
using MoCiVerification.Models;
using MoCiVerification.Services;
using SukiUI.Dialogs;

namespace MoCiVerification.ViewModels;

public partial class LoginViewModel : LoginPage
{
    [ObservableProperty] private bool _isLoggingIn;
    [ObservableProperty] private bool _isChecked;
    [ObservableProperty]
    [MinLength(1,ErrorMessage = "用户名至少1个字符")] [MaxLength(20,ErrorMessage = "用户名最多20个字符")] private string? _username;
    [ObservableProperty][Required(ErrorMessage = "密码不能为空")]
    [MinLength(6,ErrorMessage = "密码至少6个字符")] [MaxLength(20,ErrorMessage = "密码最多20个字符")] private string? _password;
    private readonly IAdminService _adminService;
    private readonly ClientSettings _settings;
    private readonly LoginNavigationService _loginNavigationService;
    private readonly ISukiDialogManager _dialogManager;
    private readonly Action avtion;


    public LoginViewModel(ISukiDialogManager dialogManager,IAdminService adminService,ClientSettings clientSettings,LoginNavigationService loginNavigationService) :base("Login")
    {
        _dialogManager = dialogManager;
        _adminService = adminService;
        _settings = clientSettings;
        _loginNavigationService = loginNavigationService;
        Username = _settings.UserName ?? "";
        Password = _settings.Password ?? "";
        if (_settings.IsAuto)
            _ = Task.Run(async () =>
            {
                await LoginAsync();
            }).ConfigureAwait(false);
        //_ = Dispatcher.UIThread.InvokeAsync(AutoLoginAsync);

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
        var (success, message, license) = await _adminService.LoginAsync(Username, Password);
        if (success)
        {
            if (await _adminService.LoginVerificationAsync())
            {
                _settings.UserName = Username;
                _settings.Password = Password;
                _settings.IsAuto = IsChecked;
                _settings.ClientLicense = license;
                _settings.SaveToJson();
                WeakReferenceMessenger.Default.Send(new LoginSuccessMessage());
            }
            else
            {
                await _dialogManager.CreateDialog()
                    .WithTitle("登录失败")
                    .WithContent("回调失败")
                    .WithActionButton("我知道了", _ => { }, true)
                    .OfType(NotificationType.Error)
                    .TryShowAsync();
            }

            
        }
        else
        {
            await _dialogManager.CreateDialog()
                .WithTitle("登录失败")
                .WithContent(message)
                .WithActionButton("我知道了", _ => { }, true)
                .OfType(NotificationType.Error)
                .TryShowAsync();
        }
        IsLoggingIn = false;
    }

    [RelayCommand]
    public async Task ToRegisterAsync()
    {
        _loginNavigationService.RequestNavigation<RegisterViewModel>();
        Username = Password = "";
    }
}