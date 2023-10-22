using Budget_Planner.pages;
using Budget_Planner.BudgetPlanner;
using Budget_Planner.BudgetPlanner.Data;

namespace Budget_Planner
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            BPApplication bpApplication = new BPApplication();
            //BPServerResult result = bpApplication.AuthBackgroundlogin();
            //if (result.ServerResult)
            //{
            //    Shell.Current.GoToAsync("//TodaysSpending");
            //}

            InitializeComponent();
            Shell.SetTabBarIsVisible(Content, false);
        }

        public async void Login(object sender, EventArgs e)
        {
            BPApplication bpApplication = new BPApplication();
            //bpApplication.AuthCreateAccount("curtis.p.smith@zoho.com", "hello");
            bpApplication.AuthLogin("curtis.p.smith@zoho.com", "pass");
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