using System;

namespace MedSync.Models;

public class Notification
{
    public int Id { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsRead { get; set; } = false;

    // ارتباط با نوبت (اختیاری - برای نوتیف‌های مرتبط با نوبت)
    public int? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    // نام فایل صوتی (اختیاری)
    public string? SoundFileName { get; set; }
}