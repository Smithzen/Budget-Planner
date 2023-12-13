using Microcharts.Maui;
using Microcharts;
using SkiaSharp;
using System.Security.Cryptography.X509Certificates;
using Budget_Planner.BudgetPlanner;
using Budget_Planner.BudgetPlanner.Data;
using ModelIO;

namespace Budget_Planner.pages;

public partial class Statistics : ContentPage
{

    public List<ChartEntry> listBarChartData = new List<ChartEntry>();
    public List<ChartEntry> listLineChartData = new List<ChartEntry>();

	public Statistics()
	{
		InitializeComponent();

	}


    protected override void OnAppearing()
    {
        base.OnAppearing();

        listBarChartData = new List<ChartEntry>();


        pickerDateFilter.SelectedIndex = 0;

        if (listBarChartData.Count > 0 )
        {
            barChart.Chart = new BarChart
            {
                BackgroundColor = SKColor.Parse("#FFFFFF"),
                Entries = listBarChartData,
                LabelOrientation = Orientation.Horizontal,
                YAxisPosition = Position.Left,
                ShowYAxisText = true,
                ShowYAxisLines = true,
                ValueLabelOrientation = Orientation.Horizontal,
                ValueLabelTextSize = 20,
                LabelTextSize = 35,
            };
        }

        if (listLineChartData.Count > 0 )
        {
            lineChart.Chart = new LineChart
            {
                BackgroundColor = SKColor.Parse("#FFFFFF"),
                Entries = listLineChartData,
                LabelOrientation = Orientation.Horizontal,
                YAxisPosition = Position.Left,,
                ShowYAxisText = true,
                ShowYAxisLines = true,
                ValueLabelOrientation = Orientation.Horizontal,
                ValueLabelTextSize = 20,
                LabelTextSize = 35,
            };
        }



    }

    private void pickerSelectedIndexChanged(object sender, EventArgs e)
    {

        getBarChartData();

    }

    private void getBarChartData()
    {
        BPApplication bpApp = new BPApplication();
        var result = bpApp.GetBarChartData(pickerDateFilter.SelectedIndex);

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


            var random = new Random();
            var color = String.Format("#{0:X6}", random.Next(0x1000000)); // = "#A197B9"

            //creating chart data
            listBarChartData.Add(new ChartEntry(value)
            {
                Label = label,
                ValueLabel = value.ToString("C"),
                Color = SKColor.Parse(color)
            });
        }

    }

    private void getLineChartData()
    {
        BPApplication bpApp = new BPApplication();
        BPServerResult result = bpApp.GetLineChartData(pickerDateFilter.SelectedIndex);
    }


}