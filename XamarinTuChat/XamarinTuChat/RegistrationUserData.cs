using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinTuChat
{
    [Serializable]
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
