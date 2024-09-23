using TestinMAUIPageNavigationPerf.Sources.Views;

namespace TestinMAUIPageNavigationPerf
{
#if MAKE_SINGLETON_IN_CODE
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute($"{nameof(MainPage)}/{nameof(SelectPage)}", new SelectPageRouteFactory());
        }
        public static TimeSpan TestInterval = TimeSpan.FromSeconds(5);
    }
    class SelectPageRouteFactory : RouteFactory
    {
        public override Element GetOrCreate() => SelectPage;

        public override Element GetOrCreate(IServiceProvider services) => SelectPage;
        public SelectPage SelectPage
        {
            get
            {
                if (_selectPage is null)
                {
                    _selectPage = new SelectPage();
                }
                return _selectPage;
            }
        }
        SelectPage? _selectPage = default;
    }
#else
    public partial class AppShell : Shell
    {

        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute($"{nameof(MainPage)}/{nameof(SelectPage)}", typeof(SelectPage));
        }
        public static TimeSpan TestInterval = TimeSpan.FromSeconds(0.5);
    }
#endif
}
