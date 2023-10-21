using Budget_Planner.BudgetPlanner;

namespace Budget_Planner.pages;

public partial class CreateAccount : ContentPage
{
	public CreateAccount()
	{
		InitializeComponent();
	}

	public async void CreateNewAccount(object sender, EventArgs e)
	{
		string enteredEmail = entryEmail.Text;
		string enteredPassword = entryPassword.Text;

		BPApplication bpApp = new BPApplication();

		var result = bpApp.AuthCreateAccount(enteredEmail, enteredPassword);

		//show result message
		labelServerResult.Text = result.ServerResultMessage;

		if (result.ServerResult)
        {
			//set background colour to green if successful
            labelServerResult.BackgroundColor = Color.FromRgba(155, 255, 155, 155);
            await Shell.Current.GoToAsync("//TodaysSpending");
		}
		else
		{
			//set to red if unsuccessful
            labelServerResult.BackgroundColor = Color.FromRgba(255, 155, 155, 155);
        }

    }
}