using Budget_Planner.pages;
using MySql.Data.MySqlClient;

namespace Budget_Planner
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            CurrentDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy");

        }

        public void CalculateTotalSpent(object sender, EventArgs e)
        {
            double _foodSpent;
            double _billsSpent;
            double _recreationSpent;
            double _otherSpent;
            double _totalSpent;

            Double.TryParse(foodSpent.Text, out _foodSpent);
            double.TryParse(billsSpent.Text, out _billsSpent);
            double.TryParse(recreationalSpent.Text, out _recreationSpent);
            double.TryParse(otherSpent.Text, out _otherSpent);

            _totalSpent = _foodSpent + _billsSpent + _recreationSpent + _otherSpent;
            totalSpent.Text = _totalSpent.ToString("C");
        }

        public async void AddExpense(object sender, EventArgs e)
        {
            //adds back button
            AddExpense addExpensePage = new AddExpense();
            addExpensePage.Title = "Add New Expense";
            await Navigation.PushAsync(addExpensePage);

            //no back button so would only be useful if you don't want to go back to another page
            //await Shell.Current.GoToAsync("//AddExpense");

        }

        public void OnSwiped(object sender, EventArgs e)
        {
            Console.WriteLine("Swiped");
        }

        //private void OnCounterClicked(object sender, EventArgs e)
        //{
        //    count++;

        //    if (count == 1)
        //        CounterBtn.Text = $"Clicked {count} time";
        //    else
        //        CounterBtn.Text = $"Clicked {count} times";

        //    SemanticScreenReader.Announce(CounterBtn.Text);
        //}

    }
}