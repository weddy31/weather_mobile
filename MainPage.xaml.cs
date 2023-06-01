using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;

namespace App1
{
    public partial class MainPage : ContentPage
    {
        private List<WeatherData> data;
        private List<string> nazwaStacji;
        private List<double> temperatura;
        private List<double> cisnienie;

        public MainPage()
        {
            InitializeComponent();
            SetupUI();
        }

        private async void SetupUI()
        {
            data = await FetchData();
            nazwaStacji = new List<string>();
            temperatura = new List<double>();
            cisnienie = new List<double>();

            foreach (var item in data)
            {
                nazwaStacji.Add(item.stacja);
            }

            label.Text = "Output:";

            comboBox.ItemsSource = nazwaStacji;

            label_2.Text = "Wybierz miasto:";

            button.Text = "Wcisnij, aby zobaczyć pogodę";
            button.Clicked += OutputData;
        }

        private async Task<List<WeatherData>> FetchData()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("http://10.0.2.2:8080/data");
            var json = await response.Content.ReadAsStringAsync();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            return JsonConvert.DeserializeObject<List<WeatherData>>(json, settings);
        }

        private void OutputData(object sender, EventArgs e)
        {
            var selectedCity = comboBox.SelectedItem.ToString();
            temperatura.Clear();
            cisnienie.Clear();

            foreach (var item in data)
            {
                if (item.stacja == selectedCity)
                {
                    temperatura.Add(item.temperatura);
                    cisnienie.Add(item.cisnienie);
                    break;
                }
            }

            var temperaturaStr = string.Join(", ", temperatura);
            var cisnienieStr = string.Join(", ", cisnienie);

            label.Text = selectedCity + "\n" + $"temperatura: {temperaturaStr}°C" + "\n" + $"cisnienie: {cisnienieStr} hPa";
        }
    }

    public class WeatherData
    {
        public string stacja { get; set; }
        public double temperatura { get; set; }
        public double cisnienie { get; set; }
    }
}
