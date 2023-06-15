using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinTuChat
{
    [Serializable]
    public class PrivateMessage
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
    }
}
