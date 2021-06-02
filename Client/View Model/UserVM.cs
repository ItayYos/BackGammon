using BL.WPFClient;
using Client.EventHandling;
using Client.Views;
using Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WPFClient.Commands;

namespace Client.View_Model
{
    class UserVM : INotifyPropertyChanged
    {
        //List of Existing User
        public ObservableCollection<string> Users { get; set; } = new ObservableCollection<string>();

        public CommandExecuter CreateUserCommand { get; set; }
        UserBL bl = new UserBL();

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public UserVM()
        {
            bl.LogInCompleted += Bl_LogInCompleted;
            bl.ConnectToServer();

            CreateUserCommand = new CommandExecuter(RegisterUser, () => {
                return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);
            });
            Task t = LoadUsers();
        }

        public async Task LoadUsers()
        {
            //Load all existing users before startup
            IEnumerable<string> userList = await bl.GetAllUsers();
            Users = new ObservableCollection<string>(userList);
            OnPropertyChanged("Users");
        }

        private string userName;
        private string password;

        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
                OnPropertyChanged("UserName");
                CreateUserCommand.NotifyCanExecuteChanged();
            }
        }
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                OnPropertyChanged("Password");
                CreateUserCommand.NotifyCanExecuteChanged();
            }
        }

        private string error;

        public string Error
        {
            get
            {
                return error;
            }
            set
            {
                error = value;
                OnPropertyChanged("Error");
            }
        }

        public async void RegisterUser()
        {
            if (Password.Length < 4)
            {
                Error = "Password must be 4 characters or more. ";
            }
            else
            {
                User user = new User(UserName, Password);
                Error = await bl.Register(user);
                if (Error == string.Empty)
                    NavNext();
            }
        }

        private void NavNext()
        {
            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(MainMenu), UserName);
        }

        /// <summary>
        /// Once user has logged in, update the users in the list
        /// </summary>
        /// <param name="userName"></param>
        private void Bl_LogInCompleted(object sender, UserLoggedInEventArgs e)
        {
            Users.Add(e.UserName);
            OnPropertyChanged("Users");
        }
    }
}
