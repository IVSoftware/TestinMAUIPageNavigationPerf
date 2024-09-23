using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http.Headers;
using TestinMAUIPageNavigationPerf.Sources.Views;

namespace TestinMAUIPageNavigationPerf.Sources.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        public static ItemViewModel? SelectedItemViewModel { get; set; }

        [RelayCommand]
        private async Task SelectItem(ItemViewModel item)
        {
            SelectedItemViewModel = item;
            try
            {
                if (App.Current?.MainPage?.Handler != null)
                {
#if WINDOWS
                    retry:
                    int tries = 1;
                    try
                    {
                        await Shell.Current.GoToAsync(nameof(SelectPage));
                    }
                    catch(System.Runtime.InteropServices.COMException ex) 
                    {
                        if(tries == 1) 
                            Debug.WriteLine($"{ex.GetType().Name}{Environment.NewLine}{ex.Message}");
                        if (tries++ < 5)
                        {
                            goto retry;
                        }
                        else throw new AggregateException(ex);
                    }
#else
                    try
                    {
                        await Shell.Current.GoToAsync(nameof(SelectPage));
                    }
                    catch(Exception ex) 
                    {
                        Debug.WriteLine($"{ex.GetType().Name} is unexpected on this platform.");
                    }
#endif
                }
            }
            catch (Exception e)
            {
                Debug.Fail(e.Message);
            }
        }
        public ItemViewModel[] Items { get; } = new[]
        {
            new ItemViewModel{ Title = "One" },
            new ItemViewModel{ Title = "Two" },
            new ItemViewModel{ Title = "Three" },
            new ItemViewModel{ Title = "Four" },
            new ItemViewModel{ Title = "Five" },
        };
    }
}
