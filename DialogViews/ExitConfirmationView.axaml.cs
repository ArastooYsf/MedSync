using Avalonia.Controls;
using MedSync.DialogViewModels;

namespace MedSync.DialogViews;

public partial class ExitConfirmationView : Window
{
    public ExitConfirmationView()
    {
        InitializeComponent();
    }

    public ExitConfirmationView(ExitConfirmationViewModel viewModel) : this()
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