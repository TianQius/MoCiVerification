using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoCiVerification.Models;
using SukiUI.Toasts;

namespace MoCiVerification.ViewModels;

public partial class AddVersionViewModel:ObservableObject
{
    public event Action? RequestClose;
    [ObservableProperty] private bool _isAddingVersion = false;
    [ObservableProperty] private string _version;
    [ObservableProperty] private ComboBoxItem _state;
    [ObservableProperty] private string _versionAnnounce;
    [ObservableProperty] private string _versionData;
    private readonly IAdminService _adminService;
    private readonly ClientSettings _clientSettings;
    private readonly ISukiToastManager _toastManager;

    public AddVersionViewModel(ISukiToastManager toastManager,IAdminService adminService,ClientSettings clientSettings)
    {
        _adminService = adminService;
        _clientSettings = clientSettings;
        _toastManager = toastManager;
    }

    [RelayCommand]
    public async Task CreateVersion()
    {
        if (String.IsNullOrEmpty(VersionAnnounce)) VersionAnnounce = "无";
        else if (String.IsNullOrEmpty(VersionData)) VersionData = "无";

        if (String.IsNullOrEmpty(Version))
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("创建版本失败")
                .WithContent("版本号不可为空！！！")
                .OfType(NotificationType.Error)
                .Queue();
        }
        else
        {
            var r = await _adminService.CreateVersion(_clientSettings.CurrentProjectName, Version, VersionAnnounce,
                VersionData, State.Content.ToString());
            if (r)
            {
                _toastManager.CreateSimpleInfoToast()
                    .WithTitle("创建版本失败")
                    .WithContent("创建版本成功！请耐心等待并刷新（有缓存）")
                    .OfType(NotificationType.Success)
                    .Queue();
                RequestClose?.Invoke();
            }
        }



    }
    
    
    
    
}