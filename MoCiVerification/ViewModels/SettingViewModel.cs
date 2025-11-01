using System;
using Avalonia.Collections;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using MoCiVerification.Features;
using MoCiVerification.Models;
using MoCiVerification.Services;
using SukiUI;
using SukiUI.Enums;
using SukiUI.Models;

namespace MoCiVerification.ViewModels;

public partial class SettingViewModel:PageBase
{
    private readonly ClientSettings _settings;
    
    
    
    private readonly SukiTheme _theme = SukiTheme.GetInstance();
    public Action<SukiBackgroundStyle>? BackgroundStyleChanged { get; set; }
    
    
    
    
    public IAvaloniaReadOnlyList<SukiColorTheme> AvailableColors { get; }
    public IAvaloniaReadOnlyList<SukiBackgroundStyle> AvailableBackgroundStyles { get; }
    
    
    
    [ObservableProperty] private bool _isLightTheme;
    [ObservableProperty] private SukiBackgroundStyle _backgroundStyle ;
    
    public SettingViewModel(ClientSettings settings) : base(
        "设置", MaterialIconKind.Settings, 100)

    {
        _settings = settings;
        AvailableBackgroundStyles = new AvaloniaList<SukiBackgroundStyle>(Enum.GetValues<SukiBackgroundStyle>());
        AvailableColors = _theme.ColorThemes;
        IsLightTheme = _theme.ActiveBaseTheme == ThemeVariant.Light;
        _theme.OnBaseThemeChanged += variant =>
            IsLightTheme = variant == ThemeVariant.Light;
        
    }
    
    [RelayCommand]
    private void SwitchToColorTheme(SukiColorTheme colorTheme) =>
        _theme.ChangeColorTheme(colorTheme);
    partial void OnIsLightThemeChanged(bool value) =>
        _theme.ChangeBaseTheme(value ? ThemeVariant.Light : ThemeVariant.Dark);
    partial void OnBackgroundStyleChanged(SukiBackgroundStyle value) => 
        BackgroundStyleChanged?.Invoke(value);
}