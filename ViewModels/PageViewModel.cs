using CommunityToolkit.Mvvm.ComponentModel;
using MedSync.Data;

namespace MedSync.ViewModels;

public partial class PageViewModel : ViewModelBase
{
    [ObservableProperty]
    public ApplicationPageNames _pageName;
}