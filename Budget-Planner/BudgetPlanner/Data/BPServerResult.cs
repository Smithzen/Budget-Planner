using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget_Planner.BudgetPlanner.Data
{
    public class BPServerResult
    {
        public bool ServerResult { get; set; } = false;
        public string ServerResultMessage {  get; set; } = string.Empty;
        public Int32 ServerResultCode { get; set; } = 0;
        public object ServerResultDataObject { get; set; } = new object();
        public IList ServerResultDataList { get; set; }

    }
}
