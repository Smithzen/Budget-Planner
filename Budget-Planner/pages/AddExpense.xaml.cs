using MySqlConnector;
using System.Diagnostics;

namespace Budget_Planner.pages;

public partial class AddExpense : ContentPage
{
    string ConnectionString = "server=192.168.1.127;uid=myuser;pwd=mypass;database=budget_planner;";


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
        DBCon.Open();

        try
        {

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