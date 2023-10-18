using Budget_Planner.pages;
using Budget_Planner.BudgetPlanner;
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
            BPApplication bpApplication = new BPApplication();
            bpApplication.EncryptUserGUID();
            bpApplication.WriteUserLoginTokenToDevice();
            
            //Login login = new Login();
            await Shell.Current.GoToAsync("//TodaysSpending");


        }

    }
}