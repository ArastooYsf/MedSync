using Avalonia;
using Avalonia.Controls;
using MedSync.DialogViewModels;
using MedSync.DialogViews;
using MedSync.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MedSync.Views;

public partial class MainView : Window
{
    public MainView()
    {
        InitializeComponent();
        Closing += MainView_Closing;
        DataContextChanged += MainView_DataContextChanged;
    }

    private MainViewModel? _previousViewModel;

    private void MainView_DataContextChanged(object? sender, System.EventArgs e)
    {
        if (_previousViewModel != null)
            _previousViewModel.RequestExit -= OnRequestExit;

        if (DataContext is MainViewModel mainViewModel)
        {
            mainViewModel.RequestExit += OnRequestExit;
            _previousViewModel = mainViewModel;
        }
    }

    private async void MainView_Closing(object? sender, WindowClosingEventArgs e)
    {
        e.Cancel = true;
        await ShowExitConfirmationAsync();
    }

    private async void OnRequestExit()
    {
        await ShowExitConfirmationAsync();
    }

    private async System.Threading.Tasks.Task ShowExitConfirmationAsync()
    {
        var services = ((App)Application.Current!).Services!;
        var exitViewModel = services.GetRequiredService<ExitConfirmationViewModel>();
        var exitDialog = new ExitConfirmationDialog(exitViewModel);

        var result = await exitDialog.ShowDialog<bool?>(this);

        if (result == true)
        {
            Closing -= MainView_Closing;
            Close();
        }
    }
}