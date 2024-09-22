using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using TestinMAUIPageNavigationPerf.Sources.Views;

namespace TestinMAUIPageNavigationPerf.Sources.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        public static ItemViewModel? SelectedItemViewModel { get; set; }

        [ObservableProperty]
        ObservableCollection<ItemViewModel> _items;

        [RelayCommand]
        private async Task SelectItem(ItemViewModel item)
        {
            SelectedItemViewModel = item;
            try
            {
                await Shell.Current.GoToAsync(nameof(SelectPage));
            }
            catch (Exception e)
            {
                Debug.Fail(e.Message);
            }
        }

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
