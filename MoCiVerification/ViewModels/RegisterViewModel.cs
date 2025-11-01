using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoCiVerification.Features;
using MoCiVerification.Models;
using MoCiVerification.Services;
using MoCiVerification.Views.Windows;
using SukiUI.Dialogs;

namespace MoCiVerification.ViewModels;

public partial class RegisterViewModel : LoginPage
{
    [ObservableProperty] private bool _isRegistering;
    [ObservableProperty] private bool _isChecked;
    [ObservableProperty] [Required(ErrorMessage = "用户名不能为空")]
    [MinLength(1,ErrorMessage = "用户名至少1个字符")] [MaxLength(20,ErrorMessage = "用户名最多20个字符")] private string? _username;
    [ObservableProperty][Required(ErrorMessage = "密码不能为空")]
    [MinLength(6,ErrorMessage = "密码至少6个字符")] [MaxLength(20,ErrorMessage = "密码最多20个字符")] private string? _password;
    [ObservableProperty][Required(ErrorMessage = "邮箱不能为空")] 
    [MinLength(6,ErrorMessage = "邮箱至少6个字符")] [MaxLength(30,ErrorMessage = "邮箱最多30个字符")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")] private string? _email;
    private readonly IAdminService _adminService;
    private readonly ClientSettings _settings;
    private readonly LoginNavigationService _loginNavigationService;
    private readonly ISukiDialogManager _dialogManager;
    private readonly IShowWindowManager _showWindowManager;


    public RegisterViewModel(ISukiDialogManager dialogManager,IShowWindowManager iShowWindowManager,LoginNavigationService loginNavigationService,IAdminService adminService,ClientSettings clientSettings) :base("Register")
    {
        _dialogManager = dialogManager;
        _showWindowManager = iShowWindowManager;
        _loginNavigationService= loginNavigationService;
        _adminService = adminService;
        _settings = clientSettings;

    }
    [RelayCommand]
    public async Task RegisterAsync()
    {
        IsRegistering = true;
        ValidateAllProperties();
        if (HasErrors)
        {
            await _dialogManager.CreateDialog()
                .OfType(NotificationType.Error)
                .WithTitle("注册失败")
                .WithContent(GetErrors().FirstOrDefault().ErrorMessage)
                .WithOkResult("我知道了")
                .Dismiss().ByClickingBackground()
                .TryShowAsync();
            IsRegistering = false;
            return;
        }
        var (success, isactive) = await _adminService.RegisterAsync(Username, Password, Email);
        if (success)
        {
            if (!isactive)
            {
                _settings.UserName = Username;
                _settings.Password = Password;
                _showWindowManager.Show<ActiveView,ActiveViewModel>();
                //_dialogManager.CreateDialog()
                 //   .WithViewModel(dialog => new ActiveViewModel(Username,dialog,_adminService))
                  //  .TryShow();
                _settings.SaveToJson();
            }
        }
        else
        {
            var r = _dialogManager.CreateDialog()
                .WithTitle("账号注册失败")
                .WithContent("服务器连接失败,请检查网络连接是否异常")
                .WithActionButton("我知道了", _ => { }, true)
                .TryShowAsync();
        }
        IsRegistering = false;
    }
    [RelayCommand]
    public void ToLoginAsync()
    {
        _loginNavigationService.RequestNavigation<LoginViewModel>();
        Username = Password = Email = "";
    }
    
}