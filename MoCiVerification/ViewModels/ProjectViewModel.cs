using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Material.Icons;
using Microsoft.Extensions.DependencyInjection;
using MoCiVerification.Features;
using MoCiVerification.Message;
using MoCiVerification.Models;
using MoCiVerification.Services;
using MoCiVerification.Views.Windows;
using SukiUI.Dialogs;
using SukiUI.Toasts;

namespace MoCiVerification.ViewModels;

public partial class ProjectViewModel:PageBase
{
    [ObservableProperty] private DataGridCollectionView _dataGridContent;
    [ObservableProperty] private ProjectDataGridContentViewModel _selectedItem;
    [ObservableProperty] private bool _isLoading = false;
    private readonly IAdminService _adminService;
    private readonly ClientSettings _settings;
    private readonly ISukiDialogManager _dialogManager;
    private readonly IShowWindowManager _showWindowManager;
    private readonly ISukiToastManager _toastManager;

    public ProjectViewModel(ISukiDialogManager dialogManager,ISukiToastManager toastManager,IShowWindowManager iShowWindowManager,IAdminService adminservice, ClientSettings settings ) : base(
        "项目管理", MaterialIconKind.ListBox, 1)
    {
        _adminService = adminservice;
        _dialogManager = dialogManager;  
        _settings = settings;
        _showWindowManager = iShowWindowManager;
        _toastManager = toastManager;
    }
    private async Task LoadProjectsAsync()
    {
        IsLoading = true;
        var lines = await _adminService.GetProjectListAsync();
        if (lines != null)
        {
            DataGridContent = new DataGridCollectionView(await Task.Run(() =>
            {
                return lines
                    .AsParallel() 
                    .AsOrdered()  
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Select(ProjectDataGridContentViewModel.FromRawLine)
                    .ToArray(); 
            }));
        }
        IsLoading = false;
    }

    [RelayCommand]
    public async Task ReflashProjects()
    {
        await LoadProjectsAsync();
    }

    public override async Task OnPageLoadedAsync()
    {
         await LoadProjectsAsync();
    }

    public override async Task OnPageDataChangedAsync()
    {
        _settings.CurrentProjectName = SelectedItem.ProjectName;

    }


    [RelayCommand]
    public void CreateNewProject()
    {
        _dialogManager.CreateDialog()
            .OfType(NotificationType.Information)
            .WithTitle("免责声明")
            .WithContent("陌辞网络验证仅用于提供程序API数据访问，请勿用于开发非法程序！\n包括但不限于 辅助,外挂,色情影视,DDOS,等违法行为。点击继续创建项目表明你已理解并且不会用于非法程序开发使用.")
            .WithActionButton("继续创建", _ => CreateProjectCommand(true), true)
            .WithActionButton("取消", _ => CreateProjectCommand(false), true)
            .Dismiss().ByClickingBackground()
            .TryShow();
    }
    [RelayCommand]
    public async Task ChangeProject()
    {
        _settings.CurrentProjectAnnouncement = SelectedItem.Announcement;
        await _showWindowManager.ShowDialogAsync<ChangeProjectView,ChangeProjectViewModel>();
        _toastManager.CreateSimpleInfoToast()
            .WithTitle("项目发生变化")
            .WithContent("改变项目属性成功！请耐心等待并刷新（有缓存）")
            .OfType(NotificationType.Success)
            .Queue();
    }

    private async Task CreateProjectCommand(bool r)
    {
        if (r)
        {
            await _showWindowManager.ShowDialogAsync<AddProjectView, AddProjectViewModel>();
        }
    }

    [RelayCommand]
    public async Task StopProject()
    {
        var r = await _adminService.StopProjectAsync(SelectedItem.ProjectName);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("项目发生变化")
                .WithContent("停用项目成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
    }
    [RelayCommand]
    public async Task RecoverProject()
    {
       var r = await _adminService.RecoverProjectAsync(SelectedItem.ProjectName);
       if (r)
       {
           _toastManager.CreateSimpleInfoToast()
               .WithTitle("项目发生变化")
               .WithContent("恢复项目成功！请耐心等待并刷新（有缓存）")
               .OfType(NotificationType.Success)
               .Queue();
       }
    }

    [RelayCommand]
    public async Task DeleteProject()
    {
        var r = await _adminService.DeleteProjectAsync(SelectedItem.ProjectName);
        if (r)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("项目发生变化")
                .WithContent("删除项目成功！请耐心等待并刷新（有缓存）")
                .OfType(NotificationType.Success)
                .Queue();
        }
        
    }
    


}
public partial class ProjectDataGridContentViewModel : ObservableObject
{
    [ObservableProperty] private string _projectName = string.Empty;
    [ObservableProperty] private string _token = string.Empty;
    [ObservableProperty] private string _type = string.Empty;
    [ObservableProperty] private string _createdTime = string.Empty;
    [ObservableProperty] private string _announcement = string.Empty;
    [ObservableProperty] private string _state = string.Empty;
    [ObservableProperty] private string _key = string.Empty;

    public static ProjectDataGridContentViewModel FromRawLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return new ProjectDataGridContentViewModel();

        var parts = line.Split(new[] { "|||" }, StringSplitOptions.None);

        return new ProjectDataGridContentViewModel
        {
            ProjectName = parts.ElementAtOrDefault(0) ?? string.Empty,
            Token = parts.ElementAtOrDefault(1) ?? string.Empty,
            Type = parts.ElementAtOrDefault(2) ?? string.Empty,
            CreatedTime = parts.ElementAtOrDefault(3) ?? string.Empty,
            Announcement = parts.ElementAtOrDefault(4) ?? string.Empty,
            State = parts.ElementAtOrDefault(5) ?? string.Empty,
            Key = (parts.ElementAtOrDefault(6) ?? string.Empty).TrimEnd()
        };
    }
}
