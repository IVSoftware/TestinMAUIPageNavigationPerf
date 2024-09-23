using System.Diagnostics;
using TestinMAUIPageNavigationPerf.Sources.ViewModels;

namespace TestinMAUIPageNavigationPerf.Sources.Views
{
    public partial class MainPage : ContentPage
    {
#if SELF_TEST
        private static uint _instanceCounter = 0;
        public MainPage()
        {
            _instanceCounter++;
            Debug.Assert(_instanceCounter <= 1, "Expecting only one instance of this class.");
            InitializeComponent();
        }
        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
                await Task.Delay(AppShell.TestInterval);
                // After for optional 'long' setup interval for the first
                // iteration, followed by test at smaller increments.
                AppShell.TestInterval = TimeSpan.FromSeconds(1);
                Debug.WriteLine($"Count = {_debugCount++}");
                var randoBC = BindingContext.Items[_rando.Next(BindingContext.Items.Length)];
                BindingContext.SelectItemCommand.Execute(randoBC);
        }
        int _debugCount = 1;
        Random _rando = new Random(Seed: 1);
#else 
        public MainPage() => InitializeComponent();
#endif
       
        new MainPageViewModel BindingContext =>(MainPageViewModel)base.BindingContext;
    }
}
