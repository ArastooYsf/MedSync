namespace MedSync.Models;

public enum NotificationType
{
    AppointmentCreated,    // نوبت جدید ایجاد شد
    AppointmentUpdated,    // نوبت ویرایش شد
    AppointmentDeleted,    // نوبت حذف شد
    AppointmentReminder,   // یادآوری 10 دقیقه قبل
    AppointmentMissed,     // نوبت گذشت و انجام نشد
    AppointmentEmergency   // نوبت اورژانسی (صدای متفاوت)
}