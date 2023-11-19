using Budget_Planner.BudgetPlanner.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace Budget_Planner.BudgetPlanner
{
    public partial class BPApplication
    {
        public BPServerResult ExpensesAddNewExpense(BPExpense expense)
        {
            BPServerResult result = new BPServerResult();


            MySqlConnection DBConM = new MySqlConnection(builder.ConnectionString);
            try
            {
                DBConM.Open();

                MySqlCommand cmd;
                Int32 intResult = 0;

                cmd = new MySqlCommand("INSERT INTO expenses (ExpenseGUID, UserGUID, ExpenseDate, ExpenseCategoryGUID, ExpenseCategoryName, ExpenseAmount, ExpenseNote)VALUE(@ExpenseGUID, @UserGUID, @ExpenseDate, @ExpenseCategoryGUID, @ExpenseCategoryName, @ExpenseAmount, @ExpenseNote)", DBConM);
                cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@ExpenseGUID", MySqlDbType = MySqlDbType.VarChar, Value = expense.ExpenseGUID });
                cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@UserGUID", MySqlDbType = MySqlDbType.VarChar, Value = UserGUID });
                cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@ExpenseDate", MySqlDbType = MySqlDbType.DateTime, Value = expense.ExpenseDate });
                cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@ExpenseCategoryGUID", MySqlDbType = MySqlDbType.VarChar, Value = expense.ExpenseCategory.CategoryGUID });
                cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@ExpenseCategoryName", MySqlDbType = MySqlDbType.VarChar, Value = expense.ExpenseCategory.CategoryName });
                cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@ExpenseAmount", MySqlDbType = MySqlDbType.Double, Value = expense.ExpenseAmount });
                cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@ExpenseNote", MySqlDbType = MySqlDbType.VarChar, Value = expense.ExpenseNote });
                intResult = cmd.ExecuteNonQuery();

                if (intResult > 0)
                {
                    result.ServerResult = true;
                    result.ServerResultMessage = "Successfully added expense";
                }

            }
            catch (Exception ex)
            {
                result.ServerResult = false;
                result.ServerResultMessage = "Error adding expense EX: " + ex.Message;
            }
            finally
            {
                DBConM.Close();
                DBConM.Dispose();
            }

            return result;
        }
    }
}
