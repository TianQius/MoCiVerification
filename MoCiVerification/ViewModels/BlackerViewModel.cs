using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using MoCiVerification.Features;
using MoCiVerification.Models;
using MoCiVerification.Views.Windows;
using SukiUI.Dialogs;
using SukiUI.Toasts;

namespace MoCiVerification.ViewModels;

public partial class BlackerViewModel:PageBase
{
    [ObservableProperty] private DataGridCollectionView? _dataGridContent;
    [ObservableProperty] private BlackerDataGridContentViewModel selectedItem;
    [ObservableProperty] private bool _isLoading = false;
    private readonly IAdminService _adminService;
    private readonly ClientSettings _settings;
    private readonly ISukiToastManager _toastManager;
    
    private readonly IShowWindowManager _showWindowManager;
    public BlackerViewModel(ISukiToastManager toastManager, IShowWindowManager iShowWindowManager,IAdminService adminservice, ClientSettings settings) :
        base("黑名单", MaterialIconKind.EmoticonDevil, 6)
    {
        _adminService = adminservice;
        _toastManager = toastManager;
        _showWindowManager = iShowWindowManager;
        _settings = settings;
        
    }
    private async Task LoadBlackersAsync()
    {
        IsLoading = true;
        var lines = await _adminService.GetBlackerListAsync(_settings.CurrentProjectName);
        if (lines != null)
        {
            DataGridContent = new DataGridCollectionView(await Task.Run(() =>
            {
                return lines
                    .AsParallel() 
                    .AsOrdered()  
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Select(BlackerDataGridContentViewModel.FromRawLine)
                    .ToArray(); 
            }));
        }
        IsLoading = false;
    }

    [RelayCommand]
    public async Task Reflash()
    {
        await LoadBlackersAsync();
    }
    [RelayCommand]
    public async Task CreateBlacker()
    {
        await _showWindowManager.ShowDialogAsync<AddBlackerView, AddBlackerViewModel>();
        
    }

    [RelayCommand]
    public async Task DeleteBlacker()
    {
        var r = await _adminService.DeleteBlacker(_settings.CurrentProjectName,SelectedItem.Value);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("黑名单发生变化")
                .WithContent("删除黑名单成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
    }


    public override async Task OnPageLoadedAsync()
    {
        if (DataGridContent != null)
        {
            await LoadBlackersAsync();
        }
    }
}

public partial class BlackerDataGridContentViewModel : ObservableObject
{
    [ObservableProperty] private string _value = string.Empty;
    [ObservableProperty] private string _reason = string.Empty;
    [ObservableProperty] private string _way = string.Empty;
    [ObservableProperty] private string _time = string.Empty;

    public static BlackerDataGridContentViewModel FromRawLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return new BlackerDataGridContentViewModel();

        var parts = line.Split(new[] { "|||" }, StringSplitOptions.None);

        return new BlackerDataGridContentViewModel()
        {
            Value = parts.ElementAtOrDefault(0) ?? string.Empty, 
            Reason= parts.ElementAtOrDefault(1) ?? string.Empty,
            Way = parts.ElementAtOrDefault(2) ?? string.Empty,
            Time = (parts.ElementAtOrDefault(3) ?? string.Empty).TrimEnd()
        };
    }
}