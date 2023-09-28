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