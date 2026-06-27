using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedSync.Services;

namespace MedSync.ViewModels;

public partial class LoginViewModel(AuthService authService) : ObservableObject
{
    private readonly AuthService _authService = authService;

    [ObservableProperty] private string _username = string.Empty;

    [ObservableProperty] private string _password = string.Empty;

    [ObservableProperty] private string _usernameError = string.Empty;

    [ObservableProperty] private string _passwordError = string.Empty;

    [ObservableProperty] private string _generalError = string.Empty;

    [ObservableProperty] private bool _isLoading;

    public event Action? LoginSucceeded;

    [RelayCommand]
    private void Login()
    {
        ClearErrors();

        if (!ValidateInputs())
            return;

        IsLoading = true;

        try
        {
            if (_authService.Login(Username, Password))
            {
                LoginSucceeded?.Invoke();
            }
            else
            {
                GeneralError = "نام کاربری یا رمز عبور اشتباه است";
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool ValidateInputs()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(Username))
        {
            UsernameError = "نام کاربری نمی‌تواند خالی باشد";
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            PasswordError = "رمز عبور نمی‌تواند خالی باشد";
            isValid = false;
        }

        return isValid;
    }

    private void ClearErrors()
    {
        UsernameError = string.Empty;
        PasswordError = string.Empty;
        GeneralError = string.Empty;
    }
}