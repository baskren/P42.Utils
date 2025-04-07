using Microsoft.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Web.WebView2.Core;
using P42.Utils;
using P42.Utils.Uno;
using Shouldly;
using Uno.UI.RuntimeTests;

namespace P42.Utils.Tests;

[TestClass]
public class A03_LocalData_ResourceItem
{
    private const string ResourceId = ".Resources.TextFile1.txt";
    private const string ExpectedContent = "THIS IS A TEXT FILE\r\n";
    private static LocalData.ResourceItem ResourceItem = LocalData.ResourceItem.Get(ResourceId);
    private static LocalData.ResourceItem AltResourceItemA = LocalData.ResourceItem.Get(ResourceId, "AltResources");
    private static LocalData.ResourceItem AltResourceItemB = LocalData.ResourceItem.Get(".Resources.html5-test-page.html");

#if !BROWSERWASM
    [TestMethod]
    public void A00_IntersessionCaching()
    {
        if (!ResourceItem.Exists)
            throw new Exception("Should ONLY fail upon first run of test on a given platform.  Except WASM, if you see two times in a row, there is an error.");
        ResourceItem.RecallText().ShouldBe(ExpectedContent);
    }
#endif

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
        Assert.ThrowsException<ArgumentException>(() => LocalData.ResourceItem.Get(ResourceId, "AltResources", typeof(P42.Utils.Uno.Platform).GetAssembly()));
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

}
