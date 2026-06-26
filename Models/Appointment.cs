using System;

namespace MedSync.Models;

public enum AppointmentStatus
{
    Normal,     // عادی
    Emergency,  // اورژانسی
    Special     // خاص
}

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public DateTime AppointmentDateTime { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Normal;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}