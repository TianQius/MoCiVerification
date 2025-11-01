using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoCiVerification.Models;
using SukiUI.Toasts;

namespace MoCiVerification.ViewModels;

public partial class AddCustomDataViewModel:ObservableObject
{
    public event Action? RequestClose;
    [ObservableProperty] private bool _isAddingData = false;
    [ObservableProperty] private string _key;
    [ObservableProperty] private string _value;
    [ObservableProperty] private string _mark;
    [ObservableProperty] private ComboBoxItem _getWay;
    
    private readonly IAdminService _adminService;
    private readonly ClientSettings _clientSettings;
    private readonly ISukiToastManager _toastManager;

    public AddCustomDataViewModel(ISukiToastManager toastManager,IAdminService adminService, ClientSettings clientSettings)
    {
        _adminService = adminService;
        _clientSettings = clientSettings;
        _toastManager = toastManager;
    }
    [RelayCommand]
    public async Task AddCustomData()
    {
        IsAddingData = true;
        if (String.IsNullOrEmpty(Mark)) Mark = "无";

        if (String.IsNullOrEmpty(Key) || String.IsNullOrEmpty(Value))
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("数据发生变化")
                .WithContent("创建数据失败！Key或Value为空")
                .OfType(NotificationType.Error)
                .Queue();
        }
        else
        {
            var r = await _adminService.CreateCustomData(_clientSettings.CurrentProjectName, Key, Value, Mark,
                GetWay.Content.ToString());
            if (r)
            {
                _toastManager.CreateSimpleInfoToast()
                    .WithTitle("卡密发生变化")
                    .WithContent("创建卡密成功！请耐心等待并刷新（有缓存）")
                    .OfType(NotificationType.Success)
                    .Queue();
                RequestClose?.Invoke();
            }
        }
        IsAddingData = false;


    }

}