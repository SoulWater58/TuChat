using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.LocalNotification;

namespace XamarinTuChat.Services
{
    public class MyBackgroundService
    {
        private readonly HttpClient client;

        string id = Application.Current.Properties["userId"] as string;

        public MyBackgroundService()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://192.168.0.6:56783/api/");
        }

        public async Task DoWorkAsync()
        {
            var response = await client.GetAsync($"GetMessages/{id}");
            //response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
        }
    }
}
