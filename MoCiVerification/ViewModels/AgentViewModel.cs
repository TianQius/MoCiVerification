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
using SukiUI.Toasts;

namespace MoCiVerification.ViewModels;

public partial class AgentViewModel:PageBase
{
    [ObservableProperty] private DataGridCollectionView? _dataGridContent;
    [ObservableProperty] private AgentDataGridContentViewModel selectedItem;
    [ObservableProperty] private bool _isLoading = false;
    private readonly IAdminService _adminService;
    private readonly ClientSettings _settings;
    private readonly ISukiToastManager _toastManager;
    
    private readonly IShowWindowManager _showWindowManager;

    public AgentViewModel(ISukiToastManager toastManager, IShowWindowManager iShowWindowManager,IAdminService adminservice, ClientSettings settings
    ):base("代理", MaterialIconKind.Proxy, 8)
    {
        _adminService = adminservice;
        _toastManager = toastManager;
        _showWindowManager = iShowWindowManager;
        _settings = settings;
    }
    private async Task LoadAgentsAsync()
    {
        IsLoading = true;
        var lines = await _adminService.GetAgentListAsync(_settings.CurrentProjectName);
        if (lines != null)
        {
            DataGridContent = new DataGridCollectionView(await Task.Run(() =>
            {
                return lines
                    .AsParallel() 
                    .AsOrdered()  
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Select(AgentDataGridContentViewModel.FromRawLine)
                    .ToArray(); 
            }));
        }
        IsLoading = false;
    }

    [RelayCommand]
    public async Task Reflash()
    {
        await LoadAgentsAsync();
    }
    [RelayCommand]
    public async Task ChangeAgentBalance()
    {
        await _showWindowManager.ShowDialogAsync<ChangeAgentBalanceView, ChangeAgentBalanceViewModel>();

    }
    [RelayCommand]
    public async Task AddAgent()
    {
        await _showWindowManager.ShowDialogAsync<AddAgentView, AddAgentViewModel>();

    }
    [RelayCommand]
    public async Task DeleteAgent()
    {
        var r = await _adminService.DeleteAgent(_settings.CurrentProjectName,SelectedItem.Username);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("代理列表发生变化")
                .WithContent("删除代理成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("删除代理失败")
                .WithContent(_settings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }
    }
    [RelayCommand]
    public async Task StopAgent()
    {
        var r = await _adminService.StopAgent(_settings.CurrentProjectName,SelectedItem.Username);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("代理列表发生变化")
                .WithContent("停用代理成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("停用代理失败")
                .WithContent(_settings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }
    }
    [RelayCommand]
    public async Task RecoverAgent()
    {
        var r = await _adminService.RecoverAgent(_settings.CurrentProjectName,SelectedItem.Username);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("代理列表发生变化")
                .WithContent("恢复代理成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("恢复代理失败")
                .WithContent(_settings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }
    }
    [RelayCommand]
    public async Task OffAgent()
    {
        var r = await _adminService.OffAgent(_settings.CurrentProjectName,SelectedItem.Username);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("代理列表发生变化")
                .WithContent("下线代理成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("下线代理失败")
                .WithContent(_settings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }
    }
    
    
    
    
    public override async Task OnPageLoadedAsync()
    {
        if (DataGridContent != null)
        {
            await LoadAgentsAsync();
        }
    }
}

public partial class AgentDataGridContentViewModel : ObservableObject
{
    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _balance = string.Empty;
    [ObservableProperty] private string _state = string.Empty;
    [ObservableProperty] private string _isuse = string.Empty;

    public static AgentDataGridContentViewModel FromRawLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return new AgentDataGridContentViewModel();

        var parts = line.Split(new[] { "|||" }, StringSplitOptions.None);

        return new AgentDataGridContentViewModel()
        {
            Username = parts.ElementAtOrDefault(0) ?? string.Empty, 
            Password= parts.ElementAtOrDefault(1) ?? string.Empty,
            Balance = parts.ElementAtOrDefault(2) ?? string.Empty,
            State = parts.ElementAtOrDefault(3) ?? string.Empty,
            Isuse = (parts.ElementAtOrDefault(4) ?? string.Empty).TrimEnd()
        };
    }
}