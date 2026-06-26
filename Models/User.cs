namespace MedSync.Models;

public class User
{
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    public bool CanSeeAppointments => true;
    public bool CanSeeLogs => true;
    public bool CanSeePatients => true;
    public bool CanSeePrescriptions => Role == UserRole.Doctor;
    public bool CanSeeSettings => Role == UserRole.Doctor;
    public bool CanSeeSupports => true;
    public bool CanSeeTests => Role == UserRole.Doctor;
    public bool CanSeeWorkers => Role == UserRole.Doctor;
}