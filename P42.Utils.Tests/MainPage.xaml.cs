namespace P42.Utils.Tests;

public sealed partial class MainPage : Page
{
    // https://github.com/unoplatform/uno.ui.runtimetests.engine

    public MainPage()
    {
        this.InitializeComponent();

    }

    private async void runButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await P42.UnoTestRunner.TestRunner.Run();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
        }
    }
}
