using P42.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace P42_Utils_Tests_Wasm;

public sealed partial class MainPage : Page
{
    WebView wv = new WebView();

    public MainPage()
    {
        this.InitializeComponent();

        Grid.SetRow(wv, 2);
        wv.HorizontalAlignment = HorizontalAlignment.Stretch;
        wv.VerticalAlignment = VerticalAlignment.Stretch;
        grid.Children.Add(wv);  
    }

    private async void startTestButton_Click(object sender, RoutedEventArgs e)
    {

        //await wv.EnsureCoreWebView2Async();
        //wv.Source = new Uri("https://platform.uno");
        //wv.NavigateToString("THIS IS A TEST");

        var resourceItem = P42.Utils.LocalData.ResourceItem.Get(".Resources.html5-test-page.html");
        resourceItem.TryAssurePulled();
        wv.Source = resourceItem.AppDataUri;
    }
}
