using System.Collections.Generic;

namespace DAL.SignalRChat
{
    public interface IDB
    {
        Dictionary<string, int> UserList { get; }
    }
}