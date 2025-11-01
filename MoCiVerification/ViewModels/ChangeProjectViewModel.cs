using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MoCiVerification.Message;
using MoCiVerification.Models;

namespace MoCiVerification.ViewModels;

public partial class ChangeProjectViewModel :ObservableObject
{
    public event Action? RequestClose;

    [ObservableProperty] private bool _isChanging;
    [ObservableProperty] private string _projectAnnounce;
    private readonly IAdminService _adminService;
    private readonly ClientSettings _clientSettings;

    public ChangeProjectViewModel(IAdminService adminService,ClientSettings clientSettings)
    {
        _adminService = adminService;
        _clientSettings = clientSettings;
        WeakReferenceMessenger.Default.Register<LoadProjectAnnouncementMessage>(this, (recipient, message) =>
        {
            ProjectAnnounce = _clientSettings.CurrentProjectAnnouncement;
        });
    }

    [RelayCommand]
    public async Task ChangeProject()
    {
        IsChanging = true;
        var r = await _adminService.ChangeProjectAnnouncementAsync(_clientSettings.CurrentProjectName, ProjectAnnounce);
        if (r)
        {
            RequestClose?.Invoke();
        }

        IsChanging = false;
        
    }

}