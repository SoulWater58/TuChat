using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TuChatWebService2.Models
{
    [Serializable]
    public class UserData
    {
        public string Id { get; set; }
        public string Username { get; set; }
    }
}