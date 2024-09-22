using TestinMAUIPageNavigationPerf.Sources.Views;

namespace TestinMAUIPageNavigationPerf
{
  public partial class AppShell : Shell
  {

    public AppShell()
    {
      InitializeComponent();

      Routing.RegisterRoute($"{nameof(MainPage)}/{nameof(SelectPage)}", typeof(SelectPage));
    }
  }
}
