using System;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedSync.Data;
using MedSync.Factories;
using MedSync.Services;

namespace MedSync.ViewModels;

public partial class MainViewModel : ViewModelBase
{

#if DEBUG
    public MainViewModel()
    {
        _pageFactory = null!;
        _notificationPanelViewModel = null!;
    } // design-time only
#endif

    private readonly PageFactory _pageFactory;
    private readonly NotificationPanelViewModel _notificationPanelViewModel;

    [ObservableProperty] private bool _sidePanelExpanded = true;
    
    public NotificationPanelViewModel NotificationPanel => _notificationPanelViewModel;
    
    public int UnreadNotificationsCount => _notificationPanelViewModel?.UnreadCount ?? 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LogsPageIsActive))]
    [NotifyPropertyChangedFor(nameof(AppointmentsPageIsActive))]
    [NotifyPropertyChangedFor(nameof(PrescriptionsPageIsActive))]
    [NotifyPropertyChangedFor(nameof(TestsPageIsActive))]
    [NotifyPropertyChangedFor(nameof(PatientsPageIsActive))]
    [NotifyPropertyChangedFor(nameof(WorkersPageIsActive))]
    [NotifyPropertyChangedFor(nameof(SupportsPageIsActive))]
    private PageViewModel _currentPage = null!;

    public bool LogsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Logs;
    public bool AppointmentsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Appointments;
    public bool PrescriptionsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Prescriptions;
    public bool TestsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Tests;
    public bool PatientsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Patients;
    public bool WorkersPageIsActive => CurrentPage.PageName == ApplicationPageNames.Workers;
    public bool SupportsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Supports;

    public bool ToggleVisibility => SidePanelExpanded;

    public event Action? RequestExit;

    public MainViewModel(PageFactory pageFactory, NotificationPanelViewModel notificationPanelViewModel)
    {
        _pageFactory = pageFactory;
        _notificationPanelViewModel = notificationPanelViewModel;
        
        // Subscribe to unread count changes
        _notificationPanelViewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(NotificationPanelViewModel.UnreadCount))
            {
                OnPropertyChanged(nameof(UnreadNotificationsCount));
            }
        };
        
        GoToAppointments();
    }

    [RelayCommand]
    private void SidePanelResize() => SidePanelExpanded = !SidePanelExpanded;
    
    [RelayCommand]
    private void Exit() => RequestExit?.Invoke();

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
        : new GridLength(0, GridUnitType.Star);

    public CornerRadius NavbarCorner => SidePanelExpanded
        ? new CornerRadius(0, 8, 8, 8)
        : new CornerRadius(0, 0, 8, 8);

    public double ColumnSpacing => SidePanelExpanded ? 15 : 0;

    partial void OnSidePanelExpandedChanged(bool value)
    {
        OnPropertyChanged(nameof(SidePanelWidth));
        OnPropertyChanged(nameof(NavbarCorner));
        OnPropertyChanged(nameof(ToggleVisibility));
        OnPropertyChanged(nameof(ColumnSpacing));
    }
}
