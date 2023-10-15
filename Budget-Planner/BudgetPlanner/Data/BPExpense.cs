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
        public double ExpenseFoodValue { get; set; } = 0;
        public double ExpenseBillsValue { get; set; } = 0;
        public double ExpenseRecreationalValue { get; set; } = 0;
        public double ExpenseOtherValue { get; set; } = 0;
        public string ExpenseNote { get; set; } = string.Empty;

    }
}