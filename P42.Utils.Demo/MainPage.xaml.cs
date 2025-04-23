namespace P42.Utils.Demo;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();


        P42.UnoTestRunner.TestRunnerExtensions.GetTestMethods();
    }
}
