using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KalorienZaehler
{
    public partial class HistoryPage : ContentPage
    {
        public HistoryPage()
        {
            InitializeComponent();
            LoadHistory(); // Lade die Historie beim Laden der Seite
        }

        void LoadHistory()
        {
            // Lade die gespeicherten Kalorien-Daten aus Preferences
            string savedDataJson = Preferences.Get("DailyCalories", string.Empty);
            if (!string.IsNullOrEmpty(savedDataJson))
            {
                // Deserialisiere die gespeicherten Daten in eine Liste von DailyCalories
                var savedData = JsonConvert.DeserializeObject<List<DailyCalories>>(savedDataJson);

                // Sortiere die Liste nach Datum (absteigend, neueste oben)
                var sortedData = savedData?.OrderByDescending(d => d.Date).ToList();

                // Weise die sortierten Daten der ListView zu
                HistoryListView.ItemsSource = sortedData;
            }
        }
    }
}
