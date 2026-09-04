using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using System.Linq;

namespace MedSync.Behaviors;

/// <summary>
/// Attached Property برای مدیریت Enter key در TextBox ها
/// </summary>
public static class TextBoxHelper
{
    /// <summary>
    /// وقتی true باشه، Enter به فیلد بعدی می‌ره
    /// </summary>
    public static readonly AttachedProperty<bool> MoveToNextOnEnterProperty =
        AvaloniaProperty.RegisterAttached<TextBox, bool>("MoveToNextOnEnter", typeof(TextBoxHelper), false);

    /// <summary>
    /// اگر true باشه، Shift+Enter خط جدید می‌زنه (برای TextBox های چندخطی)
    /// </summary>
    public static readonly AttachedProperty<bool> IsMultilineProperty =
        AvaloniaProperty.RegisterAttached<TextBox, bool>("IsMultiline", typeof(TextBoxHelper), false);

    static TextBoxHelper()
    {
        MoveToNextOnEnterProperty.Changed.AddClassHandler<TextBox>(OnMoveToNextOnEnterChanged);
    }

    public static void SetMoveToNextOnEnter(TextBox element, bool value)
    {
        element.SetValue(MoveToNextOnEnterProperty, value);
    }

    public static bool GetMoveToNextOnEnter(TextBox element)
    {
        return element.GetValue(MoveToNextOnEnterProperty);
    }

    public static void SetIsMultiline(TextBox element, bool value)
    {
        element.SetValue(IsMultilineProperty, value);
    }

    public static bool GetIsMultiline(TextBox element)
    {
        return element.GetValue(IsMultilineProperty);
    }

    private static void OnMoveToNextOnEnterChanged(TextBox textBox, AvaloniaPropertyChangedEventArgs e)
    {
        if ((bool)e.NewValue!)
        {
            textBox.KeyDown += OnTextBoxKeyDown;
        }
        else
        {
            textBox.KeyDown -= OnTextBoxKeyDown;
        }
    }

    private static void OnTextBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is not TextBox textBox || e.Key != Key.Enter)
            return;

        var isMultiline = GetIsMultiline(textBox);

        // اگر چندخطیه و Shift فشرده شده، خط جدید بزن
        if (isMultiline && e.KeyModifiers.HasFlag(KeyModifiers.Shift))
            return;

        // در غیر این صورت برو به فیلد بعدی
        e.Handled = true;
        MoveFocusToNext(textBox);
    }

    private static void MoveFocusToNext(Control control)
    {
        var topLevel = TopLevel.GetTopLevel(control);
        if (topLevel == null) return;

        var focusableElements = topLevel
            .GetVisualDescendants()
            .OfType<InputElement>()
            .Where(el => el.Focusable && el.IsEffectivelyEnabled && el.IsVisible)
            .ToList();

        var currentIndex = focusableElements.IndexOf(control);
        if (currentIndex >= 0 && currentIndex + 1 < focusableElements.Count)
        {
            focusableElements[currentIndex + 1].Focus();
        }
    }
}
