using System;
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
    private enum PickerMode { Day, Month, Year }

    private int _currentPYear;
    private int _currentPMonth;
    private readonly DateTime? _selectedDate;
    private PickerMode _mode = PickerMode.Day;
    private int _yearRangeStart;

    public PersianDatePickerDialog()
    {
        InitializeComponent();
    }

    public PersianDatePickerDialog(DateTime initialDate) : this()
    {
        _currentPYear = PersianCalendarHelper.GetPersianYear(initialDate);
        _currentPMonth = PersianCalendarHelper.GetPersianMonth(initialDate);
        _selectedDate = initialDate;
        _yearRangeStart = (_currentPYear / 12) * 12;

        PrevMonthButton.Click += OnPrev;
        NextMonthButton.Click += OnNext;
        TodayButton.Click += OnToday;
        CancelButton.Click += OnCancel;
        MonthLabel.PointerPressed += OnMonthLabelClick;
        YearLabel.PointerPressed += OnYearLabelClick;

        Render();
    }

    private void OnPrev(object? sender, RoutedEventArgs e)
    {
        switch (_mode)
        {
            case PickerMode.Day:
                _currentPMonth--;
                if (_currentPMonth < 1) { _currentPMonth = 12; _currentPYear--; }
                break;
            case PickerMode.Year:
                _yearRangeStart -= 12;
                break;
        }
        Render();
    }

    private void OnNext(object? sender, RoutedEventArgs e)
    {
        switch (_mode)
        {
            case PickerMode.Day:
                _currentPMonth++;
                if (_currentPMonth > 12) { _currentPMonth = 1; _currentPYear++; }
                break;
            case PickerMode.Year:
                _yearRangeStart += 12;
                break;
        }
        Render();
    }

    private void OnToday(object? sender, RoutedEventArgs e) => Close(DateTime.Today);
    private void OnCancel(object? sender, RoutedEventArgs e) => Close(null);

    private void OnMonthLabelClick(object? sender, PointerPressedEventArgs e)
    {
        _mode = _mode == PickerMode.Month ? PickerMode.Day : PickerMode.Month;
        Render();
    }

    private void OnYearLabelClick(object? sender, PointerPressedEventArgs e)
    {
        _mode = _mode == PickerMode.Year ? PickerMode.Day : PickerMode.Year;
        Render();
    }

    private void Render()
    {
        UpdateHeader();

        DayPanel.IsVisible = _mode == PickerMode.Day;
        MonthPanel.IsVisible = _mode == PickerMode.Month;
        YearPanel.IsVisible = _mode == PickerMode.Year;

        WeekDaysHeader.IsVisible = _mode == PickerMode.Day;
        PrevMonthButton.IsVisible = _mode != PickerMode.Month;
        NextMonthButton.IsVisible = _mode != PickerMode.Month;

        switch (_mode)
        {
            case PickerMode.Day:
                DayPanel.Children.Clear();
                RenderDays();
                break;
            case PickerMode.Month:
                MonthPanel.Children.Clear();
                RenderMonths();
                break;
            case PickerMode.Year:
                YearPanel.Children.Clear();
                RenderYears();
                break;
        }
    }

    private void UpdateHeader()
    {
        MonthLabel.Text = _mode == PickerMode.Month
            ? "انتخاب ماه"
            : PersianCalendarHelper.GetPersianMonthName(_currentPMonth);

        YearLabel.Text = _mode == PickerMode.Year
            ? $"{_yearRangeStart}–{_yearRangeStart + 11}"
            : _currentPYear.ToString();

        MonthLabel.Foreground = _mode == PickerMode.Month
            ? new SolidColorBrush(Color.Parse("#3A7FD5"))
            : new SolidColorBrush(Colors.White);

        YearLabel.Foreground = _mode == PickerMode.Year
            ? new SolidColorBrush(Color.Parse("#3A7FD5"))
            : new SolidColorBrush(Colors.White);
    }

    private void RenderDays()
    {
        var firstGreg = PersianCalendarHelper.ToGregorian(_currentPYear, _currentPMonth, 1);
        int startOffset = GetPersianDayOfWeek(firstGreg.DayOfWeek);
        int daysInMonth = PersianCalendarHelper.GetPersianDaysInMonth(_currentPYear, _currentPMonth);

        // سلول‌های خالی
        for (int i = 0; i < startOffset; i++)
        {
            var empty = new Border();
            Grid.SetRow(empty, i / 7);
            Grid.SetColumn(empty, i % 7);
            DayPanel.Children.Add(empty);
        }

        // روزها
        for (int day = 1; day <= daysInMonth; day++)
        {
            var greg = PersianCalendarHelper.ToGregorian(_currentPYear, _currentPMonth, day);
            bool isToday = greg.Date == DateTime.Today;
            bool isSelected = _selectedDate?.Date == greg.Date;

            var cell = MakeDayCell(day, greg, isToday, isSelected);
            int idx = startOffset + day - 1;
            Grid.SetRow(cell, idx / 7);
            Grid.SetColumn(cell, idx % 7);
            DayPanel.Children.Add(cell);
        }
    }

    private void RenderMonths()
    {
        string[] names =
        [
            "فروردین","اردیبهشت","خرداد",
            "تیر","مرداد","شهریور",
            "مهر","آبان","آذر",
            "دی","بهمن","اسفند"
        ];

        int todayMonth = PersianCalendarHelper.GetPersianMonth(DateTime.Today);
        int todayYear = PersianCalendarHelper.GetPersianYear(DateTime.Today);

        for (int m = 1; m <= 12; m++)
        {
            int month = m;
            bool isSelected = month == _currentPMonth &&
                              _selectedDate.HasValue &&
                              PersianCalendarHelper.GetPersianYear(_selectedDate.Value) == _currentPYear;
            bool isCurrent = month == todayMonth && _currentPYear == todayYear;

            var cell = MakePickerCell(names[m - 1], isSelected, isCurrent, () =>
            {
                _currentPMonth = month;
                _mode = PickerMode.Day;
                Render();
            });

            Grid.SetRow(cell, (m - 1) / 3);
            Grid.SetColumn(cell, (m - 1) % 3);
            MonthPanel.Children.Add(cell);
        }
    }

    private void RenderYears()
    {
        int todayYear = PersianCalendarHelper.GetPersianYear(DateTime.Today);

        for (int i = 0; i < 12; i++)
        {
            int year = _yearRangeStart + i;
            bool isSelected = year == _currentPYear &&
                              _selectedDate.HasValue &&
                              PersianCalendarHelper.GetPersianYear(_selectedDate.Value) == year;
            bool isCurrent = year == todayYear;

            var cell = MakePickerCell(year.ToString(), isSelected, isCurrent, () =>
            {
                _currentPYear = year;
                _mode = PickerMode.Day;
                Render();
            });

            Grid.SetRow(cell, i / 3);
            Grid.SetColumn(cell, i % 3);
            YearPanel.Children.Add(cell);
        }
    }

    private Control MakeDayCell(int day, DateTime greg, bool isToday, bool isSelected)
    {
        var normalBg = isSelected ? Color.Parse("#3A7FD5")
                      : isToday ? Color.Parse("#1E3A5F")
                                  : Colors.Transparent;
        var restoreBg = isToday ? Color.Parse("#1E3A5F") : Colors.Transparent;

        var border = new Border
        {
            Margin = new Thickness(2),
            CornerRadius = new CornerRadius(6),
            Background = new SolidColorBrush(normalBg),
            Cursor = new Cursor(StandardCursorType.Hand),
            Child = new TextBlock
            {
                Text = day.ToString(),
                FontSize = 13,
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontFamily = new FontFamily("avares://MedSync/Assets/Fonts#Shabnam FD"),
            }
        };

        if (!isSelected)
        {
            border.PointerEntered += (_, _) => border.Background = new SolidColorBrush(Color.Parse("#1A3A6A"));
            border.PointerExited += (_, _) => border.Background = new SolidColorBrush(restoreBg);
        }

        border.PointerPressed += (_, e) =>
        {
            if (e.GetCurrentPoint(border).Properties.IsLeftButtonPressed)
                Close(greg);
        };

        return border;
    }

    private Control MakePickerCell(string label, bool isSelected, bool isCurrent, Action onClick)
    {
        var normalBg = isSelected ? Color.Parse("#3A7FD5")
                      : isCurrent ? Color.Parse("#1E3A5F")
                                   : Colors.Transparent;
        var restoreBg = isCurrent ? Color.Parse("#1E3A5F") : Colors.Transparent;

        var border = new Border
        {
            Margin = new Thickness(4),
            CornerRadius = new CornerRadius(8),
            Background = new SolidColorBrush(normalBg),
            Cursor = new Cursor(StandardCursorType.Hand),
            Child = new TextBlock
            {
                Text = label,
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontFamily = new FontFamily("avares://MedSync/Assets/Fonts#Shabnam FD"),
            }
        };

        if (!isSelected)
        {
            border.PointerEntered += (_, _) => border.Background = new SolidColorBrush(Color.Parse("#1A3A6A"));
            border.PointerExited += (_, _) => border.Background = new SolidColorBrush(restoreBg);
        }

        border.PointerPressed += (_, e) =>
        {
            if (e.GetCurrentPoint(border).Properties.IsLeftButtonPressed)
                onClick();
        };

        return border;
    }

    private static int GetPersianDayOfWeek(DayOfWeek dow) => dow switch
    {
        DayOfWeek.Saturday => 0,
        DayOfWeek.Sunday => 1,
        DayOfWeek.Monday => 2,
        DayOfWeek.Tuesday => 3,
        DayOfWeek.Wednesday => 4,
        DayOfWeek.Thursday => 5,
        DayOfWeek.Friday => 6,
        _ => 0
    };
}