using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAL.SignalRChat
{
    public class DB : IDB
    {
        /// <summary>
        /// List of existing users with password hashcodes
        /// </summary>
        private Dictionary<string, int> userList = new Dictionary<string,int>()
        {
            { "Dolev","123456".GetHashCode() }
        };

        public Dictionary<string, int> UserList
        {
            get
            {
                return userList;
            }
        }
    }
}