using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace ListOfCitiesWeatherAPI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region(Fields)
        string API = Properties.Resources.APIKEY;
        string city;
        string url;
        WeatherResults theWeather = new WeatherResults();
        private List<Country> Countries;
        private List<CountryAndCities> CityList;
        public string[] selectedCities { get; set; }
        //ToDo Impliment country code to increase accuracy of result
        string countryCode;
        
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            PopulateCountryBox();
            PopulateCityBox();
        }
        public void PopulateCountryBox()
        {
            string jsonString = File.ReadAllText(".\\countryJSON.json");
           // Debug.WriteLine(jsonString);
            Countries = JsonConvert.DeserializeObject<List<Country>>(jsonString);
            CountryBox.ItemsSource = Countries;
        }
        public void PopulateCityBox()
        {
            string jsonCity = File.ReadAllText(".\\CountryCity.json");
           //Debug.WriteLine(jsonCity);
            CityList = JsonConvert.DeserializeObject<List<CountryAndCities>>(jsonCity);
        }
        public void GetCities()
        {
            
            string currentCountry = CountryBox.SelectedItem.ToString();
            for (int i = 0; i < CityList.Count; i++)
            {
                if (currentCountry == CityList[i].country)
                {
                    selectedCities = CityList[i].cities;
                    break;
                }
            }
            
            //ToDO Setup an index using an algorithm, then write the code to get the list of cities into the cityBox
            // to get the index where we want to start looking for our cities , to speed up the process
           // string lastLetter;
            //string currentLetter;
            //Dictionary<char, int> CountryAlphabetPositions;
           // for (int i = 1; i < length; i++)
            //{
              //      if (index[this] != indexer[last]) { letter(defined by int) increases to next letter} 
            //}
            
        }

      
        private void CountryBox_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateCountryBox();
            PopulateCityBox();
        }

        private void CountryBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cityBox.Items.Clear();
            GetCities();
            foreach (string str in selectedCities)
            {
                cityBox.Items.Add(str);
            }
            Debug.WriteLine("Selection Changed");
            
        }
        private WeatherResults GetWeather()
        {
            string header = Properties.Resources.weatherHeader;
            url = $"https://api.openweathermap.org/data/2.5/weather?q={city},{countryCode}&appid={API}&units=metric";
            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", header);
            IRestResponse response = client.Execute(request);
            var json = response.Content;

            WeatherResults wResults = new WeatherResults();

            wResults = JsonConvert.DeserializeObject<WeatherResults>(json);

            return wResults;
        }
        private void FetchWeather_Click(object sender, RoutedEventArgs e)
        {
            resultBox.Text = "Fetching Weather...";
            city = cityBox.Text;
            //ToDo setup country code
            theWeather = GetWeather();
            if (theWeather.sys != null)
            {
                resultBox.Text = $"The current weather in {city}, {theWeather.sys.country} is: \nThere is {theWeather.weather[0].main} " +
                $"\nIt is {theWeather.main.temp:0.0} C with highs of {theWeather.main.temp_max:0.0} C and lows of {theWeather.main.temp_min:0.0} C";
            }
            else
            {
                resultBox.Text = "There is currently no data for this city.\nPlease try again.";
            }
            
        }
    }
}
