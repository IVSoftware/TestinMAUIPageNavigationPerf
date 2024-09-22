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
                await Shell.Current.GoToAsync(nameof(SelectPage));
            }
            catch (Exception e)
            {
                Debug.Fail(e.Message);
            }
        }

        public ItemViewModel(string title)
        {
            Title = title;
        }
    }
}
