using Client.BL;
using Client.EventHandling;
using Client.Views;
using GalaSoft.MvvmLight.Command;
using System;
using System.ComponentModel;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WPFClient.Commands;

namespace Client.View_Model
{
    class MainMenuVM : INotifyPropertyChanged
    {
        private string _username;

        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        private string _incomingMessage;

        public string IncomingMessage
        {
            get { return _incomingMessage; }
            set { _incomingMessage = value; OnPropertyChanged(nameof(IncomingMessage)); }
        }

        private string _outgoingMsg;

        public string OutgoingMsg
        {
            get { return _outgoingMsg; }
            set { _outgoingMsg = value; OnPropertyChanged(nameof(OutgoingMsg)); }
        }

        private string _recipient;

        public string Recipient
        {
            get { return _recipient; }
            set { _recipient = value; OnPropertyChanged(nameof(Recipient)); }
        }

        private string _backgammonInvitationRecipient;

        public string BackgammonInvitationRecipient
        {
            get { return _backgammonInvitationRecipient; }
            set { _backgammonInvitationRecipient = value; OnPropertyChanged(nameof(BackgammonInvitationRecipient)); }
        }

        private string _inviteContent;

        public string InviteContent
        {
            get { return _inviteContent; }
            set { _inviteContent = value; OnPropertyChanged(nameof(InviteContent)); }
        }
        private string _inviter;

        private Visibility _showNotification;

        public Visibility ShowNotifiction
        {
            get { return _showNotification; }
            set { _showNotification = value; OnPropertyChanged(nameof(ShowNotifiction)); }
        }

        public CommandExecuter BackgammonInviteCommand { get; set; }

        public CommandExecuter AcceptGameInviteCommand { get; set; }

        public RelayCommand SendMsgCommand { get; set; }

        ChatHubConnection chatHubConnection;
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainMenuVM()
        {
            _outgoingMsg = string.Empty;
            SendMsgCommand = new RelayCommand(SendMsg);
            _recipient = "All";
            IncomingMessage = string.Empty;
            _backgammonInvitationRecipient = string.Empty;
            BackgammonInviteCommand = new CommandExecuter(InviteToBackgammon, ()=>true);
            _inviteContent = string.Empty;
            _showNotification = Visibility.Collapsed;
            AcceptGameInviteCommand = new CommandExecuter(AcceptGameInvite, () => true);

            _inviter = string.Empty;
        }

        public void OnLoggedIn(string userName)
        {
            Username = userName;
            chatHubConnection = new ChatHubConnection();
            chatHubConnection.ConnectToChat(Username);
            chatHubConnection.RecieveMsg += RecieveMsg;
            chatHubConnection.ReciveInvite += ReceiveBackgammonInvite;
            chatHubConnection.GameInvAcceptedEvent += GameInvAccepted;
        }
                        
        private void RecieveMsg(object sender, UserLoggedInEventArgs e)
        {
            IncomingMessage += $"{e.UserName}\n";
            DisplayCheck();
        }

        private void DisplayCheck()
        {
            int maxlines = 5;
            
            string[] lines = IncomingMessage.Split("\n");
            if (lines.Length > maxlines)
            {
                IncomingMessage = string.Empty;
                for (int i = lines.Length - maxlines - 1; i < lines.Length-1; i++)
                {
                    IncomingMessage += $"{lines[i]}\n";
                }
            }  
        }

        private void SendMsg()
        {
            chatHubConnection.SendMessage(Recipient, OutgoingMsg);
            OutgoingMsg = string.Empty;
        }
        
        private void InviteToBackgammon()
        {
            chatHubConnection.SendBackGammonInvite(BackgammonInvitationRecipient);
        }


        private void ReceiveBackgammonInvite(object sender, UserLoggedInEventArgs e)
        {
            InviteContent = $"{e.UserName} invited you to play backgammon";
            _inviter = e.UserName;
            ShowNotifiction = Visibility.Visible;
        }

        private void AcceptGameInvite()
        {
            chatHubConnection.AcceptGameInvite(_inviter);
            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(Backgammon), Username + "," + _inviter);
        }

        private void GameInvAccepted(object sender, UserLoggedInEventArgs e)
        {
            string invitee = e.UserName;
            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(Backgammon), Username + "," + invitee);
        }
        
        public void OnLoggedOut()
        {
            chatHubConnection.RecieveMsg -= RecieveMsg;
            chatHubConnection.ReciveInvite -= ReceiveBackgammonInvite;
            chatHubConnection.GameInvAcceptedEvent -= GameInvAccepted;
        }
    }
}
