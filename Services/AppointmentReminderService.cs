using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedSync.Data;
using MedSync.Models;
using MedSync.Services;

public class AppointmentReminderService(
    AppDbContext context,
    NotificationService notificationService)
{
    private readonly AppDbContext _context = context;
    private readonly NotificationService _notificationService = notificationService;
    private Timer? _timer;

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
        var todayStart = now.Date;
        var todayEnd = todayStart.AddDays(1);

        // فقط نوبت‌هایی که تا ۱۰ دقیقه دیگه هستن و امروزن
        var upcoming = await _context.Appointments
            .Include(a => a.Patient)
            .Where(a => a.AppointmentDateTime <= reminderTime &&
                        a.AppointmentDateTime > now)
            .ToListAsync();

        foreach (var appointment in upcoming)
        {
            // چک کن قبلاً reminder ساخته شده یا نه
            var alreadyNotified = await _context.Notifications
                .AnyAsync(n => n.AppointmentId == appointment.Id &&
                               n.Type == NotificationType.AppointmentReminder &&
                               n.CreatedAt >= todayStart);

            if (!alreadyNotified)
            {
                await _notificationService.CreateNotificationAsync(
                    NotificationType.AppointmentReminder,
                    "یادآوری نوبت",
                    $"تا ۱۰ دقیقه دیگر نوبت {appointment.Patient.FullName} است.",
                    appointment.Id,
                    "reminder.wav"
                );
            }
        }

        // فقط نوبت‌های امروز که گذشتن
        var missed = await _context.Appointments
            .Include(a => a.Patient)
            .Where(a => a.AppointmentDateTime >= todayStart &&
                        a.AppointmentDateTime < now)
            .ToListAsync();

        foreach (var appointment in missed)
        {
            var alreadyNotified = await _context.Notifications
                .AnyAsync(n => n.AppointmentId == appointment.Id &&
                               n.Type == NotificationType.AppointmentMissed &&
                               n.CreatedAt >= todayStart);

            if (!alreadyNotified)
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
}