using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using MedSync.Data;
using MedSync.Factories;
using MedSync.Services;
using MedSync.ViewModels;
using MedSync.DialogViewModels;
using MedSync.DialogViews;
using MedSync.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

[assembly: XmlnsDefinition("https://github.com/avaloniaui", "MedSync.Controls")]

namespace MedSync;

public partial class App : Application
{
    public IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=medsync.db"));

        collection.AddSingleton<MainViewModel>();
        collection.AddSingleton<AuthService>();
        collection.AddSingleton<AppointmentReminderService>();
        collection.AddSingleton<ViewModels.NotificationPanelViewModel>();

        collection.AddScoped<PatientService>();
        collection.AddScoped<AppointmentService>();
        collection.AddScoped<DbSeeder>();
        collection.AddSingleton<NotificationService>();
        collection.AddSingleton<AudioService>();
        collection.AddSingleton<SystemTrayService>();


        collection.AddTransient<AddAppointmentDialogViewModel>();
        collection.AddTransient<AddAppointmentDialog>();
        collection.AddTransient<Views.MainView>();
        collection.AddTransient<LoginView>();
        collection.AddTransient<LoginViewModel>();
        collection.AddTransient<AddPatientDialogViewModel>();
        collection.AddSingleton<AppointmentsPageViewModel>();
        collection.AddTransient<LogsPageViewModel>();
        collection.AddTransient<PatientsPageViewModel>();
        collection.AddTransient<PrescriptionsPageViewModel>();
        collection.AddTransient<SettingsPageViewModel>();
        collection.AddTransient<TestsPageViewModel>();
        collection.AddTransient<WorkersPageViewModel>();
        collection.AddTransient<SupportsPageViewModel>();
        collection.AddTransient<ExitConfirmationViewModel>();
        collection.AddTransient<ExitConfirmationDialog>();

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

        Services = collection.BuildServiceProvider();
        var services = Services;

        // Migrate and seed database
        Task.Run(async () =>
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Apply migrations
            await context.Database.MigrateAsync();

            // Seed data
            var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
            await seeder.SeedAsync();
        }).Wait();
        
        var reminderService = Services.GetRequiredService<AppointmentReminderService>();
        reminderService.Start();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var authService = services.GetRequiredService<AuthService>();

            if (!authService.IsAuthenticated)
            {
                var loginView = services.GetRequiredService<LoginView>();
                desktop.MainWindow = loginView;
                loginView.Show();
            }
            else
            {
                desktop.MainWindow = new Views.MainView
                {
                    DataContext = services.GetRequiredService<MainViewModel>()
                };
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}
