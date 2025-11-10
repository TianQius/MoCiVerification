using System;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoCiVerification.Models;
using SukiUI.Toasts;

namespace MoCiVerification.ViewModels;

public partial class ChangeAgentBalanceViewModel:ObservableObject
{
    public event Action? RequestClose;
    private readonly IAdminService _adminService;
    private readonly ClientSettings _clientSettings;
    private readonly ISukiToastManager _toastManager;
    [ObservableProperty] private bool _isChanging;
    [ObservableProperty] private string _money;

    public ChangeAgentBalanceViewModel(ISukiToastManager toastManager,IAdminService adminService, ClientSettings clientSettings)
    {
        _adminService = adminService;
        _clientSettings = clientSettings;
        _toastManager = toastManager;
    }
    [RelayCommand]
    public async Task ChangeBalance()
    {
        var r = await _adminService.ChangeAgentMoney(_clientSettings.CurrentProjectName,
            _clientSettings.CurrentAgentName, Money);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("代理余额发生变化")
                .WithContent("改代理余额成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
            RequestClose?.Invoke();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("修改代理余额失败")
                .WithContent(_clientSettings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }
    }
    
}