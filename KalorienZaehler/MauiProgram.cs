using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using Plugin.Maui.Biometric;

namespace KalorienZaehler;

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
			});
		// Register the Biometric service
		builder.Services.AddSingleton<IBiometric>(BiometricAuthenticationService.Default);
builder.Services.AddSingleton(AudioManager.Current);
builder.Services.AddTransient<MainPage>();
#if DEBUG
		builder.Logging.AddDebug();
#endif       

		return builder.Build();
	}
}
