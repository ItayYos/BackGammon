using Client.EventHandling;
using Common;
using DAL.SignalRChat;
using Server.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BL.SignalRChat
{
    public class UserBL : IUserBL
    {
        IDB _db;
        IUserHubNotificationService _userNotificator;

        public UserBL(IDB db, IUserHubNotificationService userNotificator)
        {
            _db = db;
            _userNotificator = userNotificator;
        }


        /// <summary>
        /// Validate that the new user data is valid, and register it. Notify a user was connected after succesfull user creation
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userNotificationMethod"></param>
        /// <returns></returns>
        public string Register(User user)
        {
            if (_db.UserList.Keys.Contains(user.Name))
                return "User exists";
            else if (user.Password.Count() < 4)
                return "Password length must be 4 or more";
            else
            {
                _db.UserList.Add(user.Name, user.Password.GetHashCode());
                //_userNotificator.LogInCompletedNotification(user.Name);
                return string.Empty;
            }
        }
        public string Login(User user)
        {
            KeyValuePair<string, int> result = _db.UserList.FirstOrDefault(u => u.Key == user.Name && u.Value == user.Password.GetHashCode());
            if (result.Key != null)
            {
                return string.Empty;
            }

            if (_db.UserList.Keys.Contains(user.Name))
                return "Wrong password";

            return "Wrong user name";
        }

        public IEnumerable<string> GetAllUsers()
        {
            return _db.UserList.Keys;
        }
    }
}