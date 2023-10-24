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
            BPServerResult result = bpApplication.AuthBackgroundlogin();
            if (result.ServerResult)
            {
                Shell.Current.GoToAsync("//TodaysSpending");
            }

            InitializeComponent();
            Shell.SetTabBarIsVisible(Content, false);
        }

        public async void Login(object sender, EventArgs e)
        {
            BPApplication bpApplication = new BPApplication();
            BPServerResult result = new BPServerResult();

            string userEmail = entryEmail.Text;
            string userPassword = entryPassword.Text;

            result = bpApplication.AuthLogin(userEmail, userPassword);
            if (result.ServerResult)
            {
                await Shell.Current.GoToAsync("//TodaysSpending");
            }
            else
            {
                labelServerResult.Text = result.ServerResultMessage;
                labelServerResult.IsVisible = true;
                labelServerResult.BackgroundColor = Color.FromRgba(200, 0, 0, 100);
            }

            //Login login = new Login();


        }

        public async void CreateAccount(object sender, EventArgs e)
        {
            CreateAccount createAccount = new CreateAccount();
            createAccount.Title = "Create Account";
            await Navigation.PushAsync(createAccount);
        }

    }
}