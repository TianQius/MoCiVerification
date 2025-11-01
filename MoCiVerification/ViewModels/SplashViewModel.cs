using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MoCiVerification.ViewModels;

public partial class SplashViewModel :  ViewModelBase//PageBase("欢迎使用", MaterialIconKind.Hand, 0)
{
    [ObservableProperty]
    private double opacity = 0;

    [ObservableProperty] 
    private bool isVisible = true;

    public event EventHandler? SplashCompleted;

    
    public SplashViewModel()
    {
        _ = InitializeAsync();
    }
    private async Task InitializeAsync()
    {
        await Task.Delay(50); 
            
        await FadeInAsync();
            
        await Task.Delay(2000);
            
        await FadeOutAsync();
            
        IsVisible = false;
        SplashCompleted?.Invoke(this, EventArgs.Empty);
    }

    private async Task FadeInAsync()
    {
        const int duration = 300;
        const int steps = 30;
            
        for (int i = 0; i <= steps; i++)
        {
            Opacity = (double)i / steps;
            await Task.Delay(duration / steps);
        }
        Opacity = 1.0;
    }

    private async Task FadeOutAsync()
    {
        const int duration = 300;
        const int steps = 30;
            
        for (int i = steps; i >= 0; i--)
        {
            Opacity = (double)i / steps;
            await Task.Delay(duration / steps);
        }
        Opacity = 0.0;
    }
}