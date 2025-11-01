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
using MoCiVerification.Services;
using SukiUI.Dialogs;
using SukiUI.Toasts;

namespace MoCiVerification.ViewModels;

public partial class VarViewModel:PageBase
{
    [ObservableProperty] private DataGridCollectionView _dataGridContent;
    [ObservableProperty] private VarDataGridContentViewModel selectedItem;
    [ObservableProperty] private bool _isLoading = false;
    private readonly IAdminService _adminService;
    private readonly ClientSettings _settings;
    private readonly ISukiToastManager _toastManager;
    public VarViewModel(ISukiToastManager toastManager,IAdminService adminservice, ClientSettings settings) : base("云变量", MaterialIconKind.CloudCheck, 7)
    {
        _adminService = adminservice;
        _toastManager = toastManager;
        _settings = settings;
    }
    private async Task LoadVarsAsync()
    {
        IsLoading = true;
        var lines = await _adminService.GetVariableListAsync(_settings.CurrentProjectName);
        if (lines != null)
        {
            DataGridContent = new DataGridCollectionView(await Task.Run(() =>
            {
                return lines
                    .AsParallel() 
                    .AsOrdered()  
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Select(VarDataGridContentViewModel.FromRawLine)
                    .ToArray(); 
            }));
        }
        IsLoading = false;
    }

    [RelayCommand]
    public async Task Reflash()
    {
        await LoadVarsAsync();
    }

    [RelayCommand]
    public async Task DeleteVar()
    {
        var r = await _adminService.DeleteVar(_settings.CurrentProjectName, SelectedItem.Name);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("云变量发生变化")
                .WithContent("删除云变量成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
    }


    public override async Task OnPageLoadedAsync()
    {
        await LoadVarsAsync();
    }
}
public partial class VarDataGridContentViewModel : ObservableObject
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _value = string.Empty;
    [ObservableProperty] private string _ip = string.Empty;

    public static VarDataGridContentViewModel FromRawLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return new VarDataGridContentViewModel();

        var parts = line.Split(new[] { "|||" }, StringSplitOptions.None);

        return new VarDataGridContentViewModel()
        {
            Name = parts.ElementAtOrDefault(0) ?? string.Empty,
            Value = parts.ElementAtOrDefault(1) ?? string.Empty,
            Ip = (parts.ElementAtOrDefault(2) ?? string.Empty).Trim()
        };
    }
}