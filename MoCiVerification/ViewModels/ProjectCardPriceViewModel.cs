using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoCiVerification.Models;
using Newtonsoft.Json;
using SukiUI.Toasts;

namespace MoCiVerification.ViewModels;

public partial class ProjectCardPriceViewModel:ObservableValidator
{
    public event Action? RequestClose;
    public event Action? GetProjectCardPriceOptions;
    private static readonly Lazy<Dictionary<string, PropertyInfo>> _cardPropertyCache = 
        new(() => typeof(ProjectCardPrice).GetProperties()
            .Where(p => p.Name.StartsWith("Standard") || p.Name.StartsWith("Proxy"))
            .ToDictionary(p => p.Name, p => p));

    private static readonly Lazy<Dictionary<string, PropertyInfo>> _viewModelPropertyCache = 
        new(() => typeof(ProjectCardPriceViewModel).GetProperties()
            .Where(p => p.Name.StartsWith("Stand") || p.Name.StartsWith("Proxy"))
            .ToDictionary(p => MapCardToViewModelName(p.Name), p => p));
    [ObservableProperty] private bool _isChanging;
    [ObservableProperty] [Price(0.0, 9999.99, ErrorMessage = "价格超出有效范围")]
    private decimal _standCard1 = 0,
        _standCard2 = 0,
        _standCard3 = 0,
        _standCard4 = 0,
        _standCard5 = 0,
        _standCard6 = 0,
        _standCard7 = 0,
        _standCard8 = 0,
        _proxyCard1 = 0,
        _proxyCard2 = 0,
        _proxyCard3 = 0,
        _proxyCard4 = 0,
        _proxyCard5 = 0,
        _proxyCard6 = 0,
        _proxyCard7 = 0,
        _proxyCard8 = 0;
    private readonly IAdminService _adminService;
    private readonly ClientSettings _settings;
    private readonly IShowWindowManager _showWindowManager;
    private readonly ISukiToastManager _toastManager;
    public ProjectCardPriceViewModel(IShowWindowManager iShowWindowManager,IAdminService adminservice, ClientSettings settings,ISukiToastManager toastManager)
    {
        _showWindowManager = iShowWindowManager;
        _adminService = adminservice;
        _settings = settings;
        _toastManager = toastManager;
    }

    public virtual void GetOptions()
    {
        var source = _settings.CurrentProjectCardPrice;
        var cardProperties = typeof(ProjectCardPrice).GetProperties()
            .Where(p => p.Name.StartsWith("Standard") || p.Name.StartsWith("Proxy"));
        foreach (var prop in cardProperties)
        {
            var value = source != null ? (decimal)prop.GetValue(source) : 0m;
            var targetProperty = GetType().GetProperty(MapCardToViewModelName(prop.Name));
            targetProperty?.SetValue(this, value);
        }
    }
    private static string MapCardToViewModelName(string cardName)
    {
        if (cardName.StartsWith("Standard"))
            return "Stand" + cardName.Substring(8);
        return cardName; 
    }
    [RelayCommand]
    public async Task ChangeProjectCardPrice()
    {
        IsChanging = true;
        ValidateAllProperties();
        if (HasErrors)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("保存配置失败！")
                .WithContent(GetErrors().FirstOrDefault().ErrorMessage)
                .OfType(NotificationType.Error)
                .Queue();
            IsChanging = false;
            return;
        }
        var config = new ProjectCardPrice()
        {
            StandardCard1 = StandCard1,
            StandardCard2 = StandCard2,
            StandardCard3 = StandCard3,
            StandardCard4 = StandCard4,
            StandardCard5 = StandCard5,
            StandardCard6 = StandCard6,
            StandardCard7 = StandCard7,
            StandardCard8 = StandCard8,
            ProxyCard1 = ProxyCard1,
            ProxyCard2 = ProxyCard2,
            ProxyCard3 = ProxyCard3,
            ProxyCard4 = ProxyCard4,
            ProxyCard5 = ProxyCard5,
            ProxyCard6 = ProxyCard6,
            ProxyCard7 = ProxyCard7,
            ProxyCard8 = ProxyCard8,
        };
        var r = await _adminService.ChangeProjectCardPrice(_settings.CurrentProjectName,config);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("保存配置成功！")
                .WithContent("项目卡密价格配置已保存！")
                .OfType(NotificationType.Success)
                .Queue();
            RequestClose?.Invoke();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("保存配置失败")
                .WithContent(_settings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }
        IsChanging = false;
    }
    
    
    
}