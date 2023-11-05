using Budget_Planner.BudgetPlanner.Data;
using System.Diagnostics;

namespace Budget_Planner.pages;

public partial class More : ContentPage
{
    public List<BPMoreOption> ListMoreOptions { get; set; } = new List<BPMoreOption>();



    public More()
	{
        ListMoreOptions = BPMoreOption.MoreOptionGetOptionsList();

        InitializeComponent();

    }

    public async void MoreGoToSelectedPage(object sender,  EventArgs e)
    {
        var ListView = (ListView)sender;
        var item = ListView.SelectedItem as BPMoreOption;

        await Navigation.PushAsync(item.MoreOptionPage);

    }




}

public class TestObject
{
    public string name { get; set; } = "Hello World";
    public Int32 age { get; set; } = 1;
}