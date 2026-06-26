using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedSync.Models;
using MedSync.Services;

namespace MedSync.ViewModels;

public partial class AddAppointmentDialogViewModel : ViewModelBase
{
    private readonly AppointmentService _appointmentService;
    private readonly PatientService _patientService;
    
    private int? _appointmentId;

    [ObservableProperty] private List<Patient> _patients = new();
    [ObservableProperty] private Patient? _selectedPatient;
    [ObservableProperty] private string? _selectedPatientError;

    [ObservableProperty] private DateTime? _appointmentDate = DateTime.Today;
    [ObservableProperty] private string? _appointmentDateError;

    [ObservableProperty] private string _appointmentHour = DateTime.Now.Hour.ToString("D2");
    [ObservableProperty] private string _appointmentMinute = "00";
    [ObservableProperty] private string? _appointmentTimeError;

    [ObservableProperty] private string _selectedStatus = "Normal";

    [ObservableProperty] private string _notes = string.Empty;

    [ObservableProperty] private bool _isSaving;

    public string DialogTitle => _appointmentId.HasValue ? "ویرایش نوبت" : "افزودن نوبت";
    
    public string[] StatusDisplayOptions { get; } = { "عادی", "اورژانسی" };

    public string[] HourOptions { get; } =
        Enumerable.Range(0, 24).Select(h => h.ToString("D2")).ToArray();

    public string[] MinuteOptions { get; } =
        Enumerable.Range(0, 60).Select(m => m.ToString("D2")).ToArray();

    public Action<bool>? CloseDialog { get; set; }

    public AddAppointmentDialogViewModel(
        AppointmentService appointmentService,
        PatientService     patientService)
    {
        _appointmentService = appointmentService;
        _patientService     = patientService;
        _ = LoadPatientsAsync();
    }
    
    public AddAppointmentDialogViewModel(
        AppointmentService appointmentService,
        PatientService     patientService,
        Appointment        existing)
        : this(appointmentService, patientService)
    {
        _appointmentId = existing.Id;

        AppointmentDate = existing.AppointmentDateTime.Date;
            AppointmentHour   = existing.AppointmentDateTime.Hour.ToString("D2");
        AppointmentMinute = existing.AppointmentDateTime.Minute.ToString("D2");
        Notes             = existing.Notes ?? string.Empty;

        SelectedStatus = existing.Status switch
        {
            AppointmentStatus.Emergency => "اورژانسی",
            _                           => "عادی"
        };

        // بیمار بعد از لود لیست ست می‌شه
        _ = SetPatientAfterLoadAsync(existing.PatientId);
    }

    private async Task SetPatientAfterLoadAsync(int patientId)
    {
        await LoadPatientsAsync();
        SelectedPatient = Patients.FirstOrDefault(p => p.Id == patientId);
    }
    
    private async Task LoadPatientsAsync()
    {
        var list = await _patientService.GetAllPatientsAsync();
        Patients = list.ToList();
    }

    partial void OnSelectedPatientChanged(Patient? value)
    {
        if (value != null)
            SelectedPatientError = null;
    }

    partial void OnAppointmentDateChanged(DateTime? value)
    {
        if (value.HasValue)
            AppointmentDateError = null;
    }

    partial void OnAppointmentHourChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            AppointmentTimeError = null;
    }

    partial void OnAppointmentMinuteChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            AppointmentTimeError = null;
    }

    [RelayCommand]
    private async Task Save()
    {
        if (!Validate()) return;

        IsSaving = true;

        try
        {
            var date = AppointmentDate!.Value;
            var hour   = int.Parse(AppointmentHour);
            var minute = int.Parse(AppointmentMinute);
            var dt     = date.AddHours(hour).AddMinutes(minute);

            var status = SelectedStatus switch
            {
                "اورژانسی" => AppointmentStatus.Emergency,
                _          => AppointmentStatus.Normal
            };

            if (_appointmentId.HasValue)
            {
                // ✅ حالت ویرایش
                var existing = await _appointmentService.GetAppointmentByIdAsync(_appointmentId.Value);
                if (existing is not null)
                {
                    existing.PatientId           = SelectedPatient!.Id;
                    existing.AppointmentDateTime = dt;
                    existing.Status              = status;
                    existing.Notes               = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim();

                    await _appointmentService.UpdateAppointmentAsync(existing);
                }
            }
            else
            {
                // ✅ حالت افزودن
                var appointment = new Appointment
                {
                    PatientId           = SelectedPatient!.Id,
                    AppointmentDateTime = dt,
                    Status              = status,
                    Notes               = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim(),
                    CreatedAt           = DateTime.Now
                };

                await _appointmentService.AddAppointmentAsync(appointment);
            }

            CloseDialog?.Invoke(true);
        }
        catch
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseDialog?.Invoke(false);
    }

    private bool Validate()
    {
        var isValid = true;

        if (SelectedPatient == null)
        {
            SelectedPatientError = "انتخاب بیمار الزامی است";
            isValid = false;
        }

        if (!AppointmentDate.HasValue)
        {
            AppointmentDateError = "تاریخ نوبت الزامی است";
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(AppointmentHour) ||
            string.IsNullOrWhiteSpace(AppointmentMinute))
        {
            AppointmentTimeError = "ساعت نوبت الزامی است";
            isValid = false;
        }

        return isValid;
    }
    
}
