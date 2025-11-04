using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MoCiProxyClient.Features;

public abstract partial class LoginPage(string Tag) : ObservableValidator
{
    [ObservableProperty] private string tag = Tag;
    public virtual Task OnPageLoadedAsync() => Task.CompletedTask;

    public virtual Task OnPageDataChangedAsync() => Task.CompletedTask;
}