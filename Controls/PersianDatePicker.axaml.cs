// Controls/PersianDatePicker.axaml.cs
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using MedSync.DialogViews;
using MedSync.Helpers;

namespace MedSync.Controls;

public partial class PersianDatePicker : UserControl
{
    // ─── Styled Properties ───────────────────────────────────────────────────

    public static readonly StyledProperty<DateTime?> SelectedDateProperty =
        AvaloniaProperty.Register<PersianDatePicker, DateTime?>(
            nameof(SelectedDate),
            defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public DateTime? SelectedDate
    {
        get => GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    // ─── Constructor ─────────────────────────────────────────────────────────

    public PersianDatePicker()
    {
        InitializeComponent();
        UpdateDisplay();
    }

    // ─── Property Changed ────────────────────────────────────────────────────

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SelectedDateProperty)
            UpdateDisplay();
    }

    // ─── Display ─────────────────────────────────────────────────────────────

    private void UpdateDisplay()
    {
        if (DisplayText is null) return;

        DisplayText.Text = SelectedDate.HasValue
            ? PersianCalendarHelper.FormatPersianLong(SelectedDate.Value)
            : "انتخاب تاریخ";
    }

    // ─── Open Popup ──────────────────────────────────────────────────────────

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        OpenPicker();
        e.Handled = true;
    }

    private async void OpenPicker()
    {
        var initialDate = SelectedDate ?? DateTime.Today;

        var dialog = new PersianDatePickerDialog(initialDate);
        var result = await dialog.ShowDialog<DateTime?>(
            (Window)TopLevel.GetTopLevel(this)!);

        if (result.HasValue)
            SelectedDate = result.Value;
    }
}
