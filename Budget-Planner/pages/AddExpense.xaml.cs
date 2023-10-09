using MySqlConnector;
using System.Diagnostics;

namespace Budget_Planner.pages;

public partial class AddExpense : ContentPage
{
    //string ConnectionString = "server=192.168.1.127;uid=myuser;pwd=mypass;database=budget_planner;";

    //    AccountManager manager = (AccountManager)getSystemService(ACCOUNT_SERVICE);
    //    Account[] list = manager.getAccounts();
    //    String gmail = null;

    //      for(Account account: list)
    //      {
    //          if(account.type.equalsIgnoreCase("com.google"))
    //          {
    //              gmail = account.name;
    //              break;
    //          }
    //      }
    // <uses-permission android:name="android.permission.GET_ACCOUNTS"></uses-permission>
    //https://developer.android.com/training/permissions/requesting.html



    public AddExpense()
	{
        InitializeComponent();
    }

    public void AddNewExpense(object sender, EventArgs e)
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Server = "192.168.1.127",
            UserID = "myuser",
            Password = "mypass",
            Database = "budget_planner",
        };


        MySqlConnection DBCon = new MySqlConnection(builder.ConnectionString);
        try
        {
            DBCon.Open();

            MySqlCommand cmd;
            MySqlDataReader reader;

        }
        catch (Exception ex)
        {
            Debug.WriteLine("AddNewExpense Ex: " + ex.Message);
        }
        finally
        {
            DBCon.Close();
            DBCon.Dispose();
        }
    }

}