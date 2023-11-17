using Budget_Planner.BudgetPlanner.Data;
using MySqlConnector;
using System.Diagnostics;
using Budget_Planner.BudgetPlanner;
using System.Collections.ObjectModel;

namespace Budget_Planner.pages;

public partial class AddExpense : ContentPage
{
    public List<BPCategory> listCategories { get; set; } = new List<BPCategory>();

    public AddExpense()
	{
        InitializeComponent();

        GetSpendingCategoriesList();
        BindingContext = this;

    }

    public void GetSpendingCategoriesList()
    {
        BPApplication bpApp = new BPApplication();
        var result = bpApp.SpendingCategoriesGetAllCategories();
        if (result.ServerResult)
        {
            listCategories = result.ServerResultDataList as List<BPCategory>;
        }
        else
        {
            ServerResult.Text = result.ServerResultMessage;
            ServerResult.TextColor = Color.FromRgba(200, 0, 0, 1);
            ServerResult.IsVisible = true;
        }

    }

    public void AddNewExpense(object sender, EventArgs e)
    {

    }

}