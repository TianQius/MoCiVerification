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

public partial class ChangeCustomDataViewModel:ObservableObject
{
    public event Action? RequestClose;
    private readonly IAdminService _adminService;
    private readonly ClientSettings _clientSettings;
    private readonly ISukiToastManager _toastManager;
    [ObservableProperty] private bool _isChanging;
    [ObservableProperty] private string _mask;
    [ObservableProperty] private string _customData;
    [ObservableProperty] private int _getway = 0;


    public ChangeCustomDataViewModel(ISukiToastManager toastManager,IAdminService adminService,ClientSettings clientSettings)
    {
        _adminService=adminService;
        _clientSettings=clientSettings;
        _toastManager = toastManager;
        WeakReferenceMessenger.Default.Register<LoadCustomDataMessage>(this, (recipient, message) =>
        {
            Mask = _clientSettings.CurrentCustomDataMask;
            CustomData = _clientSettings.CurrentCustomDataValue;
            Getway = _clientSettings.CurrentCustomDataGetWay == "直接获取" ? 0 : 1;
        });
    }

    [RelayCommand]
    public async Task ChangeCustomData()
    {
        var r1 = await _adminService.ChangeCustomDataValue(_clientSettings.CurrentProjectName,
            _clientSettings.CurrentCustomDataKey, CustomData);
        var r2 = await _adminService.ChangeCustomDataGetWay(_clientSettings.CurrentProjectName,
            _clientSettings.CurrentCustomDataKey, Getway == 0 ? true : false);
        var r3 = await _adminService.ChangeCustomDataMark(_clientSettings.CurrentProjectName,
            _clientSettings.CurrentCustomDataKey, Mask);
        if (r1 && r2 && r3)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("自定义数据发生变化")
                .WithContent("修改自定义数据成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
            RequestClose?.Invoke();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("修改自定义数据失败")
                .WithContent("修改自定义数据失败！")
                .OfType(NotificationType.Error)
                .Queue();
        }
    }

}