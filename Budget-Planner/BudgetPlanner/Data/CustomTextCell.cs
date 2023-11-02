using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget_Planner.BudgetPlanner.Data
{
    public class CustomTextCell : Microsoft.Maui.Controls.TextCell
    {

        public static readonly BindableProperty SelectedBackgroundColorProperty = BindableProperty.Create(
            nameof(SelectedBackgroundColor), typeof(Color), typeof(CustomTextCell), Colors.White
        );

        public Color SelectedBackgroundColor
        {
            get { return (Color)GetValue(SelectedBackgroundColorProperty); }
            set { SetValue(SelectedBackgroundColorProperty, value); }
        }

        public CustomTextCell()
        {

        }
    }
}
