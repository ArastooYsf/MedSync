using Avalonia.Controls;
using MedSync.ViewModels;

namespace MedSync.Views;

public partial class AddPatientDialog : Window
{
    public AddPatientDialog()
    {
        InitializeComponent();
    }

    public AddPatientDialog(AddPatientDialogViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseDialog = (result) =>
        {
            Close(result);
        };
    }
}