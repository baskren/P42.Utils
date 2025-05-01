using System.Runtime.InteropServices;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Web.WebView2.Core;
using P42.UnoTestRunner;
using P42.Utils;
using P42.Utils.Uno;
using Shouldly;
using Uno.Foundation;

namespace P42.Utils.Demo;

[TestClass]
public class A03_LocalData_ResourceItem
{
    private const string ResourceId = ".Resources.TextFile1.txt";
    private const string ExpectedContent = "THIS IS A TEXT FILE\r\n";
    private static LocalData.ResourceItem ResourceItem = LocalData.ResourceItem.Get(ResourceId);
    private static LocalData.ResourceItem AltResourceItemA = LocalData.ResourceItem.Get(ResourceId, "AltResources");
    private static LocalData.ResourceItem AltResourceItemB = LocalData.ResourceItem.Get(".Resources.html5-test-page.html");

    [TestMethod]
    public void A00_IntersessionCaching()
    {
        if (!ResourceItem.Exists)
            throw new Exception("Should ONLY fail upon first run of test on a given platform.  Except WASM, if you see two times in a row, there is an error.");
        ResourceItem.RecallText().ShouldBe(ExpectedContent);
    }


    [TestMethod]
    public void A01_KeyEquality()
    {
        var item = P42.Utils.LocalData.ResourceItem.Get(ResourceId);
        item.FullPath.ShouldBe(ResourceItem.FullPath);
        item.ShouldBe(ResourceItem);

        item.FullPath.ShouldNotBe(AltResourceItemA.FullPath);
        item.ShouldNotBe(AltResourceItemA);
        item.FullPath.ShouldNotBe(AltResourceItemB.FullPath);
        item.ShouldNotBe(AltResourceItemB);
    }

    [TestMethod]
    public async Task A02_ClearAndPull()
    {
        ResourceItem.Clear();
        ResourceItem.Exists.ShouldBeFalse();
        Assert.ThrowsException<System.IO.FileNotFoundException> (ResourceItem.RecallText);
        ResourceItem.TryRecallText(out var _).ShouldBe(false);
        var text = await ResourceItem.AssureSourcedTextAsync();
        ResourceItem.Exists.ShouldBeTrue();
        text.ShouldBe(ExpectedContent);

        ResourceItem.Clear();
        ResourceItem.Exists.ShouldBeFalse();
        await ResourceItem.TryAssurePulledAsync();
        ResourceItem.Exists.ShouldBeTrue();
        text = ResourceItem.RecallText();
        text.ShouldBe(ExpectedContent);
    }

    [TestMethod]
    public void A03_Recall()
    {
        ResourceItem.Exists.ShouldBeTrue();
        ResourceItem.TryRecallText(out var text).ShouldBe(true);
        text.ShouldBe(ExpectedContent);
    }

    [TestMethod]
    public async Task A04_VerifyAppDataUri()
    {
        var file = await StorageFile.GetFileFromApplicationUriAsync(ResourceItem.AppDataUri);
        var content = await FileIO.ReadTextAsync(file);
        content.ShouldBe(ExpectedContent);
    }

    [TestMethod]
    public async Task A05_VerifyFullPath()
    {
        var file = await StorageFile.GetFileFromPathAsync(ResourceItem.FullPath);
        var content = await FileIO.ReadTextAsync(file);
        content.ShouldBe(ExpectedContent);
    }

    [TestMethod]
    public async Task A06_VerifyStreamReader()
    {
        ResourceItem.TryStreamReader(out var reader).ShouldBeTrue();
        reader.ShouldNotBeNull();
        var text = await reader.ReadToEndAsync();
        text.ShouldBe(ExpectedContent);
        reader.Close();
        reader.Dispose();
    }

    [TestMethod]
    public async Task A07_VerifyStreamReader()
    {
        using var reader = ResourceItem.StreamReader();
        reader.ShouldNotBeNull();
        var text = await reader.ReadToEndAsync();
        text.ShouldBe(ExpectedContent);
    }


    [TestMethod]
    public async Task A08_VerifyStreamWriter()
    {
        ResourceItem.Clear();
        ResourceItem.Exists.ShouldBeFalse();

        ResourceItem.TryStreamWriter(out var writer).ShouldBeTrue(); 
        writer.ShouldNotBeNull();
        await writer.WriteAsync(ExpectedContent);
        writer.Close();
        writer.Dispose();

        ResourceItem.Exists.ShouldBeTrue();
        ResourceItem.TryRecallText(out var item).ShouldBeTrue();
        item.ShouldBe(ExpectedContent);
    }

    [TestMethod]
    public async Task A09_VerifyStreamWriter()
    {
        ResourceItem.Clear();
        ResourceItem.Exists.ShouldBeFalse();

        var writer = ResourceItem.StreamWriter();
        writer.ShouldNotBeNull();
        await writer.WriteAsync(ExpectedContent);
        writer.Close();
        writer.Dispose();

        ResourceItem.Exists.ShouldBeTrue();
        ResourceItem.TryRecallText(out var item).ShouldBeTrue();
        item.ShouldBe(ExpectedContent);
    }

    [TestMethod]
    public void A10_NonExistantResource()
    {
        Assert.ThrowsException<ArgumentException>(() => LocalData.ResourceItem.Get(ResourceId, "AltResources", typeof(P42.Utils.Uno.Platform).Assembly));
        Assert.ThrowsException<ArgumentException>(() => LocalData.ResourceItem.Get(".Resources.AltResources") );
    }



    [TestMethod]
    public async Task A11_AltResources()
    {
        ResourceItem.ShouldNotBe(AltResourceItemA);
        ResourceItem.ShouldNotBe(AltResourceItemB);
        AltResourceItemB.ShouldNotBe(AltResourceItemA);

        AltResourceItemA.TryAssurePulled().ShouldBeTrue();
        (await AltResourceItemB.TryAssurePulledAsync()).ShouldBeTrue();

        ResourceItem.TryRecallText(out var item).ShouldBeTrue();
        item.ShouldBe(ExpectedContent);

        AltResourceItemA.TryRecallText(out var altItemA).ShouldBeTrue();
        altItemA.ShouldBe(ExpectedContent);

        AltResourceItemB.TryRecallText(out var altItemB).ShouldBeTrue();
        altItemB.ShouldNotBe(ExpectedContent);
    }

//#if BROWSERWASM
    [TestMethod]
    public async Task A11_Beep()
    {
        await DeviceBeep.PlayAsync(1500, 300);
        Console.WriteLine("A11_Beep done");
    }
//#endif

    [TestMethod]
    [RunsOnUIThread]
    public async Task A12_WebView2_Source()
    {
        var rect = new Microsoft.UI.Xaml.Shapes.Rectangle
        {
            Fill = Microsoft.UI.Colors.Pink.ToBrush(),
            Width = 500,
            Height = 500
        };
        Grid.SetRow(rect, 1);
        var wv2 = new WebView2
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };
        Grid.SetRow(wv2, 1);

        var button = new Button
        {
            Content = $"THIS IS A BUTTON : {GetType()}"
        };


        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50, GridUnitType.Pixel) });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        grid.Children.Add(rect);
        grid.Children.Add(button);
        grid.Children.Add(wv2);
        UnitTestsUIContentHelper.Content = grid;

#if !BROWSERWASM
        await UnitTestsUIContentHelper.WaitForLoaded(grid);
        await UnitTestsUIContentHelper.WaitForIdle();
#endif

        var resource = LocalData.ResourceItem.Get(".Resources.html5-test-page.html");
        var html = resource.AssureSourcedText();
        string.IsNullOrWhiteSpace(html).ShouldBeFalse();
        var filePath = resource.FullPath;


        var asmName = GetType().Assembly.Name();    
        resource.AppDataUri.ShouldBe(new Uri($"ms-appdata:///local/{typeof(LocalData).Assembly.Name()}.{nameof(LocalData)}/{asmName}/{asmName}.Resources.html5-test-page.html"));
        var file = await StorageFile.GetFileFromApplicationUriAsync(resource.AppDataUri);
        var content = await FileIO.ReadTextAsync(file);
        content.ShouldBe(html);


        //wv2.Source = new Uri("https://platform.uno");
        //wv2.NavigateToString(resource.RecallText());
        /*
        var destUri = new Uri("ms-appdata:///Local/index.html");
        var destFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("index.html", CreationCollisionOption.ReplaceExisting);
        await FileIO.WriteTextAsync(destFile, content);
        */
        var tcs = new TaskCompletionSource<bool>();

        void Wv2_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            Task.Run(async () =>
            {
                // await DeviceBeep.PlayAsync(1500, 300);
                tcs.TrySetResult(true);
            });
        }

        void Wv2_CoreProcessFailed(WebView2 sender, CoreWebView2ProcessFailedEventArgs args)
        {
            Task.Run(async () =>
            {
                // await DeviceBeep.PlayAsync(100, 30000);
                tcs.TrySetResult(false);
            });
        }



         await wv2.EnsureCoreWebView2Async();

        wv2.NavigationCompleted += Wv2_NavigationCompleted;
        wv2.CoreProcessFailed += Wv2_CoreProcessFailed;

        //wv2.Source = destUri;  // somehow, this isn't working in this app but seems to in a simple app
        //wv2.NavigateToString(content);
        wv2.Source = resource.FileUri;

        await tcs.Task;

        /*
        _ = await wv2.ExecuteScriptAsync("document.body.replaceChildren();"); // this works
        _ = await wv2.ExecuteScriptAsync("document.body.innerHtml = \"\";"); // and this works
        var result = await wv2.ExecuteScriptAsync("document.body.textContent;"); // this works!
        */

        Console.WriteLine($"END OF [{nameof(A12_WebView2_Source)}] ");

#if !BROWSERWASM  // DOES NOT WORK IN CURRENT 
        var result = await wv2.ExecuteScriptAsync("document.body.outerHTML;"); // this works!
        Console.WriteLine($"TEST {nameof(A12_WebView2_Source)}: outerHtml=[{result}]");
        string.IsNullOrWhiteSpace(result).ShouldBeFalse();
        Assert.AreNotEqual(result, "null"); // this should work but, for some reason, it is not.  I have no clue
        result.ShouldContain("This is a test page filled with common HTML elements to be used to provide visual feedback whilst building CSS systems and frameworks.");
#endif
    }

}
