using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoCiProxyClient.Models;
using MoCiVerification.Services;
using SukiUI.Toasts;

namespace MoCiProxyClient.ViewModels;

public partial class AddCardViewModel:ObservableObject
{
    public event Action? RequestClose;
    [ObservableProperty] private bool _isAddingCard = false;
    [ObservableProperty] private string _prefix;
    [ObservableProperty] private string _cardCount;
    [ObservableProperty] private ComboBoxItem? _cardTypeValue;
    [ObservableProperty] private string _mark;
    private readonly IProxyService _iproxyService;
    private readonly ClientSettings _clientSettings;
    private readonly ISukiToastManager _toastManager;

    public AddCardViewModel(IProxyService iproxyService, ClientSettings clientSettings, ISukiToastManager toastManager)
    {
        _iproxyService = iproxyService;
        _clientSettings = clientSettings;
        _toastManager = toastManager;
    }
    
    [RelayCommand]
    public async Task AddCard()
    {
        IsAddingCard = true;
        if (String.IsNullOrEmpty(Mark)) Mark = "无";
        else if (String.IsNullOrEmpty(Prefix)) Prefix = "无";

        if (int.Parse(CardCount) > 50)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("无法创建卡密")
                .WithContent("数量不应大于50")
                .OfType(NotificationType.Success)
                .Queue();
        }
        var r = await _iproxyService.AddCard( CardCount, (string)CardTypeValue.Content, Prefix, Mark);

        if (r)
        {
            await WriteCardsToDesktopAsync(_clientSettings.GlobalMessage);
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("卡密发生变化")
                .WithContent("创建卡密成功！已生成卡密至桌面（卡密_XXXXX.txt）")
                .OfType(NotificationType.Success)
                .Queue();
            //_clientSettings.GlobalMessage 是卡密数据
            RequestClose?.Invoke();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("创建卡密失败")
                .WithContent(_clientSettings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }
        IsAddingCard = false;

    }
    
    private static async Task WriteCardsToDesktopAsync(string cardData)
    {
        if (string.IsNullOrWhiteSpace(cardData))
            return;
        await File.WriteAllTextAsync(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"卡密_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt")
            , cardData, Encoding.UTF8);
    }
}