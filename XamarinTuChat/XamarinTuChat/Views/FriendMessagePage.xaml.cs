using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinTuChat.ViewModels;

namespace XamarinTuChat.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FriendMessagePage : ContentPage
    {
        string idFriend;

        SendMessageViewModel viewModel;

        Button btnFriend;

        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "TextMessages.json");

        List<PrivateMessage> ListMessegeInDevice = new List<PrivateMessage>();

        public FriendMessagePage()
        {
            InitializeComponent();

            enSend.Text = "";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(500);

                ScrollDown();
            });
        }

        private async void ScrollDown()
        {
            var lastLabel = CollectionMessage.Children.LastOrDefault() as Label;
            if (lastLabel != null)
            {
                await scrollEl.ScrollToAsync(lastLabel, ScrollToPosition.End, true);
            }
        }

        public void SaveMessages(string userName, string message)
        {
            ListMessegeInDevice.Add(new PrivateMessage { Id = idFriend, UserName = userName, Message = message });

            var json = JsonConvert.SerializeObject(ListMessegeInDevice);

            File.WriteAllText(path, json);
        }

        // метод получения данных из другого класса
        public void SetData(string name, SendMessageViewModel viewModel1, string id, List<PrivateMessage> privateMessages, Button btn)
        {
            btnFriend = btn;
            ListMessegeInDevice = privateMessages;
            idFriend = id;
            viewModel = viewModel1;
            lblChatName.Text = name;
        }

        public void GetMessages(List<PrivateMessageFriend> privateMessages)
        {
            foreach (var messages in privateMessages)
            {
                SetMessage(messages.UserName, messages.Message);
            }
        }

        public void GetSaveMessages(List<PrivateMessageFriend> privateMessages)
        {
            foreach (var messages in privateMessages)
            {
                Label lbl = new Label();
                lbl.Text = $"{messages.UserName}: {messages.Message}";

                CollectionMessage.Children.Add(lbl);
            }
        }

        // метод вывода сообщения и отправителя
        public void SetMessage(string userName, string message)
        {
            Label lbl = new Label();
            lbl.Text = $"{userName}: {message}";

            CollectionMessage.Children.Add(lbl);

            SaveMessages(userName, message);

            ScrollDown();

            bool isPageVisible = Navigation.NavigationStack.Contains(this);

            if (isPageVisible == false)
            {
                btnFriend.ImageSource = "receiveButton.png";
            }
        }

        // метод обработки нажатия на кнопку отправки сообщения
        private async void BtnSendMessage_Clicked(object sender, EventArgs e)
        {
            // проверка пустует ли поля сообщения или нет
            if (enSend.Text.Trim() != "" && enSend.Text.Trim() != null)
            {
                // вызов метода отправки сообщения
                await viewModel.SendPrivateMessage(idFriend, enSend.Text);
            }
        }

        private void BtnClearMessage_Clicked(object sender, EventArgs e)
        {
            enSend.Text = "";
        }
    }
}