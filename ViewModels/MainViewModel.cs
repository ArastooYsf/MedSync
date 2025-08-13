using Avalonia.Svg.Skia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MedSync.ViewModels;

public partial class MainViewModel :ViewModelBase
{
    [ObservableProperty]
    private bool _sidePanelExpanded = false;

    [RelayCommand]
    private void SidePanelResize()
    {
        SidePanelExpanded = !SidePanelExpanded;
    }
    
}