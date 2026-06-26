using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedSync.Models;
using MedSync.Services;

namespace MedSync.ViewModels;

public partial class PatientDetailViewModel : ViewModelBase
{
    private readonly IRelayCommand _goBackCommand;
    private readonly PatientService _patientService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsInfoTab))]
    [NotifyPropertyChangedFor(nameof(IsVisitsTab))]
    [NotifyPropertyChangedFor(nameof(IsPrescriptionsTab))]
    [NotifyPropertyChangedFor(nameof(IsTestsTab))]
    [NotifyPropertyChangedFor(nameof(IsFilesTab))]
    private string _activeTab = "info";

    [ObservableProperty] private Patient _patient;

    [ObservableProperty] private bool _isEditMode;

    [ObservableProperty] private string _editFirstName = string.Empty;

    [ObservableProperty] private string _editLastName = string.Empty;

    [ObservableProperty] private string _editNationalCode = string.Empty;

    [ObservableProperty] private DateTimeOffset? _editBirthDate;

    [ObservableProperty] private string _editPhoneNumber = string.Empty;

    [ObservableProperty] private string _editAddress = string.Empty;

    [ObservableProperty] private string _editGender = string.Empty;

    [ObservableProperty] private string? _editMedicalHistory;

    public bool IsInfoTab => ActiveTab == "info";
    public bool IsVisitsTab => ActiveTab == "visits";
    public bool IsPrescriptionsTab => ActiveTab == "prescriptions";
    public bool IsTestsTab => ActiveTab == "tests";
    public bool IsFilesTab => ActiveTab == "files";

    public PatientDetailViewModel(Patient patient, IRelayCommand goBackCommand, PatientService patientService)
    {
        _patient = patient;
        _goBackCommand = goBackCommand;
        _patientService = patientService;
        LoadEditFields();
    }

    private void LoadEditFields()
    {
        EditFirstName = Patient.FirstName;
        EditLastName = Patient.LastName;
        EditNationalCode = Patient.NationalCode;
        EditBirthDate = new DateTimeOffset(Patient.BirthDate, TimeSpan.Zero);
        EditPhoneNumber = Patient.PhoneNumber;
        EditAddress = Patient.Address;
        EditGender = Patient.Gender;
        EditMedicalHistory = Patient.MedicalHistory;
    }

    [RelayCommand]
    private void GoBack() => _goBackCommand.Execute(null);

    [RelayCommand]
    private void ToggleEditMode()
    {
        if (IsEditMode)
        {
            LoadEditFields();
            IsEditMode = false;
        }
        else
        {
            IsEditMode = true;
        }
    }

    [RelayCommand]
    private async Task SaveChanges()
    {
        Patient.FirstName = EditFirstName;
        Patient.LastName = EditLastName;
        Patient.NationalCode = EditNationalCode;
        Patient.BirthDate = EditBirthDate!.Value.DateTime;
        Patient.PhoneNumber = EditPhoneNumber;
        Patient.Address = EditAddress;
        Patient.Gender = EditGender;
        Patient.MedicalHistory = EditMedicalHistory;

        await _patientService.UpdatePatientAsync(Patient);
        IsEditMode = false;
    }

    [RelayCommand]
    private void SetTab(string tab) => ActiveTab = tab;

    [RelayCommand]
    private void AddPrescription()
    {
    }

    [RelayCommand]
    private void AddTest()
    {
    }

    [RelayCommand]
    private void AddVisit()
    {
    }

    [RelayCommand]
    private void UploadFile()
    {
    }
}