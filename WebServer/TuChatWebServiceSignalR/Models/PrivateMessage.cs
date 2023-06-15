using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TuChatWebServiceSignalR.Models
{
    public class PrivateMessage
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
    }
}
