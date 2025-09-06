using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using MedSync.ViewModels;
using MedSync.Views;

namespace MedSync;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
            return null;
        
        var viedName = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.InvariantCulture);
        var type = Type.GetType(viedName);
        
        if (type is null)
            return null;

        var control = (Control)Activator.CreateInstance(type)!;
        control.DataContext = data;
        return control;
    }

    public bool Match(object? data) => data is ViewModelBase;
}