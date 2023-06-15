using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TuChatWebServiceSignalR.Hubs;
using TuChatWebServiceSignalR.Models;

namespace TuChatWebServiceSignalR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        List<SendMessagesOfUsers> ListMessage = ChatHub.ListMessage;

        [HttpGet]
        public int GetMessages(string id)
        {
            int num = 0;

            if (ListMessage.Count > 0)
            {
                foreach (var sendMessages in ListMessage)
                {
                    if (sendMessages.ToId == id)
                    {
                        num++;
                    }
                }
            }

            return num;
        }
    }
}
