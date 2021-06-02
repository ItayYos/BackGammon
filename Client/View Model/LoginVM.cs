using BL.WPFClient;
using Client.Views;
using Common;
using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WPFClient.Commands;

namespace Client.View_Model
{
    class LoginVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        UserBL bl;
        public CommandExecuter LoginCommand { get; set; }
        public CommandExecuter NavToRegCommand { get; set; }

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set 
            { 
                _userName = value;
                OnPropertyChanged("UserName");
                LoginCommand.NotifyCanExecuteChanged();
            }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set 
            { 
                _password = value;
                OnPropertyChanged("Password");
                LoginCommand.NotifyCanExecuteChanged();
            }
        }

        private string _output;

        public string Output
        {
            get { return _output; }
            set 
            { 
                _output = value;
                OnPropertyChanged("Output");
            }
        }

        public LoginVM()
        {
            bl = new UserBL();
            bl.ConnectToServer();
            LoginCommand = new CommandExecuter(Login, ()=> { return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password); });
            NavToRegCommand = new CommandExecuter(NavToReg, ()=>true);
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void Login()
        {
            User user = new User(UserName, Password);
            Output = await bl.Login(user);
            if (Output == string.Empty)
                NavMainMenu();
        }
        private void NavMainMenu()
        {
            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(MainMenu), UserName);
        }
        public void NavToReg()
        {
            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(Register));
        }
    }
}
