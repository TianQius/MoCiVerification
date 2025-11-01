using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MoCiVerification.Message;

namespace MoCiVerification.ViewModels;

public partial class FindViewModel:ViewModelBase
{
    
    public event Action ? RequestClose;
    [ObservableProperty] private bool _isFinding = false;
    [ObservableProperty] private string _keyword;

    public FindViewModel()
    {
        
    }






    [RelayCommand]
    public void FindData()
    {
        WeakReferenceMessenger.Default.Send(new FindRequestMessage(Keyword));
    }
    
}