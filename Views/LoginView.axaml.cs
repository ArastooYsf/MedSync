using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using MedSync.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MedSync.Views;

public partial class LoginView : Window
{
    public LoginView()
    {
        InitializeComponent();
    }

    public LoginView(LoginViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.LoginSucceeded += OnLoginSucceeded;
    }

    private void OnLoginSucceeded()
    {
        var app = (App)Application.Current!;
        var mainView = new MainView
        {
            DataContext = app.Services!.GetRequiredService<MainViewModel>()
        };
    
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = mainView;
            mainView.Show();
        }
    
        Close();
    }
    
    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}