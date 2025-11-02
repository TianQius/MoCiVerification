using System;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoCiVerification.Models;
using SukiUI.Toasts;

namespace MoCiVerification.ViewModels;

public partial class AddBlackerViewModel:ObservableObject
{
    public event Action? RequestClose;
    [ObservableProperty] private bool _isAddingBlacker = false;
    [ObservableProperty] private string _value;
    [ObservableProperty] private string _reason;
    private readonly IAdminService _adminService;
    private readonly ClientSettings _clientSettings;
    private readonly ISukiToastManager _toastManager;

    public AddBlackerViewModel(ISukiToastManager toastManager,IAdminService adminService,ClientSettings clientSettings)
    {
        _adminService = adminService;
        _clientSettings = clientSettings;
        _toastManager = toastManager;
    }

    [RelayCommand]
    public async Task AddBlacker()
    {
        if (string.IsNullOrEmpty(Reason)) Reason = "无";

        var r = await _adminService.CreateBlacker(_clientSettings.CurrentProjectName, Value, Reason);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("黑名单发生变化")
                .WithContent("创建黑名成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
            RequestClose?.Invoke();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("创建黑名失败")
                .WithContent(_clientSettings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }



    }
    
}