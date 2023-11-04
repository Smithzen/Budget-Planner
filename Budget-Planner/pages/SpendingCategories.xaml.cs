using Budget_Planner.BudgetPlanner;
using Budget_Planner.BudgetPlanner.Data;
using CommunityToolkit.Maui.Views;


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
		InitializeComponent();
	}


	public void GetSpendingCategoriesList()
	{
		BPApplication bpApp = new BPApplication();
		var result = bpApp.SpendingCategoriesGetAllCategories();
		if (result.ServerResult)
		{
			listCategories = result.ServerResultDataList as List<BPCategory>;
			ServerResult.IsVisible = false;
		}
		else
		{
			ServerResult.Text = result.ServerResultMessage;
			ServerResult.TextColor = Color.FromRgba(0, 0, 0, 1);
			ServerResult.IsVisible = true;
		}

	}


	public void SpendingCategoriesCreateNewCategory(object sender, EventArgs e)
	{
		var popup = new Popup()
		{
			Content = new VerticalStackLayout()
			{
				Children =
				{
					new Label()
					{
						Text = "Test popup",
						TextColor = Color.FromRgb(0,0,0)
					}
				}
			}
		};
	}


}