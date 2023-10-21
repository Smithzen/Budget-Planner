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
            //bpApplication.AuthCreateAccount("curtis.p.smith@zoho.com", "hello");
            
            //Login login = new Login();
            await Shell.Current.GoToAsync("//TodaysSpending");


        }

        public async void CreateAccount(object sender, EventArgs e)
        {
            CreateAccount createAccount = new CreateAccount();
            createAccount.Title = "Create Account";
            await Navigation.PushAsync(createAccount);
        }

    }
}