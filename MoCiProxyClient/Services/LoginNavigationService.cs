using System;
using MoCiProxyClient.Features;

namespace MoCiVerification.Services;

public class LoginNavigationService
{
    public Action<Type>? NavigationRequested { get; set; }
    
    public void RequestNavigation<T>() where T : LoginPage
    {
        NavigationRequested?.Invoke(typeof(T));
    }
}