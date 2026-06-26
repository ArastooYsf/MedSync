using System;
using CommunityToolkit.Mvvm.ComponentModel;
using MedSync.Helpers;
using MedSync.Models;

namespace MedSync.ViewModels;

public partial class AppointmentCardViewModel : ObservableObject
{
    private readonly Appointment _appointment;

    public AppointmentCardViewModel(Appointment appointment)
    {
        _appointment = appointment;
        Tick();
    }

    public int Id => _appointment.Id;

    public string PatientFullName => _appointment.Patient.FullName;
    public string PatientPhoneNumber => _appointment.Patient.PhoneNumber;
    public string PatientNationalCode => _appointment.Patient.NationalCode;

    public DateTime AppointmentDateTime => _appointment.AppointmentDateTime;
    public AppointmentStatus Status => _appointment.Status;
    public string? Notes => _appointment.Notes;

    public string TimeDisplay => AppointmentDateTime.ToString("HH:mm");
    public string DateDisplay => PersianCalendarHelper.FormatPersian(AppointmentDateTime);

    [ObservableProperty] private bool _isPast;
    [ObservableProperty] private bool _isNow;
    [ObservableProperty] private bool _isUpcoming;
    [ObservableProperty] private string _countdown = string.Empty;
    [ObservableProperty] private string _countdownLabel = string.Empty;

    public string StatusText => Status switch
    {
        AppointmentStatus.Normal => "عادی",
        AppointmentStatus.Emergency => "اورژانسی", 
        _ => "نامشخص"
    };

    public string StatusIcon => Status switch
    {
        AppointmentStatus.Normal => "\ue184",
        AppointmentStatus.Emergency => "\ue4e2",
        _ => "\ue184"
    };

    public bool IsNormalStatus => Status == AppointmentStatus.Normal;
    public bool IsEmergencyStatus => Status == AppointmentStatus.Emergency;
    public bool IsSpecialStatus => Status == AppointmentStatus.Special;

    public void Tick()
    {
        var now = DateTime.Now;
        var diff = AppointmentDateTime - now;
        var today = now.Date;
        var cardDate = AppointmentDateTime.Date;
        var isToday = cardDate == today;

        IsPast = AppointmentDateTime < now.AddMinutes(-1);

        IsNow = isToday && diff.TotalMinutes is >= -1 and <= 10;

        IsUpcoming = AppointmentDateTime > now.AddMinutes(10);

        if (IsPast)
        {
            var elapsed = now - AppointmentDateTime;
            Countdown = FormatTimeSpan(elapsed);
            CountdownLabel = "گذشت";
        }
        else if (IsNow)
        {
            Countdown = "الان";
            CountdownLabel = "نوبت فعلی";
        }
        else if (isToday)
        {
            Countdown = FormatTimeSpan(diff);
            CountdownLabel = "تا نوبت";
        }
        else
        {
            var daysRemaining = (int)(cardDate - today).TotalDays;
            Countdown = $"{daysRemaining} روز";
            CountdownLabel = "تا نوبت";
        }
    }

    private static string FormatTimeSpan(TimeSpan ts)
    {
        var t = ts.Duration();
        if (t.TotalHours >= 1)
            return $"{(int)t.TotalHours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";

        return $"{t.Minutes:D2}:{t.Seconds:D2}";
    }
}