
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls.Notifications;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MoCiVerification.Features;
using MoCiVerification.Message;
using MoCiVerification.Models;
using MoCiVerification.Services;
using MoCiVerification.Utilities;
using SukiUI;
using SukiUI.Dialogs;
using SukiUI.Enums;
using SukiUI.Models;
using SukiUI.Toasts;



namespace MoCiVerification.ViewModels;

public partial class MainMoCiViewModel : ViewModelBase
{
    public AvaloniaList<PageBase> Pages { get; set; }
    public PageNavigationService PageNavigationService { get; }
    [ObservableProperty] private ThemeVariant _baseTheme;
    [ObservableProperty] private string _bindCount;
    public IAvaloniaReadOnlyList<SukiColorTheme> Themes { get; }
    public ISukiToastManager ToastManager { get; }
    public IAvaloniaReadOnlyList<SukiBackgroundStyle> BackgroundStyles { get; }
    public ISukiDialogManager DialogManager { get; }
    [ObservableProperty] private bool _titleBarVisible = true;
    [ObservableProperty] private bool _useCloudVar;
    [ObservableProperty] private PageBase? _activePage;
    [ObservableProperty] private SukiBackgroundStyle _backgroundStyle = SukiBackgroundStyle.GradientSoft;
    private readonly SukiTheme _theme;
    private readonly SettingViewModel _settingTheme;
    private readonly ClientSettings _settings;
    private readonly IAdminService _adminService;

    public MainMoCiViewModel(IEnumerable<PageBase> pages, PageNavigationService pageNavigationService, ISukiToastManager toastManager,
        ISukiDialogManager dialogManager,IAdminService adminservice, ClientSettings settings)
    {
        ToastManager = toastManager;
        DialogManager = dialogManager;
        _adminService = adminservice;
        _settings = settings;
        
        Pages = new AvaloniaList<PageBase>(pages .OrderBy(x => x.Index).ThenBy(x => x.DisplayName));
        PageNavigationService = pageNavigationService;
        
        _settingTheme = (SettingViewModel)Pages.First(x => x is SettingViewModel);
        _settingTheme.BackgroundStyleChanged += style => BackgroundStyle = style;
        
        BackgroundStyles = new AvaloniaList<SukiBackgroundStyle>(Enum.GetValues<SukiBackgroundStyle>());
        _theme = SukiTheme.GetInstance();
        pageNavigationService.NavigationRequested += pageType =>
        {
            var page = Pages.FirstOrDefault(x => x.GetType() == pageType);
            if (page is null || ActivePage?.GetType() == pageType) return;
            ActivePage = page;
        };
        Themes = _theme.ColorThemes;
        BaseTheme = _theme.ActiveBaseTheme;
        _theme.OnBaseThemeChanged += variant =>
        {
            BaseTheme = variant;
            ToastManager.CreateSimpleInfoToast()
                .WithTitle("主题已变化")
                .WithContent($"主题方案已更改为 {variant}.")
                .Queue();
        };
        _theme.OnColorThemeChanged += theme => ToastManager.CreateSimpleInfoToast()
            .WithTitle("配色已变化")
            .WithContent($"配色方案已更改为 {theme.DisplayName}.")
            .Queue();
    }
   
    [RelayCommand]
    public async Task ChangeBindTimes()
    {
        var r = await _adminService.ChangeBindTimes(_settings.CurrentProjectName, BindCount);
        if (r)
        {
            ToastManager.CreateSimpleInfoToast()
                .WithTitle("解绑上限已变化")
                .WithContent($"用户解绑次数上线已更改为 {BindCount}.")
                .OfType(NotificationType.Success)
                .Queue();
        }
    }
    [RelayCommand]
    public async Task GetOptions()
    {
        UseCloudVar = await _adminService.GetVarOption(_settings.CurrentProjectName);
    }
    [RelayCommand]
    public async Task SetVarOption()
    {
        var r = await _adminService.SetVarOption(_settings.CurrentProjectName, UseCloudVar);
        if (r)
        {
            ToastManager.CreateSimpleInfoToast()
                .WithTitle("云变量设置")
                .WithContent($"云变量设置已更改为： {(UseCloudVar?"启用":"关闭")}.")
                .OfType(NotificationType.Success)
                .Queue();
        }
    }
    
    [RelayCommand]
    private static void OpenUrl(string url) => UrlUtilities.OpenUrl(url);
    partial void OnBackgroundStyleChanged(SukiBackgroundStyle value) =>
        _settingTheme.BackgroundStyle = value;
    
    
}