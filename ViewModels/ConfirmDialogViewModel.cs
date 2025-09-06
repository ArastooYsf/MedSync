using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MedSync.ViewModels;

public partial class ConfirmDialogViewModel :DialogVewModel
{
    [ObservableProperty] private string _title = "Confirm";
    [ObservableProperty] private string _message = "Confirm";
    [ObservableProperty] private string _confirmText = "Confirm";
    [ObservableProperty] private string _cancelText = "Confirm";
    [ObservableProperty] private string _iconText = "Confirm";
    
    [ObservableProperty]
    private bool _confirmed;
    
    [RelayCommand]
    public void Confirm()
    {
        Confirmed = true;
        Close();
    }

    [RelayCommand]
    public void Cancel()
    {
        Confirmed = false;
        Close();
    }
}