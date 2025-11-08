using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoCiProxyClient.Features;
using MoCiProxyClient.Models;
using MoCiProxyClient.Services;
using MoCiProxyClient.Views;
using MoCiVerification.Services;
using SukiUI.Toasts;

namespace MoCiProxyClient.ViewModels;

public partial class ProxyViewModel:LoginPage
{
    [ObservableProperty] private DataGridCollectionView _dataGridContent;
    [ObservableProperty] private ProxyDataGridContent selectedItem;
    [ObservableProperty] private ObservableCollection<ProxyDataGridContent> _selectedItems = new();
    [ObservableProperty] private bool _isLoading = false;
    private readonly ClientSettings _settings;
    private readonly ISukiToastManager _toastManager;
    private readonly IProxyService _proxyService;
    private readonly IShowWindowManager _showWindowManager;
    public ProxyViewModel(ISukiToastManager toastManager, ClientSettings settings,IProxyService proxyService,IShowWindowManager ishowwindowmanager) : base("Proxy")
    {
        _settings = settings;
        _toastManager = toastManager;
        _proxyService = proxyService;
        _showWindowManager = ishowwindowmanager;
    }
    [RelayCommand]
    public async Task AddCard()
    {
        await _showWindowManager.ShowDialogAsync<AddCardView, AddCardViewModel>();

    }
    
    [RelayCommand]
    public async Task Reflash()
    {
        await LoadCardsAsync();

    }
    [RelayCommand]
    public async Task DeleteCard()
    {
        if (SelectedItems.Count == 1)
        {
            var r = await _proxyService.DeleteCard( SelectedItem.Card);
            if (r)
            {
                _toastManager.CreateSimpleInfoToast()
                    .WithTitle("卡密发生变化")
                    .WithContent("删除卡密成功！请耐心等待并刷新（有缓存）")
                    .OfType(NotificationType.Success)
                    .Queue();
            }
            else
            {
                _toastManager.CreateSimpleInfoToast()
                    .WithTitle("删除卡密失败")
                    .WithContent(_settings.GlobalMessage)
                    .OfType(NotificationType.Error)
                    .Queue();
            }
        }
        else
        {
            foreach (var i in SelectedItems)
            {
                if(!await _proxyService.DeleteCard(i.Card))
                {
                    _toastManager.CreateSimpleInfoToast()
                        .WithTitle("删除卡密失败")
                        .WithContent(_settings.GlobalMessage)
                        .OfType(NotificationType.Error)
                        .Queue();
                }
            }
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("卡密发生变化")
                .WithContent("批量删除卡密已完成！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
        

    }
    public override async Task OnPageLoadedAsync()
    {
        await LoadCardsAsync();
    }
    private async Task LoadCardsAsync()
    {
        IsLoading = true;
        var lines = await _proxyService.GetAgentCardList();
        if (lines != null)
        {
            DataGridContent = new DataGridCollectionView(await Task.Run(() =>
            {
                return lines
                    .AsParallel() 
                    .AsOrdered()  
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Select(ProxyDataGridContent.FromRawLine)
                    .ToArray(); 
            }));
        }
        IsLoading = false;
    }
}
public partial class ProxyDataGridContent : ObservableObject
{
    [ObservableProperty] private string _card = string.Empty;
    [ObservableProperty] private string _type = string.Empty;
    [ObservableProperty] private string _usedtime = string.Empty;
    [ObservableProperty] private string _createdtime = string.Empty;
    [ObservableProperty] private string _isused = string.Empty;
    [ObservableProperty] private string _usedUser = string.Empty;
    [ObservableProperty] private string _remark = string.Empty;

    public static ProxyDataGridContent FromRawLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return new ProxyDataGridContent();

        var parts = line.Split(new[] { "|||" }, StringSplitOptions.None);

        return new ProxyDataGridContent()
        {
            Card = parts.ElementAtOrDefault(0) ?? string.Empty,
            Type = parts.ElementAtOrDefault(1) ?? string.Empty,
            Usedtime = parts.ElementAtOrDefault(2) ?? string.Empty,
            Createdtime = parts.ElementAtOrDefault(3) ?? string.Empty,
            Isused = parts.ElementAtOrDefault(4) ?? string.Empty,
            UsedUser = parts.ElementAtOrDefault(5) ?? string.Empty,
            Remark = (parts.ElementAtOrDefault(6) ?? string.Empty).TrimEnd()
        };
    }
}