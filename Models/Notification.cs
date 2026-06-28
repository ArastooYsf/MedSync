using System;

namespace MedSync.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now; // ✅ Changed from UtcNow
        public bool IsRead { get; set; } = false;
        public string? SoundFileName { get; set; }
        public int? RelatedAppointmentId { get; set; }
        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal; // ✅ New
        
        // Navigation
        public Appointment? RelatedAppointment { get; set; }
    }

    public enum NotificationPriority
    {
        Low = 0,
        Normal = 1,
        High = 2,
        Emergency = 3
    }
}