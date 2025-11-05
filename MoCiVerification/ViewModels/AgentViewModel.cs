using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using MoCiVerification.Features;
using MoCiVerification.Models;
using SukiUI.Toasts;

namespace MoCiVerification.ViewModels;

public partial class AgentViewModel:PageBase
{
    [ObservableProperty] private DataGridCollectionView? _dataGridContent;
    [ObservableProperty] private BlackerDataGridContentViewModel selectedItem;
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