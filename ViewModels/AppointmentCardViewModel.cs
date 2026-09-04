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
    public bool IsCompleted => _appointment.IsCompleted;

    public string TimeDisplay => AppointmentDateTime.ToString("HH:mm");
    public string DateDisplay => PersianCalendarHelper.FormatPersian(AppointmentDateTime);

    [ObservableProperty] private bool _isPast;
    [ObservableProperty] private bool _isNow;
    [ObservableProperty] private bool _isUpcoming;
    [ObservableProperty] private bool _isOverdue; // منقضی شده (بیشتر از 10 دقیقه گذشته)
    [ObservableProperty] private string _countdown = string.Empty;
    [ObservableProperty] private string _countdownLabel = string.Empty;
    [ObservableProperty] private bool _showCompleteButton = false;

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

        // اگر ویزیت تمام شده، دیگه تایمر نمیزنیم
        if (IsCompleted)
        {
            IsPast = true;
            IsNow = false;
            IsUpcoming = false;
            IsOverdue = false;
            ShowCompleteButton = false;
            Countdown = "تمام شده";
            CountdownLabel = "";
            return;
        }

        // محاسبه وضعیت نوبت
        var minutesFromStart = (now - AppointmentDateTime).TotalMinutes;

        IsPast = minutesFromStart > 10; // بیشتر از 10 دقیقه گذشته
        IsNow = minutesFromStart >= 0 && minutesFromStart <= 10; // در بازه 10 دقیقه ویزیت
        IsUpcoming = minutesFromStart < 0; // هنوز نرسیده
        IsOverdue = minutesFromStart > 10 && !IsCompleted; // منقضی شده و تکمیل نشده

        // نمایش دکمه اتمام فقط برای نوبت جاری
        ShowCompleteButton = IsNow;

        if (IsNow)
        {
            // تایمر 10 دقیقه‌ای برای نوبت جاری
            var remaining = TimeSpan.FromMinutes(10) - TimeSpan.FromMinutes(minutesFromStart);
            if (remaining.TotalSeconds > 0)
            {
                Countdown = FormatTimeSpan(remaining);
                CountdownLabel = "زمان باقی‌مانده";
            }
            else
            {
                Countdown = "زمان تمام شد";
                CountdownLabel = "";
            }
        }
        else if (IsOverdue)
        {
            // نوبت منقضی: نمایش زمان گذشته بدون تایمر
            var elapsed = TimeSpan.FromMinutes(minutesFromStart - 10);
            Countdown = FormatTimeSpan(elapsed) + " گذشت";
            CountdownLabel = "منقضی شده";
        }
        else if (IsUpcoming)
        {
            // نوبت آینده
            if (isToday)
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
        else
        {
            // وضعیت پیش‌فرض
            Countdown = "";
            CountdownLabel = "";
        }
    }

    private static string FormatTimeSpan(TimeSpan ts)
    {
        var t = ts.Duration();
        if (t.TotalHours >= 1)
            return $"{(int)t.TotalHours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";

        return $"{t.Minutes:D2}:{t.Seconds:D2}";
    }

    public void MarkAsCompleted()
    {
        _appointment.IsCompleted = true;
        OnPropertyChanged(nameof(IsCompleted));
        Tick();
    }
}
