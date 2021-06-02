using BL.SignalRChat;
using Common;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hubs.SignalRChat
{
    public class UserHub : Hub
    {
        IUserBL _bl;

        public UserHub(IUserBL bl)
        {
            _bl = bl;
        }        

        /// <summary>
        /// Register a user
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Register(User name)
        {
            return _bl.Register(name);
        }

        public string Login(User user)
        {
            return _bl.Login(user);
        }
        /// <summary>
        /// Get all the existing users
        /// </summary>
        public IEnumerable<string> GetAllUsers()
        {
            return _bl.GetAllUsers();
        }
    }
}