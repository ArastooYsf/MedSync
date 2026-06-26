using System;
using Avalonia.Controls;
using MedSync.ViewModels;
using MedSync.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MedSync.Views;

public partial class AppointmentsPageView : UserControl
{
    public AppointmentsPageView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private AppointmentsPageViewModel? _previousVm;

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (_previousVm != null)
        {
            _previousVm.AddAppointmentRequested  -= OnAddAppointmentRequested;
            _previousVm.EditAppointmentRequested -= OnEditAppointmentRequested;
        }

        if (DataContext is AppointmentsPageViewModel vm)
        {
            vm.AddAppointmentRequested  += OnAddAppointmentRequested;
            vm.EditAppointmentRequested += OnEditAppointmentRequested;
            _previousVm = vm;
        }
    }

    private async void OnAddAppointmentRequested(object? sender, EventArgs e)
    {
        if (DataContext is not AppointmentsPageViewModel vm) return;
        var services = ((App)Avalonia.Application.Current!).Services!;
        var dialogVm = services.GetRequiredService<AddAppointmentDialogViewModel>();
        var dialog = new AddAppointmentDialog(dialogVm);
        var topLevel = TopLevel.GetTopLevel(this) as Window;
        if (topLevel is null) return;
        var result = await dialog.ShowDialog<bool>(topLevel);
        if (result)
            await vm.LoadAppointmentsCommand.ExecuteAsync(null);
    }

    private async void OnEditAppointmentRequested(object? sender, AppointmentCardViewModel cardVm)
    {
        if (DataContext is not AppointmentsPageViewModel vm) return;
        var services           = ((App)Avalonia.Application.Current!).Services!;
        var appointmentService = services.GetRequiredService<AppointmentService>();
        var patientService     = services.GetRequiredService<PatientService>();
        var existing = await appointmentService.GetAppointmentByIdAsync(cardVm.Id);
        if (existing is null) return;
        var dialogVm = new AddAppointmentDialogViewModel(
            appointmentService,
            patientService,
            existing);
        var dialog = new AddAppointmentDialog(dialogVm);
        var topLevel = TopLevel.GetTopLevel(this) as Window;
        if (topLevel is null) return;
        var result = await dialog.ShowDialog<bool>(topLevel);
        if (result)
            await vm.LoadAppointmentsCommand.ExecuteAsync(null);
    }
}