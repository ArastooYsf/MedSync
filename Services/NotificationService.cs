using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedSync.Data;
using MedSync.Models;
using Microsoft.EntityFrameworkCore;

namespace MedSync.Services
{
    public class NotificationService
    {
        private readonly AppDbContext _context;
        private readonly AudioService _audioService;
        private readonly SystemTrayService _systemTrayService;

        // Event برای UI (Toast نمایش بده)
        public event Action<Notification>? OnNotificationCreated;

        public NotificationService(
            AppDbContext context,
            AudioService audioService,
            SystemTrayService systemTrayService)
        {
            _context = context;
            _audioService = audioService;
            _systemTrayService = systemTrayService;
        }

        /// <summary>
        /// Create and show notification (Database + Toast + Audio + System Tray)
        /// </summary>
        public async Task<Notification> CreateNotificationAsync(
            string title,
            string message,
            NotificationType type,
            string? soundFileName = null,
            int? relatedAppointmentId = null,
            NotificationPriority priority = NotificationPriority.Normal)
        {
            // 1. Save to database
            var notification = new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                SoundFileName = soundFileName,
                RelatedAppointmentId = relatedAppointmentId,
                Priority = priority,
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // 2. Trigger UI Toast (via event)
            OnNotificationCreated?.Invoke(notification);

            // 3. Play sound (non-blocking)
            _ = _audioService.PlayNotificationSoundAsync(soundFileName);

            // 4. Show system tray notification (non-blocking)
            _ = _systemTrayService.ShowNotificationAsync(title, message);

            Console.WriteLine($"[NotificationService] Created: {type} - {title}");
            return notification;
        }

        // ✅ Quick helper methods for common notification types
        public async Task NotifyAppointmentCreatedAsync(int appointmentId, string patientName, DateTime appointmentDate)
        {
            await CreateNotificationAsync(
                title: "نوبت جدید ثبت شد",
                message: $"نوبت برای {patientName} در {appointmentDate:yyyy/MM/dd HH:mm} ثبت شد",
                type: NotificationType.AppointmentCreated,
                soundFileName: "appointment_created.wav",
                relatedAppointmentId: appointmentId,
                priority: NotificationPriority.Normal
            );
        }

        public async Task NotifyAppointmentUpdatedAsync(int appointmentId, string patientName)
        {
            await CreateNotificationAsync(
                title: "نوبت ویرایش شد",
                message: $"نوبت {patientName} با موفقیت ویرایش شد",
                type: NotificationType.AppointmentUpdated,
                soundFileName: "appointment_updated.wav",
                relatedAppointmentId: appointmentId,
                priority: NotificationPriority.Low
            );
        }

        public async Task NotifyAppointmentCancelledAsync(int appointmentId, string patientName)
        {
            await CreateNotificationAsync(
                title: "نوبت لغو شد",
                message: $"نوبت {patientName} لغو شد",
                type: NotificationType.AppointmentCancelled,
                soundFileName: "appointment_cancelled.wav",
                relatedAppointmentId: appointmentId,
                priority: NotificationPriority.Normal
            );
        }

        public async Task NotifyAppointmentReminderAsync(int appointmentId, string patientName, DateTime appointmentDate)
        {
            await CreateNotificationAsync(
                title: "یادآوری نوبت",
                message: $"نوبت {patientName} در {appointmentDate:HH:mm} (10 دقیقه دیگر)",
                type: NotificationType.AppointmentReminder,
                soundFileName: "reminder.wav",
                relatedAppointmentId: appointmentId,
                priority: NotificationPriority.High
            );
        }

        public async Task NotifyAppointmentMissedAsync(int appointmentId, string patientName)
        {
            await CreateNotificationAsync(
                title: "نوبت از دست رفت",
                message: $"نوبت {patientName} انجام نشد",
                type: NotificationType.AppointmentMissed,
                soundFileName: "missed.wav",
                relatedAppointmentId: appointmentId,
                priority: NotificationPriority.High
            );
        }

        public async Task NotifyEmergencyAppointmentAsync(int appointmentId, string patientName)
        {
            await CreateNotificationAsync(
                title: "🚨 نوبت اورژانسی",
                message: $"نوبت اورژانسی برای {patientName} ثبت شد!",
                type: NotificationType.AppointmentEmergency,
                soundFileName: "emergency.wav",
                relatedAppointmentId: appointmentId,
                priority: NotificationPriority.Emergency
            );
        }

        // ✅ Database queries
        public async Task<List<Notification>> GetAllNotificationsAsync()
        {
            return await _context.Notifications
                .Include(n => n.RelatedAppointment)
                    .ThenInclude(a => a!.Patient)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetUnreadNotificationsAsync()
        {
            return await _context.Notifications
                .Where(n => !n.IsRead)
                .Include(n => n.RelatedAppointment)
                    .ThenInclude(a => a!.Patient)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync()
        {
            return await _context.Notifications.CountAsync(n => !n.IsRead);
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync()
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearAllNotificationsAsync()
        {
            var allNotifications = await _context.Notifications.ToListAsync();
            _context.Notifications.RemoveRange(allNotifications);
            await _context.SaveChangesAsync();
        }
    }
}
