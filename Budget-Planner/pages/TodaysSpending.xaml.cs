using Budget_Planner.BudgetPlanner;
using Budget_Planner.BudgetPlanner.Data;
using Microcharts;
using SkiaSharp;

namespace Budget_Planner.pages;

public partial class TodaysSpending : ContentPage
{

    private List<ChartEntry> listChartData = new List<ChartEntry>();
    private string[] chartColours = new string[]
    {
        "#472B7C",
        "#427B9B",
        "#369F95",
        "#4AC06B",
        "#9F2B0E",
        "#D39D21"
    };



    public TodaysSpending()
	{
		InitializeComponent();

        CurrentDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy");

        chartView.Chart = new DonutChart
        {
            BackgroundColor = SKColor.Parse("#FFFFFF"),
            Entries = listChartData,
            LabelMode = LabelMode.RightOnly,
            LabelTextSize = 40,
        };


    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        listChartData = new List<ChartEntry>();
        chartView.Chart = new DonutChart
        {
            BackgroundColor = SKColor.Parse("#FFFFFF"),
            Entries = listChartData,
            LabelMode = LabelMode.RightOnly,
            LabelTextSize = 40,
        };
        GetTodaysExpenses();

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

    public void GetTodaysExpenses()
    {

        BPApplication bpApp = new BPApplication();
        var result = bpApp.TodaysSpendingGetTodaysData();

        var largestExpenseCategory = SetChartData(result);

        largestSpendingCategoryValue.Text = largestExpenseCategory.ToString();
        largestSpendingCategoryValue.TextColor = Color.FromHex("#000000");

        SetTodaysTotalExpense(result);

    }

    public void SetTodaysTotalExpense(BPServerResult result)
    {
        double value = 0;

        foreach (List<BPExpense> listExpenseCategory in result.ServerResultDataList)
        {

            //getting total value for all expenses in each category
            foreach (BPExpense expense in listExpenseCategory)
            {

                value = value + expense.ExpenseAmount;
            }

        }

        labelSpentToday.Text = value.ToString("C");
    }

    public string SetChartData(BPServerResult result)
    {
        double largestExpense = 0;
        string largestExpenseCategory = string.Empty;

        var i = 0;
        foreach (List<BPExpense> listExpenseCategory in result.ServerResultDataList)
        {
            float value = 0;
            string label = string.Empty;

            //getting total value for all expenses in each category
            foreach (BPExpense expense in listExpenseCategory)
            {
                if (string.IsNullOrEmpty(label))
                {
                    label = expense.ExpenseCategory.CategoryName;
                }

                value = value + (float)expense.ExpenseAmount;
            }

            //getting the largest spending category from the lists
            if (value > largestExpense)
            {
                largestExpense = value;
                largestExpenseCategory = label;
            }

            //creating chart data
            listChartData.Add(new ChartEntry(value)
            {
                Label = label,
                Color = SKColor.Parse(chartColours[i])
            });

            i++;
        }

        return largestExpenseCategory;
    }

}