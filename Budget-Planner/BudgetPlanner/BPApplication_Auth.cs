using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace Budget_Planner.BudgetPlanner
{
    public partial class BPApplication
    {
        private string UserGUID { get; set; } = string.Empty;


        private bool CheckForExistingUserGUID()
        {

            //check for local json file

            //check json for encrypted UserGUID

            return false;
        }

        public void login()
        {
            //if there is an existing UserGUID GetUserGUID()

            //check if it matches an known user in database
            //true = set UserGUID equal to retrieved UserGUID
            //false = request manual login

            //hashing passwords
            //https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-7.0
        }


        public void GetUserGUID()
        {
            //encrypting data and decrypting data to be used for emails passwords and user ids
            //user ids to be stored in local json file to be collected when app opens or login/account creation will be requested
            //https://learn.microsoft.com/en-us/dotnet/standard/security/encrypting-data



            //retrieve encrypted hash string from local json file;
            


            //decrypt id string


            //return decrypted string



        }



    }
}
