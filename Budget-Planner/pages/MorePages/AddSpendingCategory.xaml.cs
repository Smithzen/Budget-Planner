using Budget_Planner.BudgetPlanner;
using Budget_Planner.BudgetPlanner.Data;

namespace Budget_Planner.pages.MorePages;

public partial class AddSpendingCategory : ContentPage
{
	public AddSpendingCategory()
	{
		InitializeComponent();
	}

	public async void AddSpendingCategoryCreateCategory(object sender, EventArgs e)
	{
		BPApplication bpApp = new BPApplication();
		BPServerResult result = new BPServerResult();

		string categoryName = entryCategoryName.Text;
		string categoryDescription = entryCategoryDescription.Text;


		if (!string.IsNullOrEmpty(categoryName))
		{
			result = bpApp.SpendingCategoriesAddNewCategory(categoryName, categoryDescription);
        }
		else
		{
			labelServerResult.Text = "Please enter a name for your category";
            labelServerResult.IsVisible = true;
            labelServerResult.BackgroundColor = Color.FromRgba(200, 0, 0, 1);
        }


		if (!result.ServerResult)
		{
			labelServerResult.Text = result.ServerResultMessage;
            labelServerResult.IsVisible = true;
            labelServerResult.BackgroundColor = Color.FromRgba(200, 0, 0, 1);
		}
		else
		{
			await Navigation.PopAsync();
		}
    }
}