using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace TestinMAUIPageNavigationPerf.Sources.ViewModels
{
  public partial class MainPageViewModel : ObservableObject
  {
    public ItemViewModel SelectedItemViewModel { get; set; }

    [ObservableProperty]
    ObservableCollection<ItemViewModel> _items;

    public MainPageViewModel()
    {
      Items =
      [
        new ItemViewModel{ Title = "One" },
        new ItemViewModel{ Title = "Two" },
        new ItemViewModel{ Title = "Three" },
        new ItemViewModel{ Title = "Four" },
        new ItemViewModel{ Title = "Five" },
      ];
    }
  }
}
