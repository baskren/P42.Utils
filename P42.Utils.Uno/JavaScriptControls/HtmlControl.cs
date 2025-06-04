using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Microsoft.UI;
using Microsoft.Web.WebView2.Core;

namespace P42.Utils.Uno;

[Microsoft.UI.Xaml.Data.Bindable]
public abstract partial class HtmlControl : UserControl
{
    #region Properties
    protected virtual string HtmlBody => $@"<div class=""markdown-body"" id = ""{HtmlContentId}""><p>CONTENT GOES HERE</p></ div>";

    bool _isBodyLoaded;
    public bool IsBodyLoaded 
    { 
        get => _isBodyLoaded; 
        private set
        {
            if (value && _isBodyLoaded == false)
            {
                _isBodyLoaded = true;
                BodyLoaded.RaiseEvent(this, EventArgs.Empty, nameof(BodyLoaded));
            }
        }
    }

    public readonly List<string> StyleSheets = new List<string>();

    public readonly List<string> Scripts = new List<string>();
    #endregion


    #region Fields
    protected readonly string HtmlContentId = "content-" + Guid.NewGuid().ToString();
    private readonly WebView2 internalWebView;
    #endregion


    #region Events
    WeakEventManager<EventArgs>? _bodyLoaded;
    public WeakEventManager<EventArgs> BodyLoaded => _bodyLoaded ??= new WeakEventManager<EventArgs>();
    #endregion

    public HtmlControl()
    {
        Content = internalWebView = new WebView2
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };
        internalWebView.DefaultBackgroundColor = Colors.Transparent;
        internalWebView.NavigationCompleted += OnNavigationCompleted;

        Loaded += OnControlLoaded;
    }

    private async void OnControlLoaded(object sender, RoutedEventArgs e)
    => await LoadBasePageAsync();

    private async Task LoadBasePageAsync()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var stylesheet in StyleSheets)
            sb.Append($"\t\t<style type=\"text/css\" >{stylesheet}</style>\n");
        var styleSheets = sb.ToString();

        sb.Clear();
        foreach (var script in Scripts)
            sb.Append($"\t\t<script type=\"text/javascript\">{script}</script>\n");
        var scripts = sb.ToString();

        sb.Clear();
        sb.Append("<html>\n\t<head>\n");
        sb.Append(styleSheets);
        sb.Append(scripts);
        sb.Append($@"</head>
     <body>
        <div class=""markdown-body"">
         {HtmlBody}
        </div>
    </ body>
    </ html>
");
        var html = sb.ToString();
        await internalWebView.EnsureCoreWebView2Async();
        internalWebView.NavigateToString(html);
    }

    private async void OnNavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
    {
#if __ANDROID__
        var wv = internalWebView.GetType()
            .GetField("_webView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(internalWebView) as Android.Webkit.WebView;
        wv.SetBackgroundColor(Android.Graphics.Color.Transparent);
#endif

        await LoadDefaultStyleSheetAsync();
        IsBodyLoaded = true;
        await UpdateBodyAsync();
    }

    protected abstract Task UpdateBodyAsync();

    protected async Task UpdateBodyFromScript(string contentScript)
    {
        if (Foreground is SolidColorBrush colorBrush)
        {
            var color = colorBrush.Color;
            // This is required because default ToString on wasm doesn't come out in the format #RRGGBB or even #AARRGGBB
            var colorString = $"#{color.R.ToString("X")}{color.G.ToString("X")}{color.B.ToString("X")}";
            Console.WriteLine($"Color {colorString}");
            var colorScript = $@"document.getElementById('{HtmlContentId}').style.color = '{colorString}';";
            await InvokeScriptAsync(colorScript);
        }

        if (Background is SolidColorBrush background)
        {
            var color = background.Color;
            // This is required because default ToString() on wasm doesn't come out in the format #RRGGBB or even #AARRGGBB
            var colorString = $"#{color.R.ToString("X")}{color.G.ToString("X")}{color.B.ToString("X")}";
            Console.WriteLine($"Color {colorString}");
            var colorScript = $@"document.getElementById('{HtmlContentId}').style.background-color = '{colorString}';";
            await InvokeScriptAsync(colorScript);
        }

        
        var script = $@"document.getElementById('{HtmlContentId}').innerHTML = {contentScript};";
        await InvokeScriptAsync(script);

    }

    public async Task<string> InvokeScriptAsync(string scriptToRun, bool resizeAfterScript = true)
    {
        //var source = new CancellationTokenSource();
        System.Diagnostics.Debug.WriteLine($"SCRIPT: {scriptToRun}");
        var result = await internalWebView.ExecuteScriptAsync(scriptToRun).AsTask();
        if (resizeAfterScript)
        {
            await ResizeToContent();
        }
        System.Diagnostics.Debug.WriteLine($"RESULT: {System.Text.RegularExpressions.Regex.Unescape(result)}");
        return result ?? string.Empty;

    }

    public async Task ResizeToContent()
    {
        var documentRoot = $"document.getElementById('{HtmlContentId}')";
        var heightString = await InvokeScriptAsync($"{documentRoot}.scrollHeight.toString()",
            false);
        int height;
        if (int.TryParse(heightString, out height))
            this.Height = height + 100;

    }

    protected virtual Task LoadDefaultStyleSheetAsync()
        => Task.CompletedTask;

    // StyleSheet ??= (await GetEmbeddedFileStreamAsync(GetType(), "github-markdown.css")).ReadToEnd();
    
    protected async Task ApplyStyleSheetAsync()
    {

    }

}
