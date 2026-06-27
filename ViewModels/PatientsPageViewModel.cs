using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedSync.Data;
using MedSync.Models;
using MedSync.Services;

namespace MedSync.ViewModels;

public partial class PatientsPageViewModel : PageViewModel
{
    internal readonly PatientService _patientService;

    [ObservableProperty] private ObservableCollection<Patient> _patients = [];

    [ObservableProperty] private ObservableCollection<Patient> _filteredPatients = [];

    [ObservableProperty] private Patient? _selectedPatient;

    [ObservableProperty] private bool _isLoading;

    [ObservableProperty] private string _searchText = string.Empty;

    [ObservableProperty] private bool _showDetail;

    [ObservableProperty] private PatientDetailViewModel? _detailViewModel;

    public string[] FilterOptions { get; } = ["همه", "مرد", "زن"];

    [ObservableProperty] private string _selectedFilter = "همه";

    public PatientsPageViewModel(PatientService patientService)
    {
        PageName = ApplicationPageNames.Patients;
        _patientService = patientService;
        _ = LoadPatientsAsync();
    }

    partial void OnSearchTextChanged(string value) => ApplyFilters();
    partial void OnSelectedFilterChanged(string value) => ApplyFilters();

    private void ApplyFilters()
    {
        var result = Patients.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
            result = result.Where(p =>
                p.FullName.Contains(SearchText) ||
                p.NationalCode.Contains(SearchText) ||
                p.PhoneNumber.Contains(SearchText));

        if (SelectedFilter != "همه")
            result = result.Where(p => p.Gender == SelectedFilter);

        FilteredPatients = new ObservableCollection<Patient>(result);
    }

    private async Task LoadPatientsAsync()
    {
        IsLoading = true;
        var patients = await _patientService.GetAllPatientsAsync();
        Patients = new ObservableCollection<Patient>(patients);
        ApplyFilters();
        IsLoading = false;
    }

    [RelayCommand]
    private async Task RefreshPatients()
    {
        await LoadPatientsAsync();
    }

    [RelayCommand]
    private void OpenDetail(Patient patient)
    {
        DetailViewModel = new PatientDetailViewModel(patient, ShowListCommand, _patientService);
        ShowDetail = true;
    }


    [RelayCommand]
    private void ShowList()
    {
        ShowDetail = false;
        DetailViewModel = null;
        _ = LoadPatientsAsync();
    }

    [RelayCommand]
    private async Task AddPatient()
    {
        // این از طریق event به View اطلاع میده
        await Task.CompletedTask;
        AddPatientRequested?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? AddPatientRequested;


    [RelayCommand]
    private async Task DeletePatient(Patient? patient)
    {
        if (patient == null) return;
        await _patientService.DeletePatientAsync(patient.Id);
        await LoadPatientsAsync();
    }
}