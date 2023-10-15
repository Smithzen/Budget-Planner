using Budget_Planner.pages;
using MySql.Data.MySqlClient;

namespace Budget_Planner
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Shell.SetTabBarIsVisible(Content, false);
        }

        public async void Login(object sender, EventArgs e)
        {

            //Login login = new Login();
            await Shell.Current.GoToAsync("//TodaysSpending");


        }

    }
}