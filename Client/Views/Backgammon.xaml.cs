using Client.View_Model;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Client.Views
{
    public sealed partial class Backgammon : Page
    {
        public Backgammon()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            (DataContext as BackgammonVM).OnLoggedIn(e.Parameter as string);
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            (DataContext as BackgammonVM).Output = "right dice";
        }
    }
}
