using Budget_Planner.BudgetPlanner;

namespace Budget_Planner
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}