using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace P42.Utils.AppTest;

[TestClass]
public class A08_StreamExtensions
{
    private const string ResourceId = ".Resources.TextFile1.txt";
    private const string ExpectedContent = "THIS IS A TEXT FILE\n";
    private static LocalData.ResourceItem ResourceItem = LocalData.ResourceItem.For(ResourceId);

    [TestMethod]
    public void A00_Init()
    {
        ResourceItem.TryAssureExists();
        ResourceItem.Exists.ShouldBeTrue();
        ResourceItem.RecallText().ShouldBe(ExpectedContent);
    }

    [TestMethod]
    public void A01_CopyToPath()
    {
        //var path = Path.Combine(ApplicationData.Current.TemporaryFolder.Path, "TestFile1");
        var path = Path.Combine(Platform.ApplicationTemporaryFolderPath, "TestFile1");
        using var stream = ResourceItem.Stream(FileMode.OpenOrCreate);
        stream.CopyToPath(path);
        stream.Dispose();

        var text = File.ReadAllText(path);
        text.ShouldBe(ExpectedContent);
    }
}
