using CommunityToolkit.Mvvm.ComponentModel;

namespace MoCiVerification.Features;

public abstract partial class LoginPage(string Tag) : ObservableValidator
{
    [ObservableProperty] private string tag = Tag;
}