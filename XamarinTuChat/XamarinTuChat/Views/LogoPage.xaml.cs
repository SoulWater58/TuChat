using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinTuChat.ViewModels;

namespace XamarinTuChat.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogoPage : ContentPage
    {
        string myId = Application.Current.Properties["userId"] as string;

        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "TextMessages.json");

        // экземпляр класса для отправки сообщений
        SendMessageViewModel viewModel;

        // инициализация клиента и url адреса только для чтения
        private static readonly HttpClient client = new HttpClient();
        private readonly string friendNumberUrl = "http://192.168.33.163:5005/Login/FriendNumber";

        // создание словаря нужного для определения экземпляра страницы общения относящегося к нажатой кнопки
        Dictionary<Guid, FriendMessagePage> dictPageAndId = new Dictionary<Guid, FriendMessagePage>();

        // создание словаря нужного по большей степени для удаления повторяющихся номеров телефона
        Dictionary<string, string> strNumberList = new Dictionary<string, string>();
        // словари для присваивания id пользователям
        Dictionary<string, string> strFriendIdList = new Dictionary<string, string>();
        // словарь нужен для присваивания id к экземпляру
        Dictionary<string, FriendMessagePage> strFriendPageList = new Dictionary<string, FriendMessagePage>();

        List<PrivateMessage> ListMessage = new List<PrivateMessage>();
        List<PrivateMessage> ListMessageInDevice = new List<PrivateMessage>();

        public LogoPage()
        {
            InitializeComponent();

            viewModel = new SendMessageViewModel();

            ConnectMe();

            ReceiveMessages();
        }

        // метод создания соединения с сервером
        public async void ConnectMe()
        {
            await viewModel.Connect();
        }

        public async void ReceiveMessages()
        {
            var json = await viewModel.ReceiveMasseges(myId);

            ListMessage = JsonConvert.DeserializeObject<List<PrivateMessage>>(json);

            try
            {
                var jsonMessage = File.ReadAllText(path);

                ListMessageInDevice = JsonConvert.DeserializeObject<List<PrivateMessage>>(jsonMessage);
            }
            catch { }

            // инициализация метода добавления номеров телефона из контактов пользователя
            ContactCollect();
        }

        public void SetFriendPage()
        {
            // передача данных об id и экземпляре страници для общения
            viewModel.SetFriendPageData(strFriendPageList);
        }

        private async void ContactCollect()
        {
            try
            {
                // создание списка контактов
                var cancellationToken = default(CancellationToken);
                var contacts = await Contacts.GetAllAsync(cancellationToken);
                string str = "";
                // создание массива значений которых не должно быть в номерах телефона
                var charsToRemove = new string[] { " ", "-", "+", "*", "#" };

                if (contacts == null)
                {
                    return;
                }

                // прогон списка контактов
                foreach (var contact in contacts)
                {
                    // присваивание переменной str номера контакта
                    str = contact.Phones.FirstOrDefault()?.PhoneNumber ?? string.Empty;

                    // отчистка номера от лишних символов
                    foreach (var c in charsToRemove)
                    {
                        str = str.Replace(c, string.Empty);
                    }

                    // еще одна отчистка номера от лишних символов
                    if (str.Length > 10)
                    {
                        str = str.Remove(0, 1);
                    }

                    // создание экземпляра класса и присваивание ему данных номера контакта
                    FriendNumber fN = new FriendNumber()
                    {
                        Number = str
                    };

                    // сериализация ранее созданного экземпляра класса
                    string jsonStr = JsonConvert.SerializeObject(fN);

                    Dictionary<string, string> dict = new Dictionary<string, string>()
                    {
                        { "s", jsonStr }
                    };

                    FormUrlEncodedContent form = new FormUrlEncodedContent(dict);

                    // отправка запроса на сервер по ранее инициализированному url
                    HttpResponseMessage response = await client.PostAsync(friendNumberUrl, form);

                    // получение ответа от сервера в виде строки
                    string result = await response.Content.ReadAsStringAsync();

                    // здесь на сервере проверяется зарегистрирован ли пользователь с таким номером телефона и если да, то возвращается true
                    if (result != "false")
                    {
                        // благодаря try-catch словарь автоматически отсекает повторяющиеся элементы уходя в catch
                        try
                        {
                            // создание элемента словаря с номером в виде ключа и именем в виде значения
                            strNumberList.Add(str, contact.DisplayName);
                        }
                        catch { }

                        // благодаря try-catch словарь автоматически отсекает повторяющиеся элементы уходя в catch
                        try
                        {
                            // создание элемента словаря с номером в виде ключа и id в виде значения
                            strFriendIdList.Add(str, result);
                        }
                        catch { }
                    }
                }

                // прагон номеров зарегистрированных пользователей в strNumberList
                foreach (KeyValuePair<string, string> strNumber in strNumberList)
                {
                    // создание кнопки с именнем присвоенным данному номеру телефона в контактах пользователя
                    Button btnChat = new Button()
                    {
                        Text = $"{strNumber.Value}",
                        Margin = new Thickness(10, 10),
                        ImageSource = null
                    };

                    // добавление кнопки обработчика нажатия
                    btnChat.Clicked += BtnFriendMessenger_Clicked;

                    // добовление кнопки в список StackPanel созданный через xaml
                    ContactCollection.Children.Add(btnChat);

                    // создание экземпляра страници общения
                    FriendMessagePage friendMessagePage = new FriendMessagePage();

                    // прогон номеров зарегистрированных пользователей в strFriendIdList
                    foreach (KeyValuePair<string, string> strFriend in strFriendIdList)
                    {
                        // проверка на совподение номеров в словарях
                        if (strFriend.Key == strNumber.Key)
                        {
                            // добовление в словарь id в качестве ключа и экземпляр класса в качестве значения
                            strFriendPageList.Add(strFriend.Value, friendMessagePage);

                            // вызывание метода для передачи значений номера телефона и имени на страницу общения
                            friendMessagePage.SetData(strNumber.Value, viewModel, strFriend.Value, ListMessageInDevice, btnChat);

                            if (ListMessageInDevice != null && ListMessageInDevice.Count > 0)
                            {
                                List<PrivateMessageFriend> privateMessages = new List<PrivateMessageFriend>();

                                foreach (var loadMessage in ListMessageInDevice)
                                {
                                    if (loadMessage.Id == strFriend.Value)
                                    {
                                        PrivateMessageFriend privateMessage = new PrivateMessageFriend()
                                        {
                                            UserName = loadMessage.UserName,
                                            Message = loadMessage.Message
                                        };

                                        privateMessages.Add(privateMessage);
                                    }
                                }

                                friendMessagePage.GetSaveMessages(privateMessages);
                            }

                            if (ListMessage != null && ListMessage.Count > 0)
                            {
                                List<PrivateMessageFriend> privateMessages = new List<PrivateMessageFriend>();

                                foreach (var sendMessages in ListMessage)
                                {
                                    if (sendMessages.Id == strFriend.Value)
                                    {
                                        btnChat.ImageSource = "receiveButton.png";

                                        PrivateMessageFriend privateMessage = new PrivateMessageFriend()
                                        {
                                            UserName = sendMessages.UserName,
                                            Message = sendMessages.Message
                                        };

                                        privateMessages.Add(privateMessage);
                                    }
                                }

                                friendMessagePage.GetMessages(privateMessages);
                            }
                        }
                    }

                    // добовление в словарь id кнопки и экземпляр страници общения относящийся к этой кнопке
                    dictPageAndId.Add(btnChat.Id, friendMessagePage);
                }

                SetFriendPage();
            }
            catch
            {
                await DisplayAlert("Ошибка", "Непредвиденная ошибка. Попробуйте зайти в параметры и дать разрешение приложению на использование контактов и памяти", "Ok");
            }
        }

        // обработчик нажатия на кнопку с именем контакта
        private async void BtnFriendMessenger_Clicked(object sender, EventArgs e)
        {
            // создание экземпляра нажатой кнопки
            Button btn = (Button)sender;

            btn.ImageSource = null;

            // переход на ранее созданный экземпляр страницы относящийся к нажатой кнопке
            await Navigation.PushAsync(dictPageAndId[btn.Id]);
        }

        // обработчик нажатия на кнопку регистрации
        private async void BtnReg_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegistrationPage());
        }

        // обработчик нажатия на кнопку авторизации
        private async void BtnAut_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AuthorizationPage());
        }
    }
}