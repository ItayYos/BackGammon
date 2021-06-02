using BL.SignalRChat;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;



namespace Server.Hubs
{
    public class ChatHub: Hub
    {
        public static Dictionary<string, string> _users { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> _userIDs { get; set; } = new Dictionary<string, string>();
        //IUserBL _bl;

        public ChatHub()
        {
            //_bl = bl;
        }

        public void Login(string userName)
        {
            _users.Add(userName, this.Context.ConnectionId);
            _userIDs.Add(this.Context.ConnectionId, userName);
        }

        public void Send(string recipient, string message)
        {
            string sender = _userIDs[this.Context.ConnectionId];
            if (recipient == "All")
            {
                SendToAll(sender, message);
            }
            else
            {
                string receiver = _users[recipient];
                SendTo(receiver, sender, message);
            }
        }
        private void SendTo(string receiver, string sender, string message)
        {
            Clients.Client(receiver).SendAsync("broadcastMessage", sender, message);
        }
        private void SendToAll(string sender, string message)
        {
            Clients.All.SendAsync("broadcastMessage", sender, message);
        }

        public void BackGammonInvite(string invRecipient)
        {
            string sender = _userIDs[this.Context.ConnectionId];
            string receiver = _users[invRecipient];
            Clients.Client(receiver).SendAsync("invite", sender);
        }

        public void AcceptGameInvite(string inviter)
        {
            string sender = _userIDs[this.Context.ConnectionId];
            string receiver = _users[inviter];
            Clients.Client(receiver).SendAsync("GameInviteAccepted", sender);
        }
    }
}
