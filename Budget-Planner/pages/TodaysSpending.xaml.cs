using Microcharts;
using SkiaSharp;

namespace Budget_Planner.pages;

public partial class TodaysSpending : ContentPage
{

    ChartEntry[] entries = new[]
    {
        new ChartEntry(1)
        {
            Label = "one",
            ValueLabel = "one",
            Color = SKColor.Parse("#f3f115")
        },
        new ChartEntry(2)
        {
            Label = "two",
            ValueLabel = "two",
            Color = SKColor.Parse("#11f511")
        },
        new ChartEntry(3)
        {
            Label = "three",
            ValueLabel = "three",
            Color = SKColor.Parse("#d1ddfd")
        }
    };

	public TodaysSpending()
	{
		InitializeComponent();

        CurrentDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy");

        chartView.Chart = new DonutChart
        {
            BackgroundColor = SKColor.Parse("#f3f3f3"),
            Entries = entries
        };

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

}