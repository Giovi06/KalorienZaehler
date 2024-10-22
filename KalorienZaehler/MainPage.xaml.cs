using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using Plugin.Maui.Audio;

namespace KalorienZaehler
{
	public partial class MainPage : ContentPage
	{
		int calories = 0;
		DateTime currentDate;
		private readonly IAudioManager audioManager;

		public MainPage(IAudioManager audioManager)
		{
			InitializeComponent();
			LoadCalories(); // Lädt die Kalorien beim Start der App

			this.audioManager = audioManager;
		}

		// Lädt die gespeicherten Kalorien
		void LoadCalories()
		{
			// Holen das gespeicherte Datum und Kalorien aus Preferences
			string savedDataJson = Preferences.Get("DailyCalories", string.Empty);
			if (!string.IsNullOrEmpty(savedDataJson))
			{
				// Gespeicherte Daten als Liste von Tagen laden
				var savedData = JsonConvert.DeserializeObject<List<DailyCalories>>(savedDataJson);

				// Prüfen, ob es Daten für heute gibt
				var todayData = savedData?.Find(d => d.Date.Date == DateTime.Now.Date);
				if (todayData != null)
				{
					calories = todayData.Calories;
				}
				else
				{
					// Wenn es keine Daten für heute gibt, werden die Kalorien zurückgesetzt
					calories = 0;
				}

				// Aktualisiere das Label für Kalorien
				CaloriesLabel.Text = calories.ToString();
			}

			// Speichere das aktuelle Datum
			currentDate = DateTime.Now;
		}

		// Speichert die Kalorien beim Schließen der App oder beim Wechsel des Tages
		void SaveCalories()
		{
			// Lade die gespeicherten Daten
			string savedDataJson = Preferences.Get("DailyCalories", string.Empty);
			var savedData = string.IsNullOrEmpty(savedDataJson) ? new List<DailyCalories>() : JsonConvert.DeserializeObject<List<DailyCalories>>(savedDataJson);

			// Daten für das aktuelle Datum hinzufügen oder aktualisieren
			var todayData = savedData.Find(d => d.Date.Date == currentDate.Date);
			if (todayData != null)
			{
				todayData.Calories = calories; // Aktualisiere Kalorien für den aktuellen Tag
			}
			else
			{
				savedData.Add(new DailyCalories { Date = currentDate.Date, Calories = calories }); // Neue Daten für heute hinzufügen
			}

			// Speichere die aktualisierten Daten als JSON in Preferences
			Preferences.Set("DailyCalories", JsonConvert.SerializeObject(savedData));
		}

		// Wenn der Plus-Button gedrückt wird
		void OnAddCaloriesClicked(object sender, EventArgs e)
		{
			if (int.TryParse(CaloriesEntry.Text, out int addedCalories))
			{
				calories += addedCalories;
				CaloriesLabel.Text = calories.ToString();
				SaveCalories();
			}
		}

		// Wenn der Minus-Button gedrückt wird
		void OnSubtractCaloriesClicked(object sender, EventArgs e)
		{
			if (int.TryParse(CaloriesEntry.Text, out int subtractedCalories))
			{
				calories -= subtractedCalories;
				CaloriesLabel.Text = calories.ToString();
				SaveCalories();
			}
		}

		// Methode zum Abschließen des Tages
		async void OnEndDayClicked(object sender, EventArgs e)
		{
			var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("YeahBuddy.mp3"));

			player.Play();
			player.Dispose();

			SaveCalories();

			// Setze die Kalorien auf 0 für den nächsten Tag
			calories = 0;
			CaloriesLabel.Text = calories.ToString();

			await DisplayAlert("Tag abgeschlossen",
			 "Die Kalorien für heute wurden gespeichert und der Zähler auf 0 zurückgesetzt.", "OK");
		}

		// Wenn die App geschlossen wird, speichere die Kalorien
		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			SaveCalories();
		}

		// Wenn das Datum wechselt (z.B. am nächsten Tag)
		protected override void OnAppearing()
		{
			base.OnAppearing();

			// Prüfen, ob das Datum sich geändert hat
			if (currentDate.Date != DateTime.Now.Date)
			{
				SaveCalories();

				// Setze die Kalorien für den neuen Tag auf 0
				calories = 0;
				currentDate = DateTime.Now;
				CaloriesLabel.Text = calories.ToString();
			}
		}
		void OnShowHistoryClicked(object sender, EventArgs e)
		{
			// Führe haptisches Feedback aus
			HapticFeedback.Default.Perform(HapticFeedbackType.Click);
			// Navigiere zur HistoryPage
			Navigation.PushAsync(new HistoryPage());
		}

	}
}
