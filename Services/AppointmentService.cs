using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedSync.Data;
using MedSync.Models;
using Microsoft.EntityFrameworkCore;

namespace MedSync.Services
{
    public class AppointmentService
    {
        private readonly AppDbContext _context;
        private readonly NotificationService _notificationService;

        public AppointmentService(AppDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

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

        public async Task<Appointment> AddAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Reload patient for notification
            await _context.Entry(appointment)
                .Reference(a => a.Patient)
                .LoadAsync();

            var patientName = appointment.Patient?.FullName ?? "نامشخص";

            // ✅ استفاده مستقیم از CreateNotificationAsync
            if (appointment.Status == AppointmentStatus.Emergency)
            {
                await _notificationService.CreateNotificationAsync(
                    "⚠️ نوبت فوری",
                    $"نوبت فوری برای بیمار {patientName} ثبت شد.",
                    NotificationType.AppointmentEmergency,
                    "emergency.wav",
                    appointment.Id
                );
            }
            else
            {
                await _notificationService.CreateNotificationAsync(
                    "نوبت جدید",
                    $"نوبت برای بیمار {patientName} در تاریخ {appointment.AppointmentDateTime:yyyy/MM/dd HH:mm} ثبت شد.",
                    NotificationType.AppointmentCreated,
                    "new_appointment.wav",
                    appointment.Id
                );
            }

            return appointment;
        }

        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();

            // Reload patient for notification
            await _context.Entry(appointment)
                .Reference(a => a.Patient)
                .LoadAsync();

            var patientName = appointment.Patient?.FullName ?? "نامشخص";

            // ✅ استفاده مستقیم از CreateNotificationAsync
            await _notificationService.CreateNotificationAsync(
                "بروزرسانی نوبت",
                $"نوبت بیمار {patientName} بروزرسانی شد.",
                NotificationType.AppointmentUpdated,
                "update.wav",
                appointment.Id
            );
        }

        public async Task DeleteAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return;

            var patientName = appointment.Patient?.FullName ?? "نامشخص";

            // ✅ استفاده مستقیم از CreateNotificationAsync (قبل از حذف)
            await _notificationService.CreateNotificationAsync(
                "لغو نوبت",
                $"نوبت بیمار {patientName} لغو شد.",
                NotificationType.AppointmentCancelled,
                "cancel.wav",
                appointment.Id
            );

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }
    }
}
