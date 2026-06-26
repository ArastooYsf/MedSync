using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using MedSync.Data;
using MedSync.DialogViewModels;
using MedSync.DialogViews;

namespace MedSync.ViewModels;

public partial class SettingsPageViewModel : PageViewModel
{
    private readonly AppDbContext _dbContext;

    public SettingsPageViewModel(AppDbContext dbContext)
    {
        PageName = ApplicationPageNames.Settings;
        _dbContext = dbContext;
    }

    [RelayCommand]
    private async Task ResetDatabase()
    {
        var mainWindow = (Avalonia.Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
            ?.MainWindow;

        if (mainWindow == null) return;

        var resetDialog = new ResetConfirmationView
        {
            DataContext = new ResetConfirmationViewModel()
        };

        var vm = (ResetConfirmationViewModel)resetDialog.DataContext!;
        vm.ConfirmReset += () =>
        {
            var flagPath = Path.Combine(Directory.GetCurrentDirectory(), "reset.flag");
            File.WriteAllText(flagPath, "reset");
            resetDialog.Close();
            Environment.Exit(0);
        };
        vm.CancelReset += () => resetDialog.Close();

        await resetDialog.ShowDialog(mainWindow);
    }
}