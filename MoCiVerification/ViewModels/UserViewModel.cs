using System;
using System.Collections.ObjectModel;
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
using SukiUI.Toasts;

namespace MoCiVerification.ViewModels;

public partial class UserViewModel:PageBase
{
    [ObservableProperty] private DataGridCollectionView _dataGridContent;
    [ObservableProperty] private UserDataGridContentViewModel selectedItem;
    [ObservableProperty] private ObservableCollection<UserDataGridContentViewModel> _selectedItems = new();
    [ObservableProperty] private bool _isLoading = false;
    private readonly IAdminService _adminService;
    private readonly ClientSettings _settings;
    private readonly ISukiToastManager _toastManager;
    private readonly IShowWindowManager _showWindowManager;
    public UserViewModel(ISukiToastManager toastManager,IShowWindowManager iShowWindowManager,IAdminService adminservice, ClientSettings settings) : base("用户管理", MaterialIconKind.AccountBox, 4)
    {
        _adminService = adminservice;
        _toastManager = toastManager;
        _showWindowManager= iShowWindowManager;
        _settings = settings;
    }
    private async Task LoadUsersAsync()
    {
        IsLoading = true;
        var lines = await _adminService.GetUserListAsync(_settings.CurrentProjectName);
        if (lines != null)
        {
            DataGridContent = new DataGridCollectionView(await Task.Run(() =>
            {
                return lines
                    .AsParallel() 
                    .AsOrdered()  
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Select(UserDataGridContentViewModel.FromRawLine)
                    .ToArray(); 
            }));
        }
        IsLoading = false;
    }

    [RelayCommand]
    public async Task Reflash()
    {
        await LoadUsersAsync();
    }
    [RelayCommand]
    public void FindUser()
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
                    if (obj is UserDataGridContentViewModel item)
                    {
                        return item.UserName.Contains(keyword, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        });
        _showWindowManager.Show<FindView,FindViewModel>();
    }
    [RelayCommand]
    public async Task StopUser()
    {
        var r= await _adminService.StopUser(_settings.CurrentProjectName,SelectedItem.UserName);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("用户发生变化")
                .WithContent("停用用户成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("停用用户失败")
                .WithContent(_settings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }
    }
    [RelayCommand]
    public async Task OffUser()
    {
        var r = await _adminService.OffUser(_settings.CurrentProjectName, SelectedItem.UserName);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("用户发生变化")
                .WithContent("下线用户成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("下线用户失败")
                .WithContent(_settings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }
    }
    [RelayCommand]
    public async Task DeleteUser()
    {
        var r = await _adminService.DeleteUser(_settings.CurrentProjectName, SelectedItem.UserName);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("用户发生变化")
                .WithContent("删除用户成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("删除用户失败")
                .WithContent(_settings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }
    }
    [RelayCommand]
    public async Task RecoverUser()
    {
        var r = await _adminService.RecoverUser(_settings.CurrentProjectName, SelectedItem.UserName);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("用户发生变化")
                .WithContent("恢复用户成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("恢复用户失败")
                .WithContent(_settings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }
    }
    [RelayCommand]
    public async Task UnBindUser()
    {
        var r = await _adminService.UnBindUser(_settings.CurrentProjectName, SelectedItem.UserName);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("用户发生变化")
                .WithContent("解绑用户成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("解绑用户失败")
                .WithContent(_settings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }
    }
    [RelayCommand]
    public async Task ClearUserBindTimes()
    {
        var r = await _adminService.ClearUserBindTimes(_settings.CurrentProjectName, SelectedItem.UserName);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("用户发生变化")
                .WithContent("清空绑定次数成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("清空绑定次数失败")
                .WithContent(_settings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }
    }
    [RelayCommand]
    public async Task UserToBlacker()
    {
        var r = await _adminService.CreateBlacker(_settings.CurrentProjectName, SelectedItem.UserName, "作者拉黑");
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("用户发生变化")
                .WithContent("拉黑用户成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("拉黑用户失败")
                .WithContent(_settings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }
    }
    [RelayCommand]
    public async Task MachineCodeToBlacker()
    {
        var r = await _adminService.CreateBlacker(_settings.CurrentProjectName, SelectedItem.MachineCode, "作者拉黑");
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("用户发生变化")
                .WithContent("拉黑机器码成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("拉黑机器码失败")
                .WithContent(_settings.GlobalMessage)
                .OfType(NotificationType.Error)
                .Queue();
        }
    }

    public override async Task OnPageLoadedAsync()
    {
        await LoadUsersAsync();

    }
}
public partial class UserDataGridContentViewModel : ObservableObject
{
    [ObservableProperty] private string _userName = string.Empty;
    [ObservableProperty] private string _machineCode = string.Empty;
    [ObservableProperty] private string _ip = string.Empty;
    [ObservableProperty] private string _deadTime = string.Empty;
    [ObservableProperty] private string _registerTime = string.Empty;
    [ObservableProperty] private string _loginTime = string.Empty;
    [ObservableProperty] private string _attach = string.Empty;
    [ObservableProperty] private string _unBind = string.Empty;
    [ObservableProperty] private string _isStop = string.Empty;
    [ObservableProperty] private string _isOnline = string.Empty;

    public static UserDataGridContentViewModel FromRawLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return new UserDataGridContentViewModel();

        var parts = line.Split(new[] { "|||" }, StringSplitOptions.None);

        return new UserDataGridContentViewModel()
        {
            UserName = parts.ElementAtOrDefault(0) ?? string.Empty,
            MachineCode = parts.ElementAtOrDefault(1) ?? string.Empty,
            Ip = parts.ElementAtOrDefault(2) ?? string.Empty,
            DeadTime = parts.ElementAtOrDefault(3) ?? string.Empty,
            RegisterTime = parts.ElementAtOrDefault(4) ?? string.Empty,
            LoginTime = parts.ElementAtOrDefault(5) ?? string.Empty,
            Attach = parts.ElementAtOrDefault(6) ?? string.Empty,
            UnBind = parts.ElementAtOrDefault(7) ?? string.Empty,
            IsStop = parts.ElementAtOrDefault(8) ?? string.Empty,
            IsOnline = (parts.ElementAtOrDefault(9) ?? string.Empty).TrimEnd()
        };
    }
}