using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedSync.Data;
using MedSync.Models;

namespace MedSync.Services;

public class AppointmentService(AppDbContext context, NotificationService notificationService)
{
    private readonly AppDbContext _context = context;
    private readonly NotificationService _notificationService = notificationService;

    public async Task<List<Appointment>> GetAllAppointmentsAsync()
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .OrderBy(a => a.AppointmentDateTime)
            .ToListAsync();
    }

    public async Task<Appointment?> GetAppointmentByIdAsync(int id)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task AddAppointmentAsync(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        // بارگذاری Patient برای نوتیفیکیشن
        await _context.Entry(appointment).Reference(a => a.Patient).LoadAsync();

        // نوتیفیکیشن ایجاد نوبت
        await _notificationService.CreateNotificationAsync(
            NotificationType.AppointmentCreated,
            "نوبت جدید ثبت شد",
            $"برای بیمار {appointment.Patient?.FullName} نوبت ثبت شد.",
            appointment.Id,
            "appointment_created.wav"
        );

        // اگر اورژانسی بود
        if (appointment.Status == AppointmentStatus.Emergency)
        {
            await _notificationService.CreateNotificationAsync(
                NotificationType.AppointmentEmergency,
                "نوبت اورژانسی",
                $"نوبت اورژانسی برای {appointment.Patient?.FullName} ثبت شد.",
                appointment.Id,
                "emergency.wav"
            );
        }
    }

    public async Task UpdateAppointmentAsync(Appointment appointment)
    {
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();

        // بارگذاری Patient
        await _context.Entry(appointment).Reference(a => a.Patient).LoadAsync();

        // نوتیفیکیشن ویرایش
        await _notificationService.CreateNotificationAsync(
            NotificationType.AppointmentUpdated,
            "ویرایش نوبت",
            $"نوبت بیمار {appointment.Patient?.FullName} ویرایش شد.",
            appointment.Id,
            "appointment_updated.wav"
        );
    }

    public async Task DeleteAppointmentAsync(int id)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment is null) return;

        // نوتیفیکیشن حذف
        await _notificationService.CreateNotificationAsync(
            NotificationType.AppointmentDeleted,
            "حذف نوبت",
            $"نوبت بیمار {appointment.Patient?.FullName} حذف شد.",
            null, // AppointmentId null چون حذف شده
            "appointment_deleted.wav"
        );

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
    }
}
