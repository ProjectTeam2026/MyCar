
using Microsoft.Extensions.Logging;
using Yandex.MapKit.Maui;

namespace MyCar
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
       
            string apiKey = "a19246a7-fdeb-4cdd-abf5-b2c682ecc7c6";

            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseYandexMapKit(apiKey, locale: "ru_RU")
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Anticva-Regular.otf", "AnticvaRegular");
                    fonts.AddFont("AA_Stetica_Light.otf", "AASteticaLight");
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

