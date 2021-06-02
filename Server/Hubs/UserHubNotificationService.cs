using Hubs.SignalRChat;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Hubs
{
    public class UserHubNotificationService : IUserHubNotificationService
    {
        private IHubContext<UserHub> _userHubContext;

        public UserHubNotificationService(IHubContext<UserHub> userHubContext)
        {
            _userHubContext = userHubContext;
        }

        /// <summary>
        /// Notify all users that a user has connected
        /// </summary>
        /// <param name="userName"></param>
        public void LogInCompletedNotification(string userName)
        {
            
            _userHubContext.Clients.All.SendAsync("LogInNotificated", userName);
        }
    }
}
