using System;
using System.Collections.Generic;
using Client.EventHandling;
using Common;

namespace BL.SignalRChat
{
    public interface IUserBL
    {
        IEnumerable<string> GetAllUsers();
        string Register(User user);

        string Login(User user);
    }
}