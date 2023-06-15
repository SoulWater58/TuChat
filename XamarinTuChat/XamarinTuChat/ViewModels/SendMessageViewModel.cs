using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinTuChat.Views;
namespace XamarinTuChat.ViewModels
{
    public class SendMessageViewModel
    {
        HubConnection hubConnection;

        public string userName = Application.Current.Properties["username"] as string;
        public int userId = Convert.ToInt32(Application.Current.Properties["userId"]);

        public List<UserDetail> ConnectedUsers = new List<UserDetail>();

        Dictionary<string, FriendMessagePage> strFriendPageList = new Dictionary<string, FriendMessagePage>();

        public SendMessageViewModel()
        {
            // Подключение хаба по указонному адресу
            hubConnection = new HubConnectionBuilder().WithUrl("http://192.168.33.163:56783/chat").Build();

            // по умолчянию не подключены
            IsConnected = false;

            // настройка закрытия подключения по разным причинам
            hubConnection.Closed += async (error) =>
            {
                IsConnected = false;
                await Task.Delay(5000);
                await Connect();
            };

            // настройка приема приватных сообщений
            hubConnection.On<string, string, string>("sendPrivateMessage", (userId, userName, message) =>
            {
                foreach (KeyValuePair<string, FriendMessagePage> strFriendPage in strFriendPageList)
                {
                    if (strFriendPage.Key == userId)
                    {
                        SendSetMessage(userName, message, strFriendPage.Value);
                    }
                }
            });
        }

        public async Task<string> ReceiveMasseges(string toUserId)
        {
            var privateMessages = await hubConnection.InvokeAsync<string>("RequestMessages", toUserId);

            return privateMessages;
        }

        public async Task SendSetMessage(string userName, string message, FriendMessagePage friendMessagePage)
        {
            await Task.Run(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    friendMessagePage.SetMessage(userName, message);
                });
            });
        }

        public void SetFriendPageData(Dictionary<string, FriendMessagePage> dict)
        {
            strFriendPageList = dict;
        }

        // осуществленно ли подключение
        bool isConnected;
        public bool IsConnected
        {
            get => isConnected;
            set
            {
                if (isConnected != value)
                {
                    isConnected = value;
                    OnPropertyChanged("IsConnected");
                }
            }
        }

        // метод отправки личных сообщений
        public async Task SendPrivateMessage(string toUserId, string message)
        {
            try
            {
                // на хабе вызывается метод SendPrivateMessage
                await hubConnection.InvokeAsync("SendPrivateMessage", toUserId, message);
            }
            catch (Exception ex)
            {
            }
        }

        // подключение к чату
        public async Task Connect()
        {
            if (IsConnected)
            {
                return;
            }
            try
            {
                await hubConnection.StartAsync();

                // на хабе вызывается метод Connect
                await hubConnection.InvokeAsync("Connect", userName, userId);

                IsConnected = true;
            }
            catch (Exception ex)
            {
            }
        }

        // отключение от чата
        public async Task Disconnect()
        {
            if (!IsConnected)
            {
                return;
            }

            await hubConnection.StopAsync();
            IsConnected = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
