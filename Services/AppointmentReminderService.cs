using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedSync.Data;
using MedSync.Models;
using Microsoft.EntityFrameworkCore;

namespace MedSync.Services
{
    public class AppointmentReminderService
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer? _timer;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); // Check every 1 minute
        private readonly TimeSpan _reminderBefore = TimeSpan.FromMinutes(10); // Remind 10 minutes before

        public AppointmentReminderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Start()
        {
            Console.WriteLine("[AppointmentReminderService] Started");
            _timer = new Timer(CheckAppointments, null, TimeSpan.Zero, _checkInterval);
        }

        public void Stop()
        {
            Console.WriteLine("[AppointmentReminderService] Stopped");
            _timer?.Dispose();
        }

        private async void CheckAppointments(object? state)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();

                var now = DateTime.Now;
                var reminderTime = now.Add(_reminderBefore);

                // ✅ 1. Check for upcoming appointments (10 minutes before)
                var upcomingAppointments = await context.Appointments
                    .Include(a => a.Patient)
                    .Where(a => a.AppointmentDate > now 
                             && a.AppointmentDate <= reminderTime 
                             && !a.ReminderSent)
                    .ToListAsync();

                foreach (var appointment in upcomingAppointments)
                {
                    await notificationService.NotifyAppointmentReminderAsync(
                        appointment.Id,
                        appointment.Patient?.FullName ?? "نامشخص",
                        appointment.AppointmentDate
                    );

                    appointment.ReminderSent = true;
                }

                // ✅ 2. Check for missed appointments (15 minutes after appointment time)
                var missedThreshold = now.AddMinutes(-15);
                var missedAppointments = await context.Appointments
                    .Include(a => a.Patient)
                    .Where(a => a.AppointmentDate < missedThreshold 
                             && a.AppointmentDate > now.AddHours(-24) // Only check last 24 hours
                             && !a.ReminderSent) // Hasn't been processed yet
                    .ToListAsync();

                foreach (var appointment in missedAppointments)
                {
                    await notificationService.NotifyAppointmentMissedAsync(
                        appointment.Id,
                        appointment.Patient?.FullName ?? "نامشخص"
                    );

                    appointment.ReminderSent = true; // Mark as processed
                }

                if (upcomingAppointments.Any() || missedAppointments.Any())
                {
                    await context.SaveChangesAsync();
                    Console.WriteLine($"[AppointmentReminderService] Processed {upcomingAppointments.Count} reminders, {missedAppointments.Count} missed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AppointmentReminderService] Error: {ex.Message}");
            }
        }
    }
}
