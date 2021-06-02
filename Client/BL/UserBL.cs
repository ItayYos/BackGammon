using Client.EventHandling;
using Common;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;
using Client.BL;

namespace BL.WPFClient
{
    class UserBL
    {
        HubConnection hubConnection;
        public event EventHandler<UserLoggedInEventArgs> LogInCompleted;

        public UserBL()
        {
        }

        public void ConnectToServer()
        {
            hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:56556/User")
            .Build();

            hubConnection.On<string>("LogInNotificated", async (string userName) =>
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        LogInCompleted(this, new UserLoggedInEventArgs(userName));
                    });
            });

            hubConnection.StartAsync();
        }

        public async Task<string> Register(User user)
        {
            string error = await hubConnection.InvokeAsync<string>("Register", user);
            return error;
        }

        public async Task<IEnumerable<string>> GetAllUsers()
        {
            IEnumerable<string> error = await hubConnection.InvokeAsync<IEnumerable<string>>("GetAllUsers");
            return error;
        }

        public async Task<string> Login(User user)
        {
            string error = await hubConnection.InvokeAsync<string>("Login", user);
            return error;
        }

        
    }
}
