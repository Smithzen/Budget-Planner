using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget_Planner.pages;

namespace Budget_Planner.BudgetPlanner.Data
{
    public class BPMoreOption
    {
        public string MoreOptionName { get; set; } = string.Empty;
        public string MoreOptionDetail { get; set; } = string.Empty;
        public Page MoreOptionPage { get; set; } = new Page();

        public static List<BPMoreOption> MoreOptionGetOptionsList()
        {
            List<BPMoreOption> listMoreOptions = new List<BPMoreOption>();

            listMoreOptions.Add(new BPMoreOption()
            {
                MoreOptionName = "Spending Category",
                MoreOptionDetail = "Manage your expense categories",
                MoreOptionPage = new SpendingCategories("Spending Categories")
            });

            return listMoreOptions;
        }

    }
}
