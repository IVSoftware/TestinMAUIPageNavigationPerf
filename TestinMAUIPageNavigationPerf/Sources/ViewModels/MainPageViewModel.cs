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
        new("One"),
        new("Two"),
        new("Three"),
        new("Four"),
        new("Five")
      ];
    }
  }
}
