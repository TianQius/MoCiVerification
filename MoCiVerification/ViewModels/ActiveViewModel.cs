using System;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoCiVerification.Models;
using MoCiVerification.Services;
using SukiUI.Dialogs;
using SukiUI.Toasts;

namespace MoCiVerification.ViewModels;

public partial class ActiveViewModel : ObservableObject
{
    
    public event Action? RequestClose;
    [ObservableProperty] private bool _isActiving;
    [ObservableProperty] private string _key;
    private readonly IAdminService _adminService;
    private readonly ClientSettings _clientSettings;
    private readonly ISukiToastManager _toastManager;
    

    public ActiveViewModel(ISukiToastManager toastManager,IAdminService adminService,ClientSettings clientSettings)
    {
        _adminService = adminService;
        _clientSettings = clientSettings;
        _toastManager = toastManager;

    }

    [RelayCommand]
    public async Task ActiveDialog()
    {
        IsActiving = true;
        var r = await _adminService.ActiveEmailAsync(_clientSettings.UserName, Key);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("账户发生变化")
                .WithContent("激活账户成功！")
                .OfType(NotificationType.Success)
                .Queue();
            RequestClose?.Invoke();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("激活失败")
                .WithContent(_clientSettings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }
        IsActiving = false;

    }


}