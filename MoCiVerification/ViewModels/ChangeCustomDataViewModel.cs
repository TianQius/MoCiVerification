using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MoCiVerification.Message;
using MoCiVerification.Models;

namespace MoCiVerification.ViewModels;

public partial class ChangeCustomDataViewModel:ObservableObject
{
    public event Action? RequestClose;
    private readonly IAdminService _adminService;
    private readonly ClientSettings _clientSettings;
    [ObservableProperty] private bool _isChanging;
    [ObservableProperty] private string _mask;
    [ObservableProperty] private string _customData;
    [ObservableProperty] private int _getway = 0;


    public ChangeCustomDataViewModel(IAdminService adminService,ClientSettings clientSettings)
    {
        _adminService=adminService;
        _clientSettings=clientSettings;
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
            RequestClose?.Invoke();
        }
    }

}