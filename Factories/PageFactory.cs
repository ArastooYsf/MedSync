using System;
using MedSync.Data;
using MedSync.ViewModels;

namespace MedSync.Factories;

public class PageFactory(Func<ApplicationPageNames, PageViewModel> factory)
{
    public PageViewModel GetPageViewModel(ApplicationPageNames pageName) => factory.Invoke(pageName);
    
}