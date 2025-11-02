using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using MoCiVerification.Features;
using MoCiVerification.Models;
using MoCiVerification.Services;
using MoCiVerification.Views.Windows;
using SukiUI.Dialogs;
using SukiUI.Toasts;

namespace MoCiVerification.ViewModels;

public partial class VersionViewModel:PageBase
{
    [ObservableProperty] private DataGridCollectionView _dataGridContent;
    [ObservableProperty] private VersionDataGridContentViewModel _selectedItem;
    [ObservableProperty] private bool _isLoading = false;
    private readonly IAdminService _adminService;
    private readonly ClientSettings _settings;
    private readonly ISukiToastManager _toastManager;
    private readonly IShowWindowManager _showWindowManager;
    
    public VersionViewModel(IShowWindowManager iShowWindowManager,  ISukiToastManager toastManager,IAdminService adminservice, ClientSettings settings) : base("版本管理", MaterialIconKind.AlphaVBox, 2)
    {
        _showWindowManager = iShowWindowManager;
        _adminService = adminservice;
        _toastManager = toastManager;
        _settings = settings;
    }
    
    private async Task LoadVersionsAsync()
    {
        IsLoading = true;
        var lines = await _adminService.GetVersionListAsync(_settings.CurrentProjectName);
        if (lines != null)
        {
            DataGridContent = new DataGridCollectionView(await Task.Run(() =>
            {
                return lines
                    .AsParallel() 
                    .AsOrdered()  
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Select(VersionDataGridContentViewModel.FromRawLine)
                    .ToArray(); 
            }));
        }
        IsLoading = false;
    }

    [RelayCommand]
    public async Task Reflash()
    {
        await LoadVersionsAsync();

    }
    [RelayCommand]
    public async Task StopVersion()
    {
        var r = await _adminService.StopVersion(_settings.CurrentProjectName, SelectedItem.Version);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("版本发生变化")
                .WithContent("停用版本成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("停用版本失败")
                .WithContent(_settings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }
        
    }
    [RelayCommand]
    public async Task RecoverVersion()
    {
        var r = await _adminService.RecoverVersion(_settings.CurrentProjectName, SelectedItem.Version);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("版本发生变化")
                .WithContent("恢复版本成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("恢复版本失败")
                .WithContent(_settings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }

    }
    [RelayCommand]
    public async Task DeleteVersion()
    {
        var r = await _adminService.DeleteVersion(_settings.CurrentProjectName, SelectedItem.Version);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("版本发生变化")
                .WithContent("删除版本成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("删除版本失败")
                .WithContent(_settings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }

    }
    [RelayCommand]
    public async Task CreateVersion()
    {
        await _showWindowManager.ShowDialogAsync<AddVersionView,AddVersionViewModel>();

    }
    [RelayCommand]
    public async Task ChangeVersionAD()
    {
        _settings.CurrentVersion = SelectedItem.Version;
        _settings.CurrentVersionData = SelectedItem.Data;
        _settings.CurrentVersionAnnouncement = SelectedItem.Announcement;
        await _showWindowManager.ShowDialogAsync<ChangeVersionView,ChangeVersionViewModel>();
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("版本发生变化")
                .WithContent("版本属性修改成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        

    }

    public override async Task OnPageLoadedAsync()
    {
        await LoadVersionsAsync();
    }
    
}


public partial class VersionDataGridContentViewModel : ObservableObject
{
    [ObservableProperty] private string _version = string.Empty;
    [ObservableProperty] private string _updataTime = string.Empty;
    [ObservableProperty] private string _announcement = string.Empty;
    [ObservableProperty] private string _data = string.Empty;
    [ObservableProperty] private string _state = string.Empty;

    public static VersionDataGridContentViewModel FromRawLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return new VersionDataGridContentViewModel();

        var parts = line.Split(new[] { "|||" }, StringSplitOptions.None);

        return new VersionDataGridContentViewModel()
        {
            Version = parts.ElementAtOrDefault(0) ?? string.Empty,
            UpdataTime = parts.ElementAtOrDefault(1) ?? string.Empty,
            Announcement = parts.ElementAtOrDefault(2) ?? string.Empty,
            Data = parts.ElementAtOrDefault(3) ?? string.Empty,
            State = (parts.ElementAtOrDefault(4) ?? string.Empty).TrimEnd()
        };
    }
}
