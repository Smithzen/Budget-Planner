using Budget_Planner.BudgetPlanner;
using Budget_Planner.BudgetPlanner.Data;
using Budget_Planner.pages.MorePages;
using System.Diagnostics;



namespace Budget_Planner.pages;

public partial class SpendingCategories : ContentPage
{
	public List<BPCategory> listCategories { get; set; } = new List<BPCategory>();

	public SpendingCategories()
	{
		InitializeComponent();
	}

	public SpendingCategories(string title)
	{
		this.Title = title;

		GetSpendingCategoriesList();

		InitializeComponent();

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

	public void SpendingCategoriesRefreshListView(object sender, EventArgs e)
	{
		BPApplication bpApp = new BPApplication();
		var result = bpApp.SpendingCategoriesGetAllCategories();
		if (result.ServerResult)
		{
			listCategories = result.ServerResultDataList as List<BPCategory>;
			listSpendingCategories.ItemsSource = listCategories;
		}
		else
		{
			ServerResult.Text = result.ServerResultMessage;
			ServerResult.TextColor = Color.FromRgba(200, 0, 0, 1);
			ServerResult.IsVisible = true;
		}
        listSpendingCategories.EndRefresh();

    }

    public async void SpendingCategoriesCreateNewCategory(object sender, EventArgs e)
	{
		AddSpendingCategory addSpendingCategory = new AddSpendingCategory();
		await Navigation.PushAsync(addSpendingCategory);

    }

	public async void SpendingCategoriesDeleteCategory(object sender , EventArgs e)
	{
		var button = (Button)sender;
		var categoryGUID = button.CommandParameter;

		var result = await DisplayAlert("Delete Category", "Are you sure you want to delete this category?", "Yes", "No");
		Debug.WriteLine(result);

		//if result is true(find new variable name) then call bpApp delete function
		if (result)
		{
			BPApplication bpApp = new BPApplication();
			var serverResult = bpApp.SpendingCategoriesDeleteCategory(categoryGUID.ToString());

			if (serverResult.ServerResult)
			{
				listSpendingCategories.BeginRefresh();
			}
		}
	}

}