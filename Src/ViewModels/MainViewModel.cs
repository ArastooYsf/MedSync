using CommunityToolkit.Mvvm.ComponentModel;

namespace MedSync.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor()]
    private bool _sideMenuExpanded = true;
    
    public  
}