using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Budget_Planner.BudgetPlanner.Data;
using MySqlConnector;

namespace Budget_Planner.BudgetPlanner
{
    public partial class BPApplication
    {
        public BPServerResult SpendingCategoriesGetAllCategories()
        {
            BPServerResult bpServerResult = new BPServerResult();
            List<BPCategory> listCategories = new List<BPCategory>();

            MySqlConnection DBConRO = new MySqlConnection(builder.ConnectionString);
            try
            {
                DBConRO.Open();

                MySqlCommand cmd;
                MySqlDataReader reader;

                cmd = new MySqlCommand("SELECT CategoryGUID, CategoryName, CategoryDescription FROM categories WHERE UserGUID=@UserGUID", DBConRO);
                cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@UserGUID", MySqlDbType = MySqlDbType.VarChar, Value = UserGUID });
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    listCategories.Add(new BPCategory()
                    {
                        CategoryGUID = reader["CategoryGUID"].ToString(),
                        CategoryName = reader["CategoryName"].ToString(),
                        CategoryDescription = reader["CategoryDescription"].ToString()
                    });
                }
                reader.Close();
                reader.Dispose();


                bpServerResult.ServerResultMessage = "Successfully retrieved category list";
                bpServerResult.ServerResult = true;
                bpServerResult.ServerResultDataList = listCategories;


            }
            catch (Exception ex)
            {
                bpServerResult.ServerResultMessage = ex.Message;
                bpServerResult.ServerResultDataList = null;
                bpServerResult.ServerResult = false;
            }
            finally
            {
                DBConRO.Close();
                DBConRO.Dispose();
            }


            return bpServerResult;
        }
    }
}
