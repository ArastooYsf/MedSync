using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedSync.Data;
using MedSync.Models;

namespace MedSync.Services;

public class NotificationService
{
    private readonly AppDbContext _context;

    public NotificationService(AppDbContext context)
    {
        _context = context;
    }

    // ایجاد نوتیفیکیشن جدید
    public async Task<Notification> CreateNotificationAsync(
        NotificationType type,
        string title,
        string message,
        int? appointmentId = null,
        string? soundFileName = null)
    {
        var notification = new Notification
        {
            Type = type,
            Title = title,
            Message = message,
            AppointmentId = appointmentId,
            SoundFileName = soundFileName,
            CreatedAt = DateTime.Now,
            IsRead = false
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        return notification;
    }

    // دریافت همه نوتیفیکیشن‌ها
    public async Task<List<Notification>> GetAllNotificationsAsync()
    {
        return await _context.Notifications
            .Include(n => n.Appointment!)
            .ThenInclude(a => a!.Patient)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    // دریافت نوتیفیکیشن‌های خوانده نشده
    // دریافت نوتیفیکیشن‌های خوانده نشده
    public async Task<List<Notification>> GetUnreadNotificationsAsync()
    {
        return await _context.Notifications
            .Where(n => !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    // علامت خوانده شده
    public async Task MarkAsReadAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification == null) return;

        notification.IsRead = true;
        await _context.SaveChangesAsync();
    }

    // حذف نوتیفیکیشن
    public async Task DeleteAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification == null) return;

        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();
    }
}

    