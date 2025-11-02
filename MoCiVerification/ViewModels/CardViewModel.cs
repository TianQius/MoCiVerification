using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Material.Icons;
using MoCiVerification.Features;
using MoCiVerification.Message;
using MoCiVerification.Models;
using MoCiVerification.Views.Windows;
using SukiUI.Converters;
using SukiUI.Dialogs;
using SukiUI.Toasts;

namespace MoCiVerification.ViewModels;

public partial class CardViewModel:PageBase
{
    [ObservableProperty] private DataGridCollectionView _dataGridContent;
    [ObservableProperty] private CardDataGridContentViewModel selectedItem;
    [ObservableProperty] private ObservableCollection<CardDataGridContentViewModel> _selectedItems = new();
    [ObservableProperty] private bool _isLoading = false;
    private readonly IAdminService _adminService;
    private readonly ClientSettings _settings;
    private readonly ISukiToastManager _toastManager;
    
    private readonly IShowWindowManager _showWindowManager;
    
    public CardViewModel(ISukiToastManager toastManager,IShowWindowManager iShowWindowManager,IAdminService adminservice, ClientSettings settings) : base("卡密管理", MaterialIconKind.CardAccountDetails, 3)
    {
        _adminService = adminservice;
        _showWindowManager = iShowWindowManager;
        _toastManager = toastManager;
        _settings = settings;
    }
    
    private async Task LoadCardsAsync()
    {
        IsLoading = true;
        var lines = await _adminService.GetCardListAsync(_settings.CurrentProjectName);
        if (lines != null)
        {
            DataGridContent = new DataGridCollectionView(await Task.Run(() =>
            {
                return lines
                    .AsParallel() 
                    .AsOrdered()  
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Select(CardDataGridContentViewModel.FromRawLine)
                    .ToArray(); 
            }));
        }
        IsLoading = false;
    }

    [RelayCommand]
    public async Task Reflash()
    {
        await LoadCardsAsync();

    }
    [RelayCommand]
    public void FindCard()
    {
        WeakReferenceMessenger.Default.Unregister<FindRequestMessage>(_settings.SearchRecipient);
        WeakReferenceMessenger.Default.Register<FindRequestMessage>(_settings.SearchRecipient, (r, m) =>
        {
            var keyword = m.Keyword?.Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                DataGridContent.Filter = null;
            }
            else
            {
                DataGridContent.Filter = (obj) =>
                {
                    if (obj is CardDataGridContentViewModel item)
                    {
                        return item.Card.Contains(keyword, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        });
        _showWindowManager.Show<FindView,FindViewModel>();
    }
    [RelayCommand]
    public async Task DeleteCard()
    {
        if (SelectedItems.Count == 1)
        {
            var r = await _adminService.DeleteCard(_settings.CurrentProjectName, SelectedItem.Card);
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
                if(!await _adminService.DeleteCard(_settings.CurrentProjectName, i.Card))
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
    [RelayCommand]
    public async Task CreateCard()
    {
        await _showWindowManager.ShowDialogAsync<AddCardView,AddCardViewModel>();
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("卡密发生变化")
                .WithContent("创建卡密成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
    }

    public override async Task OnPageLoadedAsync()
    {
        await LoadCardsAsync();
    }
}

public partial class CardDataGridContentViewModel : ObservableObject
{
    [ObservableProperty] private string _card = string.Empty;
    [ObservableProperty] private string _type = string.Empty;
    [ObservableProperty] private string _usedtime = string.Empty;
    [ObservableProperty] private string _createdtime = string.Empty;
    [ObservableProperty] private string _isused = string.Empty;
    [ObservableProperty] private string _usedUser = string.Empty;
    [ObservableProperty] private string _remark = string.Empty;
    [ObservableProperty] private string _marker = string.Empty;

    public static CardDataGridContentViewModel FromRawLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return new CardDataGridContentViewModel();

        var parts = line.Split(new[] { "|||" }, StringSplitOptions.None);

        return new CardDataGridContentViewModel()
        {
            Card = parts.ElementAtOrDefault(0) ?? string.Empty,
            Type = parts.ElementAtOrDefault(1) ?? string.Empty,
            Usedtime = parts.ElementAtOrDefault(2) ?? string.Empty,
            Createdtime = parts.ElementAtOrDefault(3) ?? string.Empty,
            Isused = parts.ElementAtOrDefault(4) ?? string.Empty,
            UsedUser = parts.ElementAtOrDefault(5) ?? string.Empty,
            Remark = parts.ElementAtOrDefault(6) ?? string.Empty,
            Marker = (parts.ElementAtOrDefault(7) ?? string.Empty).TrimEnd()
        };
    }
}
