using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace hopins
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class myProduct : Page
    {
        public myProduct()
        {
            this.InitializeComponent();
            
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        public class RootObject
        {
            public int id { get; set; }
            public string nama { get; set; }
            public string harga { get; set; }
            public string deskripsi { get; set; }
            public string gambar { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }

    


        public async void yeah()
        {
            //var client = new HttpClient();
            //HttpResponseMessage response =
            //await client.GetAsync(new Uri("http:/http://hopins16.azurewebsites.net/hopins/public/getorder"));
            //client.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
            //var json = await response.Content.ReadAsStringAsync();
            //var result = JsonConvert.DeserializeObject<RootObject>(json);
            //foreach (KeyValuePair<string, nama> kvp in result.nama)
            //{
            //    productName.Text = string.Format(kvp.Key + " id: " + kvp.Value.Id);
            //    textBlock.Text = string.Format(kvp.Key + " name: " + kvp.Value.DisplayName);
            //}

            //productName.Text = result.nama;

        }



    }
}
