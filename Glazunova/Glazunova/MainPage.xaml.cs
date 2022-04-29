using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Windows.Input;


namespace Glazunova
{
    public class TypeInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string Generation { get; set; }
        public string Pokemon { get; set; }
    }


    public class RateViewModel : INotifyPropertyChanged
    {
        private string generation;
        private string pokemon;

        public string Generation
        {
            get { return generation; }
            private set
            {
                generation = value.ToString();
                OnPropertyChanged("Generation");
            }
        }

        public string Pokemon
        {
            get { return pokemon; }
            private set
            {
                pokemon = value.ToString();
                OnPropertyChanged("Pokemon");
            }
        }

        public ICommand LoadDataCommand { protected set; get; }

        public RateViewModel()
        {
            this.LoadDataCommand = new Command(LoadData);
        }

        private async void LoadData()
        {
            Random random = new Random();
            string t = random.Next(1, 18).ToString();
            string url = "https://pokeapi.co/api/v2/type/" + t;

            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(url);
                var response = await client.GetAsync(client.BaseAddress);
                response.EnsureSuccessStatusCode(); // выброс исключения, если произошла ошибка

                // десериализация ответа в формате json
                var content = await response.Content.ReadAsStringAsync();
                JObject o = JObject.Parse(content);

                var str = o.SelectToken(@"$.query.results.generation");
                var typeInfo = JsonConvert.DeserializeObject<TypeInfo>(str.ToString());

                this.Generation = typeInfo.Generation;
                this.Pokemon = typeInfo.Pokemon;
            }
            catch (Exception ex)
            { }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }


    public partial class MainPage : ContentPage
    {
        RateViewModel viewModel;
        public MainPage()
        {
            viewModel = new RateViewModel();
            // установка контекста данных
            this.BindingContext = viewModel;
        }

    }
}
