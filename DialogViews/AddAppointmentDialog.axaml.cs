using Avalonia.Controls;
using MedSync.ViewModels;

namespace MedSync.Views;

public partial class AddAppointmentDialog : Window
{
    public AddAppointmentDialog()
    {
        InitializeComponent();
    }

    public AddAppointmentDialog(AddAppointmentDialogViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseDialog = (result) =>
        {
            Close(result);
        };
    }

}