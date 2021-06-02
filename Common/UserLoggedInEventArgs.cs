using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.EventHandling
{
    public class UserLoggedInEventArgs : EventArgs
    {
        public string UserName { get; set; }

        public UserLoggedInEventArgs(string userName) : base()
        {
            UserName = userName;
        }
    }
}
