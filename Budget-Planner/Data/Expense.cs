using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget_Planner.Data
{
    public class Expense
    {
        public string? ExpenseGUID { get; set; } = string.Empty;
        public DateTime ExpenseDate { get; set; } = DateTime.Now;
        public Double ExpenseFoodValue { get; set; } = 0;
        public Double ExpenseBillsValue { get; set; } = 0;
        public Double ExpenseRecreationalValue { get; set; } = 0;
        public Double ExpenseOtherValue { get; set; } = 0;
        public string ExpenseNote { get; set; } = string.Empty;

    }
}