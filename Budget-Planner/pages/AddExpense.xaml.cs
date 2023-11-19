using Budget_Planner.BudgetPlanner.Data;
using MySqlConnector;
using System.Diagnostics;
using Budget_Planner.BudgetPlanner;
using System.Collections.ObjectModel;
using Org.BouncyCastle.Asn1.BC;
using Microsoft.Maui.Graphics.Text;

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

    public async void AddNewExpense(object sender, EventArgs e)
    {
        BPApplication bpApp = new BPApplication();

        BPExpense newExpense = new BPExpense();
        newExpense.ExpenseGUID = Guid.NewGuid().ToString();
        newExpense.ExpenseDate = dateExpenseDate.Date;
        newExpense.ExpenseCategory = pickerSpendingCategory.SelectedItem as BPCategory;
        newExpense.ExpenseAmount = Convert.ToDouble(entryAmountSpent.Text);
        newExpense.ExpenseNote = editorExpenseNotes.Text;


        var result = bpApp.ExpensesAddNewExpense(newExpense);

        if (result.ServerResult)
        {
            ServerResult.IsVisible = true;
            ServerResult.Text = result.ServerResultMessage;
            ServerResult.BackgroundColor = Color.FromRgba(0, 100, 0, 0.5);
            await Navigation.PopToRootAsync();
        }
        else
        {
            ServerResult.IsVisible = true;
            ServerResult.Text = result.ServerResultMessage;
            ServerResult.BackgroundColor = Color.FromRgba(100, 0, 0, 0.5);
        }


    }

    private void ExpenseAmountFormatValue(object sender, TextChangedEventArgs e)
    {

        if (e.NewTextValue.Contains("."))
        {
            if (e.NewTextValue.Length - 1 - e.NewTextValue.IndexOf(".") > 2)
            {
                var s = e.NewTextValue.Substring(0, e.NewTextValue.IndexOf(".") + 2 + 1);
                entryAmountSpent.Text = s;
                entryAmountSpent.SelectionLength = s.Length;
            }
        }

    }
}