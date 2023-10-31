namespace Budget_Planner.pages;

public partial class More : ContentPage
{
    public List<TestObject> list { get; set; } = new List<TestObject>();



    public More()
	{
		InitializeComponent();

        Int32 i = 0;
        while(i < 10)
        {
            list.Add(new TestObject());
            i++;
        }

    }
}

public class TestObject
{
    public string name { get; set; } = "Hello World";
    public Int32 age { get; set; } = 1;
}

public class CustomViewCell : Microsoft.Maui.Controls.ViewCell
{

    public static readonly BindableProperty SelectedBackgroundColorProperty = BindableProperty.Create(
        nameof(SelectedBackgroundColor), typeof(Color), typeof(CustomViewCell), Colors.White
    );

    public Color SelectedBackgroundColor
    {
        get { return (Color)GetValue(SelectedBackgroundColorProperty); }
        set { SetValue(SelectedBackgroundColorProperty, value); }
    }

    public CustomViewCell()
    {

    }
}