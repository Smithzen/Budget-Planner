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

        public BPServerResult SpendingCategoriesAddNewCategory(string categoryName, string categoryDescription)
        {
            BPServerResult bpServerResult = new BPServerResult();
            BPCategory bpCategory = new BPCategory();

            bpCategory.CategoryGUID = Guid.NewGuid().ToString();
            bpCategory.CategoryName = categoryName;
            bpCategory.CategoryDescription = categoryDescription;
            bpCategory.CategoryCreationDate = DateTime.Now;


            MySqlConnection DBConM = new MySqlConnection(builder.ConnectionString);
            try
            {
                DBConM.Open();

                MySqlCommand cmd;
                Int32 intResult = 0;

                cmd = new MySqlCommand("INSERT INTO categories (CategoryGUID, UserGUID, CategoryName, CategoryDescription, CategoryCreationDate) VALUE (@CategoryGUID, @UserGUID, @CategoryName, @CategoryDescription, @CategoryCreationDate)", DBConM);
                cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@CategoryGUID", MySqlDbType = MySqlDbType.VarChar, Value = bpCategory.CategoryGUID });
                cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@UserGUID", MySqlDbType = MySqlDbType.VarChar, Value = BPApplication.UserGUID });
                cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@CategoryName", MySqlDbType = MySqlDbType.VarChar, Value = bpCategory.CategoryName});
                cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@CategoryDescription", MySqlDbType = MySqlDbType.VarChar, Value = bpCategory.CategoryDescription });
                cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@CategoryCreationDate", MySqlDbType = MySqlDbType.VarChar, Value = bpCategory.CategoryCreationDate });
                intResult = cmd.ExecuteNonQuery();

                if (intResult > 0)
                {
                    bpServerResult.ServerResult = true;
                    bpServerResult.ServerResultMessage = "Successfully created new spending category";
                }

            }
            catch (Exception ex)
            {
                bpServerResult.ServerResult = false;
                bpServerResult.ServerResultMessage = "SpendingCategoriesAddNewCategory Ex: " + ex.Message;
            }
            finally
            {
                DBConM.Close();
                DBConM.Dispose();
            }


            return bpServerResult;

        }
    }
}
