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
    // ─── Fields ──────────────────────────────────────────────────────────────

    private enum PickerMode { Day, Month, Year }

    private int _currentPYear;
    private int _currentPMonth;
    private DateTime? _selectedDate;
    private PickerMode _mode = PickerMode.Day;
    private int _yearRangeStart;

    // ─── Constructors ────────────────────────────────────────────────────────

    public PersianDatePickerDialog()
    {
        InitializeComponent();
    }

    public PersianDatePickerDialog(DateTime initialDate) : this()
    {
        _currentPYear   = PersianCalendarHelper.GetPersianYear(initialDate);
        _currentPMonth  = PersianCalendarHelper.GetPersianMonth(initialDate);
        _selectedDate   = initialDate;
        _yearRangeStart = (_currentPYear / 12) * 12;

        PrevMonthButton.Click         += OnPrev;
        NextMonthButton.Click         += OnNext;
        TodayButton.Click             += OnToday;
        CancelButton.Click            += OnCancel;
        MonthLabel.PointerPressed     += OnMonthLabelClick;
        YearLabel.PointerPressed      += OnYearLabelClick;

        Render();
    }

    // ─── Events ──────────────────────────────────────────────────────────────

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

    // ─── Render ──────────────────────────────────────────────────────────────

    private void Render()
    {
        UpdateHeader();
        DaysGrid.Items.Clear();

        switch (_mode)
        {
            case PickerMode.Day:
                WeekDaysHeader.IsVisible    = true;
                PrevMonthButton.IsVisible   = true;
                NextMonthButton.IsVisible   = true;
                RenderDays();
                break;
            case PickerMode.Month:
                WeekDaysHeader.IsVisible    = false;
                PrevMonthButton.IsVisible   = false;
                NextMonthButton.IsVisible   = false;
                RenderMonths();
                break;
            case PickerMode.Year:
                WeekDaysHeader.IsVisible    = false;
                PrevMonthButton.IsVisible   = true;
                NextMonthButton.IsVisible   = true;
                RenderYears();
                break;
        }
    }

    // ─── Header ──────────────────────────────────────────────────────────────

    private void UpdateHeader()
    {
        // ترتیب: ماه سال (مثل تیر ۱۴۰۴)
        MonthLabel.Text = _mode switch
        {
            PickerMode.Month => "انتخاب ماه",
            _                => PersianCalendarHelper.GetPersianMonthName(_currentPMonth)
        };

        YearLabel.Text = _mode switch
        {
            PickerMode.Year => $"{_yearRangeStart}–{_yearRangeStart + 11}",
            _               => _currentPYear.ToString()
        };

        MonthLabel.Foreground = _mode == PickerMode.Month
            ? new SolidColorBrush(Color.Parse("#3A7FD5"))
            : new SolidColorBrush(Colors.White);

        YearLabel.Foreground = _mode == PickerMode.Year
            ? new SolidColorBrush(Color.Parse("#3A7FD5"))
            : new SolidColorBrush(Colors.White);
    }

    // ─── Day Render ──────────────────────────────────────────────────────────

    private void RenderDays()
    {
        var firstGreg    = PersianCalendarHelper.ToGregorian(_currentPYear, _currentPMonth, 1);
        int startOffset  = GetPersianDayOfWeek(firstGreg.DayOfWeek);
        int daysInMonth  = PersianCalendarHelper.GetPersianDaysInMonth(_currentPYear, _currentPMonth);

        // سلول‌های خالی قبل از روز اول
        for (int i = 0; i < startOffset; i++)
            DaysGrid.Items.Add(new Border { Width = 40, Height = 40, Margin = new Thickness(2) });

        for (int day = 1; day <= daysInMonth; day++)
        {
            var greg       = PersianCalendarHelper.ToGregorian(_currentPYear, _currentPMonth, day);
            bool isToday   = greg.Date == DateTime.Today;
            bool isSelected = _selectedDate?.Date == greg.Date;
            DaysGrid.Items.Add(MakeDayCell(day, greg, isToday, isSelected));
        }
    }

    // ─── Month Render ─────────────────────────────────────────────────────────

    private void RenderMonths()
    {
        string[] names =
        {
            "فروردین","اردیبهشت","خرداد",
            "تیر","مرداد","شهریور",
            "مهر","آبان","آذر",
            "دی","بهمن","اسفند"
        };

        int todayMonth = PersianCalendarHelper.GetPersianMonth(DateTime.Today);
        int todayYear  = PersianCalendarHelper.GetPersianYear(DateTime.Today);

        for (int m = 1; m <= 12; m++)
        {
            int month      = m;
            bool isSelected = month == _currentPMonth &&
                              _selectedDate.HasValue &&
                              PersianCalendarHelper.GetPersianYear(_selectedDate.Value) == _currentPYear;
            bool isCurrent  = month == todayMonth && _currentPYear == todayYear;

            var cell = MakePickerCell(names[m - 1], isSelected, isCurrent, () =>
            {
                _currentPMonth = month;
                _mode = PickerMode.Day;
                Render();
            });

            DaysGrid.Items.Add(cell);
        }
    }

    // ─── Year Render ──────────────────────────────────────────────────────────

    private void RenderYears()
    {
        int todayYear = PersianCalendarHelper.GetPersianYear(DateTime.Today);

        for (int i = 0; i < 12; i++)
        {
            int year       = _yearRangeStart + i;
            bool isSelected = year == _currentPYear &&
                              _selectedDate.HasValue &&
                              PersianCalendarHelper.GetPersianYear(_selectedDate.Value) == year;
            bool isCurrent  = year == todayYear;

            var cell = MakePickerCell(year.ToString(), isSelected, isCurrent, () =>
            {
                _currentPYear = year;
                _mode = PickerMode.Day;
                Render();
            });

            DaysGrid.Items.Add(cell);
        }
    }

    // ─── Cell Factories ───────────────────────────────────────────────────────

    private Control MakeDayCell(int day, DateTime greg, bool isToday, bool isSelected)
    {
        var normalBg   = isSelected ? Color.Parse("#3A7FD5")
                       : isToday   ? Color.Parse("#1E3A5F")
                                   : Colors.Transparent;
        var hoverBg    = Color.Parse("#1A3A6A");
        var restoreBg  = isToday ? Color.Parse("#1E3A5F") : Colors.Transparent;

        var border = new Border
        {
            Width        = 40,
            Height       = 40,
            Margin       = new Thickness(2),
            CornerRadius = new CornerRadius(6),
            Background   = new SolidColorBrush(normalBg),
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

        if (!isSelected)
        {
            border.PointerEntered += (_, _) => border.Background = new SolidColorBrush(hoverBg);
            border.PointerExited  += (_, _) => border.Background = new SolidColorBrush(restoreBg);
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
        var normalBg  = isSelected ? Color.Parse("#3A7FD5")
                      : isCurrent  ? Color.Parse("#1E3A5F")
                                   : Colors.Transparent;
        var hoverBg   = Color.Parse("#1A3A6A");
        var restoreBg = isCurrent ? Color.Parse("#1E3A5F") : Colors.Transparent;

        var border = new Border
        {
            Width        = 72,
            Height       = 44,
            Margin       = new Thickness(4),
            CornerRadius = new CornerRadius(8),
            Background   = new SolidColorBrush(normalBg),
            Cursor       = new Cursor(StandardCursorType.Hand),
            Child        = new TextBlock
            {
                Text                = label,
                FontSize            = 13,
                Foreground          = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
                FontFamily          = new FontFamily("avares://MedSync/Assets/Fonts#Shabnam FD"),
            }
        };

        if (!isSelected)
        {
            border.PointerEntered += (_, _) => border.Background = new SolidColorBrush(hoverBg);
            border.PointerExited  += (_, _) => border.Background = new SolidColorBrush(restoreBg);
        }

        border.PointerPressed += (_, e) =>
        {
            if (e.GetCurrentPoint(border).Properties.IsLeftButtonPressed)
                onClick();
        };

        return border;
    }

    // ─── Helper ───────────────────────────────────────────────────────────────

    // شنبه = ستون ۰، جمعه = ستون ۶
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