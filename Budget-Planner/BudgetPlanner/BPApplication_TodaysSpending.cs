using Budget_Planner.BudgetPlanner.Data;
using Microcharts;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget_Planner.BudgetPlanner
{
    public partial class BPApplication
    { 

        public BPServerResult TodaysSpendingGetTodaysData()
        {

            BPServerResult result = new BPServerResult();


            List<BPExpense> listTodaysExpenses = new List<BPExpense>();
            List<string> listCategoryNames = new List<string>();

            MySqlConnection DBConRO = new MySqlConnection(builder.ConnectionString);
            try
            {
                DBConRO.Open();

                MySqlCommand cmd;
                MySqlDataReader reader;

                //get necessary fields from the database
                cmd = new MySqlCommand("SELECT ExpenseGUID, ExpenseCategoryName, ExpenseAmount FROM expenses WHERE UserGUID=@UserGUID AND ExpenseDate=@ExpenseDate", DBConRO);
                cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@UserGUID", MySqlDbType = MySqlDbType.VarChar, Value = BPApplication.UserGUID });
                cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@ExpenseDate", MySqlDbType = MySqlDbType.DateTime, Value = DateTime.Now.Date });
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //list of all retrieved expenses
                    listTodaysExpenses.Add(new BPExpense()
                    {
                        ExpenseGUID = reader["ExpenseGUID"].ToString(),
                        ExpenseCategory = new BPCategory() { CategoryName = reader["ExpenseCategoryName"].ToString() },
                        ExpenseAmount = Convert.ToDouble(reader["ExpenseAmount"])

                    });

                    //adding unique categories to a list of categories
                    if (!(listCategoryNames.Contains(reader["ExpenseCategoryName"].ToString())))
                        listCategoryNames.Add(reader["ExpenseCategoryName"].ToString());
                }
                reader.Close();
                reader.Dispose();

                //list of lists by expense category
                List<List<BPExpense>> listExpensesByCategory = new List<List<BPExpense>>();

                //creating each unique list from the list of unique expense categories
                foreach (string category in listCategoryNames)
                {
                    List<BPExpense> listExpense = new List<BPExpense>();
                    listExpense = listTodaysExpenses.Where(p => p.ExpenseCategory.CategoryName == category).ToList();
                    listExpensesByCategory.Add(listExpense);
                }

                result.ServerResultDataList = listExpensesByCategory;
                result.ServerResult = true;
                result.ServerResultMessage = "Successfully retrieved today's expenses";

            }
            catch (Exception ex)
            {
                result.ServerResult = false;
                result.ServerResultMessage = ex.Message;
            }
            finally
            {
                DBConRO.Close();
                DBConRO.Dispose();
            }

            return result;
        }

    }
}
