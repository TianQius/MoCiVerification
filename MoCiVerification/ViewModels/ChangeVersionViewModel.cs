using System;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MoCiVerification.Message;
using MoCiVerification.Models;
using SukiUI.Toasts;

namespace MoCiVerification.ViewModels;

public partial class ChangeVersionViewModel:ObservableObject
{
    public event Action? RequestClose;
    [ObservableProperty] private bool _isChanging;
    [ObservableProperty] private string _versionAnnounce;
    [ObservableProperty] private string _versionData;
    
    private readonly IAdminService _adminService;
    private readonly ClientSettings _clientSettings;
    private readonly ISukiToastManager _toastManager;

    public ChangeVersionViewModel(ISukiToastManager toastManager,IAdminService adminService,ClientSettings clientSettings)
    {
        _adminService = adminService;
        _clientSettings = clientSettings;
        _toastManager = toastManager;
        WeakReferenceMessenger.Default.Register<LoadVersionMessage>(this, (recipient, message) =>
        {
            VersionAnnounce = _clientSettings.CurrentVersionAnnouncement;
            VersionData = _clientSettings.CurrentVersionData;
        });
        
    }
    [RelayCommand]
    public async Task ChangeVersion()
    {
        IsChanging = true;
        var r = await _adminService.ChangeVersionAnnouncement(_clientSettings.CurrentProjectName, _clientSettings.CurrentVersion,VersionAnnounce);
        var rm = _clientSettings.GlobalMessage;
        var r2 = await _adminService.ChangeVersionData(_clientSettings.CurrentProjectName, _clientSettings.CurrentVersion,VersionData);
        var rm2 = _clientSettings.GlobalMessage;
        if (r && r2)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("版本属性发生变化")
                .WithContent("修改版本属性成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
            RequestClose?.Invoke();
        }
        else if (!r && r2)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("修改版本公告失败")
                .WithContent(rm)
                .OfType(NotificationType.Error)
                .Queue();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("修改版本数据失败")
                .WithContent(rm2)
                .OfType(NotificationType.Error)
                .Queue();
        }

        IsChanging = false;
        
    }
    
    
    
    
}