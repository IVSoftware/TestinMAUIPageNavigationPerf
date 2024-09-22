using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using TestinMAUIPageNavigationPerf.Sources.Views;

namespace TestinMAUIPageNavigationPerf.Sources.ViewModels
{
    public partial class ItemViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        IList<int> _numbers = Enumerable.Range(1, 20).ToList();

        [RelayCommand]
        private async Task SelectItem()
        {
            if (MauiProgram.MainPage != default)
                MauiProgram.MainPage.SelectedItemViewModel = this;
            try
            {
                var stopwatch = Stopwatch.StartNew();
                await Shell.Current.GoToAsync(nameof(SelectPage));
                stopwatch.Stop();
                Debug.WriteLine($"{nameof(SelectItem)} Elapsed: {stopwatch.Elapsed}");
            }
            catch (Exception e)
            {
                Debug.Fail(e.Message);
            }
        }
    }
}
