using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedSync.Data;
using MedSync.Models;
using MedSync.Services;

public class AppointmentReminderService
{
    private readonly AppDbContext _context;
    private readonly NotificationService _notificationService;
    private Timer? _timer;

    public AppointmentReminderService(
        AppDbContext context,
        NotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public void Start()
    {
        _timer = new Timer(async _ => await CheckAppointments(),
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(30));
    }

    private async Task CheckAppointments()
    {
        var now = DateTime.Now;
        var reminderTime = now.AddMinutes(10);

        var appointments = await _context.Appointments
            .Include(a => a.Patient)
            .Where(a => a.AppointmentDateTime <= reminderTime &&
                        a.AppointmentDateTime > now)
            .ToListAsync();

        foreach (var appointment in appointments)
        {
            await _notificationService.CreateNotificationAsync(
                NotificationType.AppointmentReminder,
                "یادآوری نوبت",
                $"تا 10 دقیقه دیگر نوبت {appointment.Patient.FullName} است.",
                appointment.Id,
                "reminder.wav"
            );
        }

        var missed = await _context.Appointments
            .Include(a => a.Patient)
            .Where(a => a.AppointmentDateTime < now)
            .ToListAsync();

        foreach (var appointment in missed)
        {
            await _notificationService.CreateNotificationAsync(
                NotificationType.AppointmentMissed,
                "نوبت گذشته",
                $"نوبت {appointment.Patient.FullName} گذشته است.",
                appointment.Id,
                "missed.wav"
            );
        }
    }
}
