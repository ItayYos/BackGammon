using System;
using System.Threading.Tasks;
using Client.EventHandling;

using Microsoft.AspNetCore.SignalR.Client;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Client.BL
{
    class ChatHubConnection
    {
        HubConnection hubConnection;
        public event EventHandler<UserLoggedInEventArgs> RecieveMsg;
        public event EventHandler<UserLoggedInEventArgs> ReciveInvite;
        public event EventHandler<UserLoggedInEventArgs> GameInvAcceptedEvent;

        public ChatHubConnection()
        {

        }

        public void ConnectToChat(string username)
        {
            hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:56556/Chat")
            .Build();

            hubConnection.On("broadcastMessage", async (string name, string message) => {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        RecieveMsg(this, new UserLoggedInEventArgs($"{name}: {message}"));
                    });
            });
             

            hubConnection.On("invite", async (string sender) => {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        ReciveInvite(this, new UserLoggedInEventArgs(sender));
                    });
            });

            hubConnection.On("GameInviteAccepted", async (string invitee) =>
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        GameInvAcceptedEvent(this, new UserLoggedInEventArgs(invitee));
                    });
            });

            hubConnection.StartAsync();

            hubConnection.InvokeAsync("Login", username);
        }

        public void SendMessage(string recipient, string message)
        {
            hubConnection.InvokeAsync("Send", recipient, message);
        }

        public void SendBackGammonInvite(string inviteRecp)
        {
            hubConnection.InvokeAsync("BackGammonInvite", inviteRecp);
        }

        public void AcceptGameInvite(string inviter)
        {
            hubConnection.InvokeAsync("AcceptGameInvite", inviter);
        }

    }
}
