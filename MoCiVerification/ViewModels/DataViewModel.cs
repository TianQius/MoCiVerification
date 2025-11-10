using System;
using System.Collections.ObjectModel;
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
public partial class DataViewModel:PageBase
{
    [ObservableProperty] private DataGridCollectionView _dataGridContent;
    [ObservableProperty] private CustomDataGridContentViewModel _selectedItem;
    [ObservableProperty] private ObservableCollection<CustomDataGridContentViewModel> _selectedItems = new();
    [ObservableProperty] private bool _isLoading = false;
    private readonly IAdminService _adminService;
    private readonly ClientSettings _settings;
    private readonly ISukiToastManager _toastManager;
    private readonly IShowWindowManager _showWindowManager;
    
    public DataViewModel(ISukiToastManager toastManager,IAdminService adminservice, ClientSettings settings,IShowWindowManager iShowWindowManager) : base("自定义数据", MaterialIconKind.Database, 5)
    {
        _adminService = adminservice;
        _toastManager  = toastManager;
        _settings = settings;
        _showWindowManager = iShowWindowManager;
    }
    
    private async Task LoadDatasAsync()
    {
        IsLoading = true;
        var lines = await _adminService.GetDataListAsync(_settings.CurrentProjectName);
        if (lines != null)
        {
            DataGridContent = new DataGridCollectionView(await Task.Run(() =>
            {
                return lines
                    .AsParallel() 
                    .AsOrdered()  
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Select(CustomDataGridContentViewModel.FromRawLine)
                    .ToArray(); 
            }));
        }
        IsLoading = false;
    }

    [RelayCommand]
    public async Task Reflash()
    {
        await LoadDatasAsync();
        
    }
    [RelayCommand]
    public async Task DeleteCustomData()
    {
        if (SelectedItems.Count == 1)
        {
            var r= await _adminService.DeleteCustomData(_settings.CurrentProjectName, SelectedItem.Key);
            if (r)
            {
                _toastManager.CreateSimpleInfoToast()
                    .WithTitle("数据发生变化")
                    .WithContent("删除数据成功！请耐心等待并刷新（有缓存）")
                    .OfType(NotificationType.Success)
                    .Queue();
            }
            else
            {
                _toastManager.CreateSimpleInfoToast()
                    .WithTitle("删除数据失败")
                    .WithContent(_settings.GlobalMessage)
                    .OfType(NotificationType.Error)
                    .Queue();
            }
        }
        else
        {
            foreach (var i in SelectedItems)
            {
                if (!await _adminService.DeleteCustomData(_settings.CurrentProjectName, i.Key))
                {
                    _toastManager.CreateSimpleInfoToast()
                        .WithTitle("删除数据失败")
                        .WithContent(_settings.GlobalMessage)
                        .OfType(NotificationType.Error)
                        .Queue();
                }
            }
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("数据发生变化")
                .WithContent("批量删除数据成已完成！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
       
    }
    [RelayCommand]
    public async Task CreateCustomData()
    {
        await _showWindowManager.ShowDialogAsync<AddCustomDataView,AddCustomDataViewModel>();
    }
    [RelayCommand]
    public async Task ChangeCustomData()
    {
        _settings.CurrentCustomDataValue = SelectedItem.Value;
        _settings.CurrentCustomDataGetWay = SelectedItem.GetWay;
        _settings.CurrentCustomDataKey = SelectedItem.Key;
        _settings.CurrentCustomDataMask = SelectedItem.Remark;
        await _showWindowManager.ShowDialogAsync<ChangeCustomDataView,ChangeCustomDataViewModel>();
            
        
    }
    

    public override async Task OnPageLoadedAsync()
    {
        await LoadDatasAsync();
    }
}

public partial class CustomDataGridContentViewModel : ObservableObject
{
    [ObservableProperty] private string _key = string.Empty;
    [ObservableProperty] private string _value = string.Empty;
    [ObservableProperty] private string _remark = string.Empty;
    [ObservableProperty] private string _getWay = string.Empty;

    public static CustomDataGridContentViewModel FromRawLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return new CustomDataGridContentViewModel();

        var parts = line.Split(new[] { "|||" }, StringSplitOptions.None);

        return new CustomDataGridContentViewModel()
        {
            Key = parts.ElementAtOrDefault(0) ?? string.Empty,
            Value = parts.ElementAtOrDefault(1) ?? string.Empty,
            Remark = parts.ElementAtOrDefault(2) ?? string.Empty,
            GetWay = (parts.ElementAtOrDefault(3) ?? string.Empty).TrimEnd()
        };
    }
}