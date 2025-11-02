using System;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoCiVerification.Models;
using SukiUI.Toasts;

namespace MoCiVerification.ViewModels;

public partial class AddProjectViewModel:ObservableObject
{
    public event Action? RequestClose;
    [ObservableProperty] private bool _isAddingProject = false;
    [ObservableProperty]
    private string _projectName;
    [ObservableProperty]
    private int _projectType = 0;
    [ObservableProperty]
    private string _projectAnnounce;
    private readonly IAdminService _adminService;
    private readonly ISukiToastManager _toastManager;
    
    private readonly ClientSettings _clientSettings;

    public AddProjectViewModel(ISukiToastManager toastManager,IAdminService adminService, ClientSettings clientSettings)
    {
        _adminService = adminService;
        _toastManager = toastManager;
        _clientSettings = clientSettings;
    }

    [RelayCommand]
    public async Task AddProject()
    {
        IsAddingProject = true;
        if (String.IsNullOrEmpty(ProjectAnnounce)) ProjectAnnounce = "无";
        else if (String.IsNullOrEmpty(ProjectName))
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("创建项目失败")
                .WithContent("项目名称不可为空！！！")
                .OfType(NotificationType.Error)
                .Queue();
            RequestClose?.Invoke();
        }
        else
        {
            var r = await _adminService.AddProjectAsync(ProjectName, ProjectType == 0 ? "单码项目" : "用户项目", ProjectAnnounce, "moci");
            if (r)
            {
                _toastManager.CreateSimpleInfoToast()
                    .WithTitle("项目发生变化")
                    .WithContent("创建项目成功！请耐心等待并刷新（有缓存）")
                    .OfType(NotificationType.Success)
                    .Queue();
                RequestClose?.Invoke();
            }
            else
            {
                _toastManager.CreateSimpleInfoToast()
                    .WithTitle("创建项目失败")
                    .WithContent(_clientSettings.GlobalMessage)
                    .OfType(NotificationType.Error)
                    .Queue();
            }
        }
        IsAddingProject = false;
        
    }
    
    
}