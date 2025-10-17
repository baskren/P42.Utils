using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using Windows.Data.Pdf;

namespace P42.Utils.Uno;

public partial class PdfViewerControl : HtmlControl
{
    #region Properties

    #region PdfBytes Property
    public static readonly DependencyProperty PdfBytesProperty = DependencyProperty.Register(
        nameof(PdfBytes),
        typeof(byte[]),
        typeof(PdfViewerControl),
        new PropertyMetadata(default(byte[]), (d,e) => ((PdfViewerControl)d).OnPdfBytesChanged(e))
    );

    private void OnPdfBytesChanged(DependencyPropertyChangedEventArgs e)
    {
        
    }

    public byte[] PdfBytes
    {
        get => (byte[])GetValue(PdfBytesProperty);
        set => SetValue(PdfBytesProperty, value);
    }
    #endregion PdfBytes Property

    protected override string HtmlBody => $@"<canvas id = ""{HtmlContentId}""></canvas>";

    #endregion


    #region Fields
    string PdfWorkerScript = string.Empty;
    string PdfMinScript = string.Empty;
    #endregion

    public PdfViewerControl()
    {
        var asm = GetType().Assembly;
        if (EmbeddedResourceExtensions.TryGetText(out var script1, ".pdf.worker.min.js", asm)) 
            PdfWorkerScript = script1;
        if (EmbeddedResourceExtensions.TryGetText(out var script2, ".pdf.min.js", asm))
            PdfMinScript = script2;
    }

    protected override async Task UpdateBodyAsync()
    => await DisplayPdfAsync();

    async Task DisplayPdfAsync()
    {
        if (PdfBytes == null || PdfBytes.Length == 0)
        {
            var script = 
$"""
var canvas = document.getElementById('{HtmlContentId}');
var context = canvas.getContext('2d');
context.clearRect(0,0,canvas.width, canvas.height);                
""";
            return;
        }

        await InvokeScriptAsync(PdfWorkerScript);
        await InvokeScriptAsync(PdfMinScript);

        if (!EmbeddedResourceExtensions.TryGetText(out var js, ".contentpdf.js"))
            return;
        if (!EmbeddedResourceExtensions.TryGetBytes(out var pdfBytes, ".sampledocpage1.pdf"))
            return;

        var pdfAsString = Convert.ToBase64String(PdfBytes);
        js = js.Replace("the-canvas", HtmlContentId);
        js = js.Replace("##CONTENT##", pdfAsString);

        await InvokeScriptAsync(js, false);

    }

    protected override void OnWebViewMessageReceived(WebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
    {
        var message = args.Source;
        var json = Regex.Unescape(args.WebMessageAsJson);
        var objs = args.AdditionalObjects;
    }
}
