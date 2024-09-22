using Microsoft.Extensions.Logging;
using TestinMAUIPageNavigationPerf.Sources.ViewModels;
using TestinMAUIPageNavigationPerf.Sources.Views;

namespace TestinMAUIPageNavigationPerf
{
  public static class MauiProgram
  {
    private static IServiceProvider _services;
    public static IServiceProvider Services => _services;

    private static MainPageViewModel? _mainPage;
    public static MainPageViewModel? MainPage
      => _mainPage ??= Services?.GetService<MainPageViewModel>();

    public static MauiApp CreateMauiApp()
    {
      var builder = MauiApp.CreateBuilder();
      builder
        .UseMauiApp<App>()
        .ConfigureFonts(fonts =>
        {
          fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
          fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });

#if DEBUG
      builder.Logging.AddDebug();
#endif
      //builder.Services.AddSingleton<MainPageViewModel>();
      //builder.Services.AddSingleton<MainPage>();
      //builder.Services.AddSingleton<SelectPage>();
      var app = builder.Build();
      _services = app.Services;
      return app;
    }
  }
}
