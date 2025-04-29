using Avalonia.Svg.Skia;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MedSync.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SideMenuImage))]
    private bool _sideMenuExpanded = false;

    public SvgImage SideMenuImage => new SvgImage { Source = SvgSource.Load($"avaress://{nameof(MedSync)}/Src/Assets/Images/{(SideMenuExpanded ? "Banner" : "Logo")}.svg") };
}