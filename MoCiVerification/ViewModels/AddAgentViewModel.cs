using System;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoCiVerification.Models;
using SukiUI.Toasts;

namespace MoCiVerification.ViewModels;

public partial class AddAgentViewModel:ObservableObject
{
    public event Action? RequestClose;
    [ObservableProperty] private bool _isAdding = false;
    [ObservableProperty] private string _userName;
    [ObservableProperty] private string _passWord;
    [ObservableProperty] private string _money;
    private readonly IAdminService _adminService;
    private readonly ClientSettings _clientSettings;
    private readonly ISukiToastManager _toastManager;


    public AddAgentViewModel(ISukiToastManager toastManager,IAdminService adminService,ClientSettings clientSettings)
    {
        _toastManager = toastManager;
        _adminService = adminService;
        _clientSettings = clientSettings;
    }

    [RelayCommand]
    public async Task AddAgent()
    {
        if (string.IsNullOrEmpty(Money)) Money = "0";
        else if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(PassWord))
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("新增代理失败")
                .WithContent("代理名称或代理密码为空！！！")
                .OfType(NotificationType.Error)
                .Queue();
            return;
        }
        var r = await _adminService.AddAgent(_clientSettings.CurrentProjectName, UserName, PassWord, Money);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("代理列表发生变化")
                .WithContent("新增代理成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
            RequestClose?.Invoke();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("新增代理失败")
                .WithContent(_clientSettings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }
    }
    
    
}