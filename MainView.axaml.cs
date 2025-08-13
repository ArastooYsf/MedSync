using Avalonia.Controls;
using Avalonia.Input;
using MedSync.ViewModels;

namespace MedSync;

public partial class MainView : Window
{
    public MainView()
    {
        InitializeComponent();
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount != 1)
            return;

        (DataContext as MainViewModel)?.SidePanelResizeCommand?.Execute(null);

    }
}