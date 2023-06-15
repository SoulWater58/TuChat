using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TuChatWebService2.Models
{
    public class RegistrationUserData
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Number { get; set; }
    }
}