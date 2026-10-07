using Microsoft.Extensions.Logging;

namespace MyCar
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Anticva-Regular.otf", "AnticvaRegular");
                    fonts.AddFont("AA_Stetica_Light.otf", "AA_SteticaLight");
                    fonts.AddFont("Vetrino.otf", "Vetrino");
                    fonts.AddFont("NexaText-Light.otf", "NexaTextLight");
                });

#if ANDROID
            Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("Autoplay", (handler, view) =>
            {
                handler.PlatformView.Settings.MediaPlaybackRequiresUserGesture = false;
            });
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
