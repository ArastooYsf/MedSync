using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using MedSync.ViewModels;

namespace MedSync.DialogViewModels;

public partial class ExitConfirmationViewModel : ViewModelBase
{
    public event Action? ConfirmExit;
    public event Action? CancelExit;

    [RelayCommand]
    private void Confirm()
    {
        ConfirmExit?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        CancelExit?.Invoke();
    }
}