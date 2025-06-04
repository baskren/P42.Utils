using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P42.UnoTestRunner;

namespace P42.Utils.AppTest;

[TestClass]
internal class B11_PdfViewerControl
{
    [TestMethod]
    [RunsOnUIThread]
    public void A00_ShowPdf()
    {
        var control = new P42.Utils.Uno.PdfViewerControl();
        UnitTestsUIContentHelper.Content = control;

        if (EmbeddedResourceExtensions.TryGetBytes(out var bytes, ".sampledocpage1.pdf", GetType().Assembly))
            control.PdfBytes = bytes;
    }
}
