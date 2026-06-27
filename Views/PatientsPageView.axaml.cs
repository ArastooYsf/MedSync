using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MedSync.ViewModels;

namespace MedSync.Views;

public partial class PatientsPageView : UserControl
{
    public PatientsPageView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is PatientsPageViewModel vm)
        {
            vm.AddPatientRequested += OnAddPatientRequested;
        }
    }

    private async void OnAddPatientRequested(object? sender, EventArgs e)
    {
        if (DataContext is not PatientsPageViewModel vm) return;

        var dialogVm = new AddPatientDialogViewModel(vm._patientService);
        var dialog = new AddPatientDialog(dialogVm);

        if (TopLevel.GetTopLevel(this) is not Window parentWindow) return;

        await dialog.ShowDialog(parentWindow);
        await vm.RefreshPatientsCommand.ExecuteAsync(null);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}