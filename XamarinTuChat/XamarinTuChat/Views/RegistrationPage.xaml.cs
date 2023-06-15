using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinTuChat.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage : ContentPage
    {
        // инициализация клиента и url адреса только для чтения
        private static readonly HttpClient client = new HttpClient();
        private readonly string registrUrl = "http://192.168.33.163:5005/Login/Reg";

        public RegistrationPage()
        {
            InitializeComponent();
            entName.Text = "";
            entSurname.Text = "";
            entLogin.Text = "";
            entNumber.Text = "";
            entPassword.Text = "";
            entPasswordRepair.Text = "";
            entEmail.Text = "";
        }

        // обработчик нажатия на кнопку регистрации
        private async void BtnReg_Clicked(object sender, EventArgs e)
        {
            if (entName.Text.Trim() != null && entSurname.Text.Trim() != null && entLogin.Text.Trim() != null && entPassword.Text.Trim() != null
                && entPasswordRepair.Text.Trim() != null && entEmail.Text.Trim() != null && entNumber.Text.Trim() != null
                && entName.Text.Trim() != "" && entSurname.Text.Trim() != "" && entLogin.Text.Trim() != "" && entPassword.Text.Trim() != ""
                && entPasswordRepair.Text.Trim() != "" && entEmail.Text.Trim() != "" && entNumber.Text.Trim() != "")
            {
                if (entPasswordRepair.Text.Trim() == entPassword.Text.Trim())
                {
                    if (entNumber.Text.Trim().Length == 10)
                    {
                        if (entEmail.Text.Length < 8 || !entEmail.Text.Contains("@") || !entEmail.Text.Contains("."))
                        {
                            await DisplayAlert("Ошибка", "Не верно введен email", "Ok");
                        }
                        else
                        {
                            RegistrationUserData rUD = new RegistrationUserData()
                            {
                                Name = entName.Text,
                                Surname = entSurname.Text,
                                Login = entLogin.Text,
                                Password = entPassword.Text,
                                Email = entEmail.Text,
                                Number = entNumber.Text
                            };

                            // сериализация ранее созданного экземпляра класса
                            string jsonStr = JsonConvert.SerializeObject(rUD);

                            Dictionary<string, string> dict = new Dictionary<string, string>()
                            {
                                { "s", jsonStr }
                            };

                            FormUrlEncodedContent form = new FormUrlEncodedContent(dict);

                            // отправка запроса на сервер по ранее инициализированному url
                            HttpResponseMessage response = await client.PostAsync(registrUrl, form);

                            // получение ответа от сервера в виде строки
                            string result = await response.Content.ReadAsStringAsync();

                            if (result == "0")
                            {
                                await DisplayAlert("Ошибка", "пользователь с таким email'ом уже существует", "Ok");
                            }
                            else if (result == "-1")
                            {
                                await DisplayAlert("Ошибка", "пользователь с таким номером и email'ом уже существует", "Ok");
                            }
                            else if (result == "-2")
                            {
                                await DisplayAlert("Ошибка", "пользователь с таким номером уже существует", "Ok");
                            }
                            else
                            {
                                await DisplayAlert("Good", "все ок", "Ok");
                                await Navigation.PopAsync();
                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Не верно введен номер телефона", "Ok");
                    }
                }
                else
                {
                    await DisplayAlert("Ошибка", "Повтор пароля не совподает", "Ok");
                }
            }
            else
            {
                await DisplayAlert("Ошибка", "Все поля должны быть заполнены", "Ok");
            }
        }
    }
}