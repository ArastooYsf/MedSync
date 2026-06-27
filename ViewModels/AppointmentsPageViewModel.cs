using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedSync.Data;
using MedSync.Helpers;
using MedSync.Models;
using MedSync.Services;

namespace MedSync.ViewModels;

public partial class AppointmentsPageViewModel : PageViewModel
{
    private readonly AppointmentService _appointmentService;
    private readonly PatientService _patientService;
    private readonly Timer _clockTimer;
    private int _tickCounter;

    private const int PageSize = 20;

    [ObservableProperty] private ObservableCollection<AppointmentCardViewModel> _appointments = [];
    [ObservableProperty] private ObservableCollection<AppointmentCardViewModel> _filteredAppointments = [];
    [ObservableProperty] private AppointmentCardViewModel? _selectedAppointment;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private string _currentTime = string.Empty;
    [ObservableProperty] private string _selectedStatusFilter = "همه";
    [ObservableProperty] private DateTime? _selectedDate = DateTime.Today;
    [ObservableProperty] private int _currentPage = 1;
    [ObservableProperty] private int _totalPages = 1;
    [ObservableProperty] private bool _hasMoreItems = false;

    public string[] StatusFilters { get; } = ["همه", "عادی", "اورژانسی"];

    public event EventHandler? AddAppointmentRequested;
    public event EventHandler<AppointmentCardViewModel>? EditAppointmentRequested;


    public AppointmentsPageViewModel(
        AppointmentService appointmentService,
        PatientService patientService)
    {
        PageName = ApplicationPageNames.Appointments;

        _appointmentService = appointmentService;
        _patientService = patientService;

        _clockTimer = new Timer(1000);
        _clockTimer.Elapsed += OnTimerTick;
        _clockTimer.Start();

        _ = LoadAppointmentsAsync();
    }

    private void OnTimerTick(object? sender, ElapsedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            _tickCounter++;
            CurrentTime = PersianCalendarHelper.FormatPersian(DateTime.Now, "yyyy/MM/dd   HH:mm:ss");

            var now = DateTime.Now;
            foreach (var card in Appointments)
            {
                var diff = Math.Abs((card.AppointmentDateTime - now).TotalHours);
                if (diff <= 2)
                    card.Tick();
                else if (_tickCounter % 10 == 0)
                    card.Tick();
            }
        });
    }

    partial void OnSearchTextChanged(string value) => ResetPagination();
    partial void OnSelectedStatusFilterChanged(string value) => ResetPagination();
    partial void OnSelectedDateChanged(DateTime? value) => ResetPagination();

    private void ResetPagination()
    {
        CurrentPage = 1;
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var result = Appointments.AsEnumerable();

        if (SelectedDate.HasValue)
            result = result.Where(a =>
                a.AppointmentDateTime.Date == SelectedDate.Value.Date);

        if (!string.IsNullOrWhiteSpace(SearchText))
            result = result.Where(a =>
                a.PatientFullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                a.PatientPhoneNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                a.PatientNationalCode.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        if (SelectedStatusFilter != "همه")
        {
            result = SelectedStatusFilter switch
            {
                "عادی" => result.Where(a => a.Status == AppointmentStatus.Normal),
                "اورژانسی" => result.Where(a => a.Status == AppointmentStatus.Emergency),
                _ => result
            };
        }

        var list = result.ToList();
        TotalPages = Math.Max(1, (int)Math.Ceiling(list.Count / (double)PageSize));
        CurrentPage = Math.Min(CurrentPage, TotalPages);
        HasMoreItems = CurrentPage < TotalPages;

        var paged = list.Take(CurrentPage * PageSize);
        FilteredAppointments = new ObservableCollection<AppointmentCardViewModel>(paged);
    }

    [RelayCommand]
    private void LoadMoreItems()
    {
        if (!HasMoreItems) return;
        CurrentPage++;
        ApplyFilters();
    }

    private async Task LoadAppointmentsAsync()
    {
        IsLoading = true;

        var data = await _appointmentService.GetAllAppointmentsAsync();

        Appointments = new ObservableCollection<AppointmentCardViewModel>(
            data.Select(a => new AppointmentCardViewModel(a)));

        ResetPagination();

        IsLoading = false;
    }

    [RelayCommand]
    private async Task LoadAppointments() => await LoadAppointmentsAsync();

    [RelayCommand]
    private async Task AddAppointment()
    {
        await Task.CompletedTask;
        AddAppointmentRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task EditAppointment(AppointmentCardViewModel? vm)
    {
        if (vm is null) return;
        await Task.CompletedTask;
        EditAppointmentRequested?.Invoke(this, vm);
    }


    [RelayCommand]
    private async Task DeleteAppointment(AppointmentCardViewModel? vm)
    {
        if (vm is null) return;

        IsLoading = true;
        await _appointmentService.DeleteAppointmentAsync(vm.Id);
        await LoadAppointmentsAsync();
        IsLoading = false;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _clockTimer.Stop();
            _clockTimer.Elapsed -= OnTimerTick;
            _clockTimer.Dispose();
        }
    }
}
