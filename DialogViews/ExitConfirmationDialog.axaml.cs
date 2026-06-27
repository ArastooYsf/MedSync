using Avalonia.Controls;
using MedSync.DialogViewModels;

namespace MedSync.DialogViews;

public partial class ExitConfirmationDialog : Window
{
    public ExitConfirmationDialog()
    {
        InitializeComponent();
    }

    public ExitConfirmationDialog(ExitConfirmationViewModel viewModel) : this()
    {
        DataContext = viewModel;

        viewModel.ConfirmExit += () =>
        {
            Close(true);
        };

        viewModel.CancelExit += () =>
        {
            Close(false);
        };
    }
}