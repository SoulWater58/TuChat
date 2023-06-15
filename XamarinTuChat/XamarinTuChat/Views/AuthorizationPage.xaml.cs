using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft;
using Newtonsoft.Json;

namespace XamarinTuChat.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthorizationPage : ContentPage
    {
        // инициализация клиента и url адреса только для чтения
        private static readonly HttpClient client = new HttpClient();
        private readonly string loginUrl = "http://192.168.33.163:5005/Login/Auth";
        private readonly string passUrl = "http://192.168.33.163:5005/Login/RememberPassword";

        public AuthorizationPage()
        {
            InitializeComponent();
            LogBox.Text = "";
            PassBox.Text = "";
        }

        // обработчик нажатия на кнопку авторизации
        private async void BtnLog_Clicked(object sender, EventArgs e)
        {
            if (LogBox.Text != null && LogBox.Text != "")
            {
                LogBox.BackgroundColor = Color.Transparent;

                if (PassBox.Text != null && PassBox.Text != "")
                {
                    PassBox.BackgroundColor = Color.Transparent;

                    // создание экземпляра класса и присваивание ему данных логина и пароля пользователя
                    DigitPair dP = new DigitPair()
                    {
                        Number = LogBox.Text,
                        Password = PassBox.Text
                    };

                    // сериализация ранее созданного экземпляра класса
                    string jsonStr = JsonConvert.SerializeObject(dP);

                    Dictionary<string, string> dict = new Dictionary<string, string>()
                    {
                        { "s", jsonStr }
                    };

                    FormUrlEncodedContent form = new FormUrlEncodedContent(dict);

                    // отправка запроса на сервер по ранее инициализированному url
                    HttpResponseMessage response = await client.PostAsync(loginUrl, form);

                    // получение ответа от сервера в виде строки
                    string result = await response.Content.ReadAsStringAsync();

                    if (result == "-1")
                    {
                        await DisplayAlert("Ошибка", "Неверный пароль", "Ok");
                    }
                    else if (result == "0")
                    {
                        await DisplayAlert("Ошибка", "Такого логина не существует", "Ok");
                    }
                    else
                    {
                        // десериализация ответа от сервера
                        UserData uD = JsonConvert.DeserializeObject<UserData>(result);
                        await DisplayAlert("Успешно", "Успешная авторизация", "Ok");
                        AppShell appShell = new AppShell();
                        // добовление id пользователя в глобальный словарь
                        Application.Current.Properties["userId"] = uD.Id;
                        // добовление имени пользователя в глобальный словарь
                        Application.Current.Properties["username"] = uD.Username;
                        // задание экземпляра класса AppShell как главной страница
                        Application.Current.MainPage = appShell;
                        appShell.GetData(uD.Id, uD.Username);
                    }
                }
                else
                {
                    PassBox.BackgroundColor = Color.Red;
                }
            }
            else
            {
                LogBox.BackgroundColor = Color.Red;

                if (PassBox.Text != null && PassBox.Text != "")
                {
                    PassBox.BackgroundColor = Color.Transparent;
                }
                else
                {
                    PassBox.BackgroundColor = Color.Red;
                }
            }
        }

        // обработчик нажатия на кнопку забыли пароль
        private async void BtnPass_Clicked(object sender, EventArgs e)
        {
            if (LogBox.Text != null && LogBox.Text != "")
            {
                LogBox.BackgroundColor = Color.Transparent;

                Dictionary<string, string> dict = new Dictionary<string, string>()
                {
                    { "s", LogBox.Text }
                };

                FormUrlEncodedContent form = new FormUrlEncodedContent(dict);

                HttpResponseMessage response = await client.PostAsync(passUrl, form);

                string result = await response.Content.ReadAsStringAsync();

                if (result == "0")
                {
                    await DisplayAlert("Ошибка", "Такого логина не существует", "Ok");
                }
                else if (result == "-1")
                {
                    await DisplayAlert("Ошибка", "Непредвиденная ошибка. Попробуйте еще раз", "Ok");
                }
                else
                {
                    await DisplayAlert("Успешно", $"На вашу почту {result} было отправленно сообщение с вашим паролем", "Ok");
                }
            }
            else
            {
                LogBox.BackgroundColor = Color.Red;
            }
        }

        // обработчик нажатия на кнопку регистрация
        private async void BtnReg_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegistrationPage());
        }
    }
}