using Budget_Planner.BudgetPlanner.Data;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Budget_Planner.BudgetPlanner
{
    public partial class BPApplication
    {

        public BPServerResult GetBarChartData(Int32 selectedIndex)
        {
            BPServerResult result = new BPServerResult();

            List<BPExpense> listExpenses = new List<BPExpense>();
            List<string> listCategoryNames = new List<string>();

            MySqlConnection DBConRO = new MySqlConnection(builder.ConnectionString);
            try
            {

                DBConRO.Open();

                MySqlCommand cmd;
                MySqlDataReader reader;

                DateTime searchDate = GetSearchDate(selectedIndex);

                cmd = new MySqlCommand("SELECT * FROM expenses WHERE ExpenseDate>=@ExpenseDate AND UserGUID=@UserGUID", DBConRO);
                cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@ExpenseDate", MySqlDbType = MySqlDbType.DateTime, Value = searchDate });
                cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@UserGUID", MySqlDbType = MySqlDbType.VarChar, Value = BPApplication.UserGUID });
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //list of all retrieved expenses
                    listExpenses.Add(new BPExpense()
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
                    List<BPExpense> listExpenseCategory = new List<BPExpense>();
                    listExpenseCategory = listExpenses.Where(p => p.ExpenseCategory.CategoryName == category).ToList();
                    listExpensesByCategory.Add(listExpenseCategory);
                }

                result.ServerResultDataList = listExpensesByCategory;
                result.ServerResult = true;
                result.ServerResultMessage = "Successfully retrieved expenses";


            }
            catch (Exception ex)
            {
                result.ServerResult = false;
                result.ServerResultMessage = "GetLineChartData Ex: " + ex.Message;
            }
            finally
            {
                DBConRO.Close();
                DBConRO.Dispose();
            }

            return result;
        }


        public BPServerResult GetLineChartData(int selectedIndex)
        {

            BPServerResult result = new BPServerResult();


            List<BPExpense> listExpenses = new List<BPExpense>();
            List<DateTime> listExpenseDates = new List<DateTime>();


            MySqlConnection DBConRO = new MySqlConnection(builder.ConnectionString);
            try
            {



            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return result;
        }


        private static DateTime GetSearchDate(int selectedIndex)
        {
            DateTime searchDate = DateTime.Now;

            switch (selectedIndex)
            {
                case 0:
                    searchDate = DateTime.Now.AddDays(-7);
                    break;

                case 1:
                    searchDate = DateTime.Now.AddDays(-30);
                    break;

                case 2:
                    searchDate = DateTime.MinValue;
                    break;

                default:
                    throw new Exception("No valid date range added");
            }

            return searchDate;
        }
    }
}
