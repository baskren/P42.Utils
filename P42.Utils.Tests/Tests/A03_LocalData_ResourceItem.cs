using Microsoft.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Web.WebView2.Core;
using P42.Utils.Uno;
using Shouldly;
using Uno.UI.RuntimeTests;

namespace P42.Utils.Tests;

[TestClass]
public class A03_LocalData_ResourceItem
{
    private const string ResourceId = ".Resources.TextFile1.txt";
    private const string ExpectedContent = "THIS IS A TEXT FILE\r\n";
    private static LocalData.ResourceItemKey ResourceKey = LocalData.ResourceItemKey.Get(ResourceId);
    private static LocalData.ResourceItemKey AltResourceKeyA = LocalData.ResourceItemKey.Get(ResourceId, "AltResources");
    private static LocalData.ResourceItemKey AltResourceKeyB = LocalData.ResourceItemKey.Get(".Resources.html5-test-page.html");

#if !BROWSERWASM
    [TestMethod]
    public void A00_IntersessionCaching()
    {
        if (!ResourceKey.Exists)
            throw new Exception("Should ONLY fail upon first run of test on a given platform.  Except WASM, if you see two times in a row, there is an error.");
        LocalData.Text.TryRecallItem(out var item, ResourceKey).ShouldBeTrue();
        item.ShouldBe(ExpectedContent);
    }
#endif

    [TestMethod]
    public void A01_KeyEquality()
    {
        var key = P42.Utils.LocalData.ResourceItemKey.Get(ResourceId);
        key.FullPath.ShouldBe(ResourceKey.FullPath);
        key.ShouldBe(ResourceKey);

        key.FullPath.ShouldNotBe(AltResourceKeyA.FullPath);
        key.ShouldNotBe(AltResourceKeyA);
        key.FullPath.ShouldNotBe(AltResourceKeyB.FullPath);
        key.ShouldNotBe(AltResourceKeyB);
    }

    [TestMethod]
    public void A02_ClearAndPull()
    {
        ResourceKey.Clear();
        ResourceKey.Exists.ShouldBeFalse();
        LocalData.Text.TryRecallItem(out var item, ResourceKey).ShouldBeFalse();
        ResourceKey.TryRecallOrPullItem().ShouldBe(true);
        ResourceKey.Exists.ShouldBeTrue();
    }

    [TestMethod]
    public void A03_Recall()
    {
        ResourceKey.Exists.ShouldBeTrue();
        LocalData.Text.TryRecallItem(out var item, ResourceKey).ShouldBeTrue();
        item.ShouldBe(ExpectedContent);
    }

    [TestMethod]
    public async Task A04_VerifyAppDataUri()
    {
        var file = await StorageFile.GetFileFromApplicationUriAsync(ResourceKey.AppDataUri);
        var content = await FileIO.ReadTextAsync(file);
        content.ShouldBe(ExpectedContent);
    }

    [TestMethod]
    public async Task A05_VerifyFullPath()
    {
        var file = await StorageFile.GetFileFromPathAsync(ResourceKey.FullPath);
        var content = await FileIO.ReadTextAsync(file);
        content.ShouldBe(ExpectedContent);
    }

    [TestMethod]
    public async Task A06_VerifyStreamReader()
    {
        LocalData.StreamReader.TryRecallItem(out var reader, ResourceKey).ShouldBeTrue();
        reader.ShouldNotBeNull();
        var text = await reader.ReadToEndAsync();
        text.ShouldBe(ExpectedContent);
        reader.Close();
        reader.Dispose();
    }

    [TestMethod]
    public async Task A07_VerifyStreamWriter()
    {
        ResourceKey.Clear();
        ResourceKey.Exists.ShouldBeFalse();

        LocalData.StreamWriter.TryRecallItem(out var writer, ResourceKey).ShouldBeTrue();
        writer.ShouldNotBeNull();
        await writer.WriteAsync(ExpectedContent);
        writer.Close();
        writer.Dispose();

        ResourceKey.Exists.ShouldBeTrue();
        LocalData.Text.TryRecallItem(out var item, ResourceKey).ShouldBeTrue();
        item.ShouldBe(ExpectedContent);

    }

    [TestMethod]
    public void A08_NonExistantResource()
    {
        Assert.ThrowsException<ArgumentException>(() => LocalData.ResourceItemKey.Get(ResourceId, "AltResources", typeof(P42.Utils.Uno.Platform).GetAssembly()));
        Assert.ThrowsException<ArgumentException>(() => LocalData.ResourceItemKey.Get(".Resources.AltResources") );
    }

    [TestMethod]
    public void A09_AltResources()
    {
        ResourceKey.ShouldNotBe(AltResourceKeyA);
        ResourceKey.ShouldNotBe(AltResourceKeyB);
        AltResourceKeyB.ShouldNotBe(AltResourceKeyA);

        AltResourceKeyA.TryRecallOrPullItem().ShouldBeTrue();
        AltResourceKeyB.TryRecallOrPullItem().ShouldBeTrue();

        LocalData.Text.TryRecallItem(out var item, ResourceKey).ShouldBeTrue();
        item.ShouldBe(ExpectedContent);

        LocalData.Text.TryRecallItem(out var altItemA, AltResourceKeyA).ShouldBeTrue();
        altItemA.ShouldBe(ExpectedContent);

        LocalData.Text.TryRecallItem(out var altItemB, AltResourceKeyB).ShouldBeTrue();
        altItemB.ShouldNotBe(ExpectedContent);
    }
}
