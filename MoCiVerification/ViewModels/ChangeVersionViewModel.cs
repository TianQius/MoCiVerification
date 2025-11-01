using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MoCiVerification.Message;
using MoCiVerification.Models;

namespace MoCiVerification.ViewModels;

public partial class ChangeVersionViewModel:ObservableObject
{
    public event Action? RequestClose;
    [ObservableProperty] private bool _isChanging;
    [ObservableProperty] private string _versionAnnounce;
    [ObservableProperty] private string _versionData;
    
    private readonly IAdminService _adminService;
    private readonly ClientSettings _clientSettings;

    public ChangeVersionViewModel(IAdminService adminService,ClientSettings clientSettings)
    {
        _adminService = adminService;
        _clientSettings = clientSettings;
        WeakReferenceMessenger.Default.Register<LoadVersionMessage>(this, (recipient, message) =>
        {
            VersionAnnounce = _clientSettings.CurrentVersionAnnouncement;
            VersionData = _clientSettings.CurrentVersionData;
        });
        
    }
    [RelayCommand]
    public async Task ChangeVersion()
    {
        IsChanging = true;
        var r = await _adminService.ChangeVersionAnnouncement(_clientSettings.CurrentProjectName, _clientSettings.CurrentVersion,VersionAnnounce);
        var r2 = await _adminService.ChangeVersionData(_clientSettings.CurrentProjectName, _clientSettings.CurrentVersion,VersionData);
        if (r && r2)
        {
            RequestClose?.Invoke();
        }

        IsChanging = false;
        
    }
    
    
    
    
}