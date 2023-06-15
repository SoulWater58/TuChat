using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TuChatWebServiceSignalR.Models;

namespace TuChatWebServiceSignalR.Hubs
{
    public class ChatHub : Hub
    {
        //private readonly TuChatDBContext tuChatDB;

        static List<UserDetail> ConnectedUsers = new List<UserDetail>();

        public static List<SendMessagesOfUsers> ListMessage = new List<SendMessagesOfUsers>();

        public void Connect(string UserName, int UserID)
        {
            var id = Context.ConnectionId;

            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            {
                ConnectedUsers.Add(new UserDetail { ConnectionId = id, UserName = $"{UserName} - {UserID}", UserID = UserID });
            }
        }

        public async Task SendPrivateMessage(string toUserId, string message)
        {
            try
            {
                int _fromUserId = ConnectedUsers.Where(u => u.ConnectionId == Context.ConnectionId).Select(U => U.UserID).FirstOrDefault();

                int _toUserId = Convert.ToInt32(toUserId);

                string userName = "";

                UserDetail FromUsers = ConnectedUsers.Where(u => u.UserID == _fromUserId).FirstOrDefault();
                UserDetail ToUsers = ConnectedUsers.Where(x => x.UserID == _toUserId).FirstOrDefault();
                
                if (FromUsers != null)
                {
                    userName = FromUsers.UserName;
                    await Clients.Client(FromUsers.ConnectionId).SendAsync("sendPrivateMessage", toUserId, FromUsers.UserName, message);

                    if (ToUsers != null)
                    {
                        await Clients.Client(ToUsers.ConnectionId).SendAsync("sendPrivateMessage", _fromUserId.ToString(), FromUsers.UserName, message);
                    }
                    else
                    {
                        SendMessagesOfUsers sendMessages = new SendMessagesOfUsers()
                        {
                            FromId = _fromUserId.ToString(),
                            ToId = toUserId,
                            UserName = userName,
                            Message = message
                        };

                        ListMessage.Add(sendMessages);

                        // здесь должен быть код отправки push уведомления

                        //var serverKey = "AAAAZgj6XQg:APA91bE9UEtrFzmDGY1QeKGxUU0k4trfP4YeD72IuM_0kt4EeB2Txqh46fc9HpGawpFRBzxHCC4JKm99BcaJFaIwbNlZRtuYMiih1GzUBwZuflF_FvD0N4D9YzPz7hL9bYYrV9dA8PfN";

                        //var messageCloud = new
                        //{
                        //    to = "<FCM registration token получателя>",
                        //    notification = new
                        //    {
                        //        title = "Новое сообщение",
                        //        body = $"{userName}: {message}"
                        //    },
                        //    data = new
                        //    {
                        //        fromUserId = _fromUserId.ToString(),
                        //        toUserId = toUserId,
                        //        userName = userName,
                        //        message = message
                        //    }
                        //};

                        //var json = JsonConvert.SerializeObject(messageCloud);
                        //var request = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send");
                        //request.Headers.TryAddWithoutValidation("Authorization", $"key={serverKey}");
                        //request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                        //using (var client = new HttpClient())
                        //{
                        //    var response = await client.SendAsync(request);
                        //    response.EnsureSuccessStatusCode();
                        //}
                    }
                }
            }
            catch { }
        }

        public string RequestMessages(string toUserId)
        {
            List<PrivateMessage> privateMessages = new List<PrivateMessage>();
            List<SendMessagesOfUsers> messageDelete = new List<SendMessagesOfUsers>();

            if (ListMessage.Count > 0)
            {
                foreach (var sendMessages in ListMessage)
                {
                    if (sendMessages.ToId == toUserId)
                    {
                        PrivateMessage privateMessage = new PrivateMessage()
                        {
                            Id = sendMessages.FromId,
                            UserName = sendMessages.UserName,
                            Message = sendMessages.Message
                        };

                        privateMessages.Add(privateMessage);

                        messageDelete.Add(sendMessages);
                    }
                }

                foreach (var message in messageDelete)
                {
                    ListMessage.Remove(message);
                }
            }

            var json = JsonConvert.SerializeObject(privateMessages);

            return json;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            ConnectedUsers.Remove(ConnectedUsers.Where(b => b.ConnectionId == Context.ConnectionId).FirstOrDefault());
            await base.OnDisconnectedAsync(exception);
        }
    }
}
