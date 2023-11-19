using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget_Planner.BudgetPlanner.Data
{
    public class BPExpense
    {
        public string ExpenseGUID { get; set; } = string.Empty;
        public DateTime ExpenseDate { get; set; } = DateTime.Now;
        public BPCategory ExpenseCategory { get; set; } = new BPCategory();
        public double ExpenseAmount { get; set; } = 0;
        public string ExpenseNote { get; set; } = string.Empty;

    }
}