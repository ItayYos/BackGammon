using Client.View_Model;
using System;
using System.ServiceModel.Security;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainMenu : Page
    {
                
        public MainMenu()
        {
            this.InitializeComponent();            
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            (DataContext as MainMenuVM).OnLoggedIn(e.Parameter as string);
        }
        
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            (DataContext as MainMenuVM).OnLoggedOut();
            base.OnNavigatedFrom(e);
        }
        
    }
}
