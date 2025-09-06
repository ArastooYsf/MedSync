using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MedSync.Data;
using MedSync.Factories;
using MedSync.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MedSync;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.AddSingleton<MainViewModel>();
        collection.AddTransient<AppointmentsPageViewModel>();
        collection.AddTransient<LogsPageViewModel>();
        collection.AddTransient<PatientsPageViewModel>();
        collection.AddTransient<PrescriptionsPageViewModel>();
        collection.AddTransient<SettingsPageViewModel>();
        collection.AddTransient<TestsPageViewModel>();
        collection.AddTransient<WorkersPageViewModel>();
        collection.AddTransient<SupportsPageViewModel>();

        collection.AddSingleton<Func<ApplicationPageNames, PageViewModel>>(x => name => name switch
        {
            ApplicationPageNames.Logs => x.GetRequiredService<LogsPageViewModel>(),
            ApplicationPageNames.Tests => x.GetRequiredService<TestsPageViewModel>(),
            ApplicationPageNames.Workers => x.GetRequiredService<WorkersPageViewModel>(),
            ApplicationPageNames.Appointments => x.GetRequiredService<AppointmentsPageViewModel>(),
            ApplicationPageNames.Prescriptions => x.GetRequiredService<PrescriptionsPageViewModel>(),
            ApplicationPageNames.Patients => x.GetRequiredService<PatientsPageViewModel>(),
            ApplicationPageNames.Settings => x.GetRequiredService<SettingsPageViewModel>(),
            ApplicationPageNames.Supports => x.GetRequiredService<SupportsPageViewModel>(),
            _ => throw new InvalidOperationException(),
        });
        
        collection.AddSingleton<PageFactory>();
        
        var services = collection.BuildServiceProvider();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainView
            {
                DataContext = services.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}