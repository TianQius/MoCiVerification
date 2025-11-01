using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoCiVerification.Models;
using SukiUI.Toasts;

namespace MoCiVerification.ViewModels;

public partial class AddCardViewModel:ObservableObject
{
    public event Action? RequestClose;
    [ObservableProperty] private bool _isAddingCard = false;
    [ObservableProperty] private string _prefix;
    [ObservableProperty] private string _cardCount;
    [ObservableProperty] private ComboBoxItem _cardTypeValue;
    [ObservableProperty] private string _mark;
    private readonly IAdminService _adminService;
    private readonly ClientSettings _clientSettings;
    private readonly ISukiToastManager _toastManager;

    public AddCardViewModel(ISukiToastManager toastManager,IAdminService adminService,ClientSettings clientSettings)
    {
        _adminService = adminService;
        _clientSettings = clientSettings;
        _toastManager = toastManager;
    }

    [RelayCommand]
    public async Task AddCard()
    {
        IsAddingCard = true;
        if (String.IsNullOrEmpty(Mark)) Mark = "无";
        else if (String.IsNullOrEmpty(Prefix)) Prefix = "无";
        var r = await _adminService.CreateCard(_clientSettings.CurrentProjectName,
            int.Parse(CardCount), (string)CardTypeValue.Content, Prefix, Mark);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("卡密发生变化")
                .WithContent("创建卡密成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
            RequestClose?.Invoke();
        }
        IsAddingCard=false;


    }
    
    
    
    
}