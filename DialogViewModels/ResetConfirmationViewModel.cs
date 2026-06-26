using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using MedSync.ViewModels;

namespace MedSync.DialogViewModels;

public partial class ResetConfirmationViewModel : ViewModelBase
{
    public event Action? ConfirmReset;
    public event Action? CancelReset;

    [RelayCommand]
    private void Confirm()
    {
        ConfirmReset?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        CancelReset?.Invoke();
    }
}