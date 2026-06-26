// DialogViews/PersianDatePickerDialog.axaml.cs
using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using MedSync.Helpers;

namespace MedSync.DialogViews;

public partial class PersianDatePickerDialog : Window
{
    // ─── Fields ──────────────────────────────────────────────────────────────

    private int _currentPYear;
    private int _currentPMonth;
    private DateTime? _selectedDate;

    // ─── Constructor ─────────────────────────────────────────────────────────

    public PersianDatePickerDialog(DateTime initialDate)
    {
        InitializeComponent();

        _currentPYear  = PersianCalendarHelper.GetPersianYear(initialDate);
        _currentPMonth = PersianCalendarHelper.GetPersianMonth(initialDate);
        _selectedDate  = initialDate;

        PrevMonthButton.Click += OnPrevMonth;
        NextMonthButton.Click += OnNextMonth;
        TodayButton.Click     += OnToday;
        CancelButton.Click    += OnCancel;

        RenderCalendar();
    }

    // ─── Navigation ──────────────────────────────────────────────────────────

    private void OnPrevMonth(object? sender, RoutedEventArgs e)
    {
        _currentPMonth--;
        if (_currentPMonth < 1) { _currentPMonth = 12; _currentPYear--; }
        RenderCalendar();
    }

    private void OnNextMonth(object? sender, RoutedEventArgs e)
    {
        _currentPMonth++;
        if (_currentPMonth > 12) { _currentPMonth = 1; _currentPYear++; }
        RenderCalendar();
    }

    private void OnToday(object? sender, RoutedEventArgs e)
    {
        Close(DateTime.Today);
    }

    private void OnCancel(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }

    // ─── Render Calendar ─────────────────────────────────────────────────────

    private void RenderCalendar()
    {
        // آپدیت هدر
        MonthLabel.Text = PersianCalendarHelper.GetPersianMonthName(_currentPMonth);
        YearLabel.Text  = _currentPYear.ToString();

        DaysGrid.Items.Clear();

        // روز اول ماه چه روزی از هفته است؟
        // هفته شمسی: شنبه=0, یکشنبه=1, ..., جمعه=6
        var firstDay = PersianCalendarHelper.ToGregorian(_currentPYear, _currentPMonth, 1);
        int firstDayOfWeek = GetPersianDayOfWeek(firstDay.DayOfWeek);

        int daysInMonth = PersianCalendarHelper.GetPersianDaysInMonth(_currentPYear, _currentPMonth);

        // خانه‌های خالی قبل از روز اول
        for (int i = 0; i < firstDayOfWeek; i++)
            DaysGrid.Items.Add(CreateEmptyCell());

        // روزها
        for (int day = 1; day <= daysInMonth; day++)
        {
            var gregDate = PersianCalendarHelper.ToGregorian(_currentPYear, _currentPMonth, day);
            bool isToday    = gregDate.Date == DateTime.Today;
            bool isSelected = _selectedDate.HasValue &&
                              _selectedDate.Value.Date == gregDate.Date;

            DaysGrid.Items.Add(CreateDayCell(day, gregDate, isToday, isSelected));
        }
    }

    // ─── Cell Builders ───────────────────────────────────────────────────────

    private Control CreateEmptyCell()
    {
        return new Border { Width = 40, Height = 40, Margin = new Thickness(2) };
    }

    private Control CreateDayCell(int day, DateTime gregDate, bool isToday, bool isSelected)
    {
        var bg = isSelected
            ? new SolidColorBrush(Color.Parse("#3A7FD5"))
            : isToday
                ? new SolidColorBrush(Color.Parse("#1E3A5F"))
                : new SolidColorBrush(Colors.Transparent);

        var border = new Border
        {
            Width        = 40,
            Height       = 40,
            Margin       = new Thickness(2),
            CornerRadius = new CornerRadius(6),
            Background   = bg,
            Cursor       = new Cursor(StandardCursorType.Hand),
            Child        = new TextBlock
            {
                Text                = day.ToString(),
                FontSize            = 13,
                Foreground          = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
                FontFamily          = new FontFamily("avares://MedSync/Assets/Fonts#Shabnam FD"),
            }
        };

        // هایلایت hover
        border.PointerEntered += (_, _) =>
        {
            if (!isSelected)
                border.Background = new SolidColorBrush(Color.Parse("#1A3A6A"));
        };

        border.PointerExited += (_, _) =>
        {
            if (!isSelected)
                border.Background = isToday
                    ? new SolidColorBrush(Color.Parse("#1E3A5F"))
                    : new SolidColorBrush(Colors.Transparent);
        };

        border.PointerPressed += (_, e) =>
        {
            if (e.GetCurrentPoint(border).Properties.IsLeftButtonPressed)
                Close(gregDate);
        };

        return border;
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    /// <summary>
    /// تبدیل DayOfWeek میلادی به ایندکس هفته شمسی (شنبه=0)
    /// </summary>
    private static int GetPersianDayOfWeek(DayOfWeek dow) => dow switch
    {
        DayOfWeek.Saturday  => 0,
        DayOfWeek.Sunday    => 1,
        DayOfWeek.Monday    => 2,
        DayOfWeek.Tuesday   => 3,
        DayOfWeek.Wednesday => 4,
        DayOfWeek.Thursday  => 5,
        DayOfWeek.Friday    => 6,
        _                   => 0
    };
}
