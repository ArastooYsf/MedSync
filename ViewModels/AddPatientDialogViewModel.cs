using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedSync.Models;
using MedSync.Services;

namespace MedSync.ViewModels;

public partial class AddPatientDialogViewModel : ViewModelBase
{
    private readonly PatientService _patientService;

    [ObservableProperty] private string _firstName = string.Empty;

    [ObservableProperty] private string? _firstNameError;

    [ObservableProperty] private string _lastName = string.Empty;

    [ObservableProperty] private string? _lastNameError;

    [ObservableProperty] private string _nationalCode = string.Empty;

    [ObservableProperty] private string? _nationalCodeError;

    [ObservableProperty] private DateTime? _birthDate = DateTime.Now.AddYears(-30);

    [ObservableProperty] private string? _birthDateError;

    [ObservableProperty] private string _phoneNumber = string.Empty;

    [ObservableProperty] private string? _phoneNumberError;

    [ObservableProperty] private string _address = string.Empty;

    [ObservableProperty] private string _gender = string.Empty;

    [ObservableProperty] private string? _genderError;

    [ObservableProperty] private string _medicalHistory = string.Empty;

    [ObservableProperty] private bool _isSaving;

    public AddPatientDialogViewModel(PatientService patientService)
    {
        _patientService = patientService;
    }

    partial void OnFirstNameChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            FirstNameError = null;
    }

    partial void OnLastNameChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            LastNameError = null;
    }

    partial void OnNationalCodeChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        if (value.Length == 10 && long.TryParse(value, out _))
            NationalCodeError = null;
    }

    partial void OnPhoneNumberChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        if (value.Length == 11 && value.StartsWith("09"))
            PhoneNumberError = null;
    }

    partial void OnBirthDateChanged(DateTime? value)
    {
        if (value.HasValue &&
            value.Value <= DateTime.Today &&
            value.Value >= DateTime.Today.AddYears(-120))
        {
            BirthDateError = null;
        }
    }

    partial void OnGenderChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            GenderError = null;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!await ValidateInputsAsync())
            return;

        IsSaving = true;

        try
        {
            var patient = new Patient
            {
                FirstName = FirstName.Trim(),
                LastName = LastName.Trim(),
                NationalCode = NationalCode.Trim(),
                BirthDate = BirthDate!.Value,
                PhoneNumber = PhoneNumber.Trim(),
                Address = Address.Trim(),
                Gender = Gender,
                MedicalHistory = string.IsNullOrWhiteSpace(MedicalHistory) ? null : MedicalHistory.Trim(),
                CreatedAt = DateTime.Now
            };

            await _patientService.AddPatientAsync(patient);
            CloseDialog?.Invoke(true);
        }
        catch (Exception)
        {
            NationalCodeError = "خطا در ذخیره اطلاعات. ممکن است این کد ملی قبلاً ثبت شده باشد";
        }
        finally
        {
            IsSaving = false;
        }
    }

    private async Task<bool> ValidateInputsAsync()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(FirstName))
        {
            FirstNameError = "نام الزامی است";
            isValid = false;
        }
        else
        {
            FirstNameError = null;
        }

        if (string.IsNullOrWhiteSpace(LastName))
        {
            LastNameError = "نام خانوادگی الزامی است";
            isValid = false;
        }
        else
        {
            LastNameError = null;
        }

        if (string.IsNullOrWhiteSpace(NationalCode))
        {
            NationalCodeError = "کد ملی الزامی است";
            isValid = false;
        }
        else if (NationalCode.Length != 10 || !long.TryParse(NationalCode, out _))
        {
            NationalCodeError = "کد ملی باید 10 رقم باشد";
            isValid = false;
        }
        else
        {
            var exists = await _patientService.GetPatientByNationalCodeAsync(NationalCode.Trim());
            if (exists != null)
            {
                NationalCodeError = "این کد ملی قبلاً ثبت شده است";
                isValid = false;
            }
            else
            {
                NationalCodeError = null;
            }
        }

        if (!BirthDate.HasValue)
        {
            BirthDateError = "تاریخ تولد الزامی است";
            isValid = false;
        }
        else if (BirthDate.Value > DateTime.Today)
        {
            BirthDateError = "تاریخ تولد نمی‌تواند در آینده باشد";
            isValid = false;
        }
        else if (BirthDate.Value < DateTime.Today.AddYears(-120))
        {
            BirthDateError = "تاریخ تولد نامعتبر است";
            isValid = false;
        }
        else
        {
            BirthDateError = null;
        }

        if (string.IsNullOrWhiteSpace(PhoneNumber))
        {
            PhoneNumberError = "شماره تماس الزامی است";
            isValid = false;
        }
        else if (PhoneNumber.Length != 11 || !PhoneNumber.StartsWith("09"))
        {
            PhoneNumberError = "شماره تماس باید 11 رقم و با 09 شروع شود";
            isValid = false;
        }
        else
        {
            PhoneNumberError = null;
        }

        if (string.IsNullOrWhiteSpace(Gender))
        {
            GenderError = "جنسیت الزامی است";
            isValid = false;
        }
        else
        {
            GenderError = null;
        }

        return isValid;
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseDialog?.Invoke(false);
    }

    public Action<bool>? CloseDialog { get; set; }
}