using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExCSS;
using MedSync.Data;
using MedSync.Factories;
using MedSync.Views;

namespace MedSync.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private const string buttonActiveClass = "active";
    
    private PageFactory _pageFactory;
    
    [ObservableProperty]
    private bool _sidePanelExpanded = true;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LogsPageIsActive))]
    [NotifyPropertyChangedFor(nameof(AppointmentsPageIsActive))]
    [NotifyPropertyChangedFor(nameof(PrescriptionsPageIsActive))]
    [NotifyPropertyChangedFor(nameof(TestsPageIsActive))]
    [NotifyPropertyChangedFor(nameof(PatientsPageIsActive))]
    [NotifyPropertyChangedFor(nameof(WorkersPageIsActive))]
    [NotifyPropertyChangedFor(nameof(SupportsPageIsActive))]
    private PageViewModel _currentPage;
    
    public bool LogsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Logs;
    public bool AppointmentsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Appointments;
    public bool PrescriptionsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Prescriptions;
    public bool TestsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Tests;
    public bool PatientsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Patients;
    public bool WorkersPageIsActive => CurrentPage.PageName == ApplicationPageNames.Workers;
    public bool SupportsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Supports;

    public MainViewModel()
    {
        CurrentPage = new SettingsPageViewModel();
    }

    public MainViewModel(PageFactory pageFactory)
    {
        _pageFactory = pageFactory;
        
        GoToAppointments();
    }

    [RelayCommand]
    private void SidePanelResize()
    {
        SidePanelExpanded = !SidePanelExpanded;
    }

    [RelayCommand]
    private void GoToSettings() => CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Settings);
    
    [RelayCommand]
    private void GoToLogs() => CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Logs);
    
    [RelayCommand]
    private void GoToAppointments() => CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Appointments);
    
    [RelayCommand]
    private void GoToPrescriptions() => CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Prescriptions);
    
    [RelayCommand]
    private void GoToTests() => CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Tests);
    
    [RelayCommand]
    private void GoToPatients() => CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Patients);
    [RelayCommand]
    private void GoToWorkers() => CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Workers);
    [RelayCommand]
    private void GoToSupports() => CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Supports);
    
    public GridLength SidePanelWidth => SidePanelExpanded 
        ? new GridLength(1, GridUnitType.Star)
        : new GridLength(94.5, GridUnitType.Pixel);

    public Thickness LogoMargin => SidePanelExpanded
        ? new Thickness(45,-10,45,5)
        : new Thickness(0,-10,0,5);

    public Thickness ButtonPadding => SidePanelExpanded
        ? new Thickness(5, 0)
        : new Thickness(0);

    public HorizontalAlignment ButtonAlignment => SidePanelExpanded
        ? HorizontalAlignment.Right
        : HorizontalAlignment.Center;
    
    public int ButtonColumn => SidePanelExpanded ? 1 : 0;
    
    public double IconFontSize => SidePanelExpanded
        ? 21
        : 30;
    
    public string ToggleLabel => SidePanelExpanded 
        ? "\ue09e"
        : "\ue0a6";
    
    partial void OnSidePanelExpandedChanged(bool value)
    {
        OnPropertyChanged(nameof(SidePanelWidth));
        OnPropertyChanged(nameof(LogoMargin));
        OnPropertyChanged(nameof(ButtonPadding));
        OnPropertyChanged(nameof(IconFontSize));
        OnPropertyChanged(nameof(ButtonAlignment));
        OnPropertyChanged(nameof(ButtonColumn));
        OnPropertyChanged(nameof(ToggleLabel));
    }
    
}
