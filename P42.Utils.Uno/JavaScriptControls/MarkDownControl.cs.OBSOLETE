using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Markdig;

namespace P42.Utils.Uno;

[Microsoft.UI.Xaml.Data.Bindable]
public partial class MarkDownControl : HtmlControl
{
    #region Properties
    public static readonly DependencyProperty MarkdownTextProperty =
        DependencyProperty.Register("MarkdownText", typeof(string), typeof(MarkDownControl), new PropertyMetadata(null, (d,e) => ((MarkDownControl)d).UpdateBodyAsync().SafeFireAndForget()));
    public string MarkdownText
    {
        get { return (string)GetValue(MarkdownTextProperty); }
        set { SetValue(MarkdownTextProperty, value); }
    }

    protected override string HtmlBody => $@"<div class=""markdown-body"" id = ""{HtmlContentId}""><p>CONTENT GOES HERE</p></ div>";

    string StyleSheetReourceId => "P42.Utils.Uno.Resources.github-markdown.css";
    #endregion


    #region Fields
    MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseEmojiAndSmiley()
        .UseAutoLinks()
        .UseListExtras()
        .Build();
    #endregion


    #region Events
    WeakEventManager<EventArgs>? _markedLoaded;
    public WeakEventManager<EventArgs> MarkedLoaded => _markedLoaded ??= new WeakEventManager<EventArgs>();
    #endregion

    public MarkDownControl()
    {
        if (EmbeddedResourceExtensions.TryGetText(out var css, StyleSheetReourceId, GetType().Assembly))
            StyleSheets.Add(css);
    }

    #region Event Handlers
    protected override async Task UpdateBodyAsync()
        => await DisplayMarkdown(MarkdownText);
    
    private async Task DisplayMarkdown(string markdown)
    {
        if (!IsBodyLoaded)
            return;

        var html = Markdig.Markdown.ToHtml(markdown, pipeline);
        html = html.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\"", "\\\"").Replace("\'", "\\\'");//.Replace("\t","\\t").Replace("`","");
        await UpdateBodyFromScript($"`{html}`");
        MarkedLoaded.RaiseEvent(this, EventArgs.Empty, nameof(MarkedLoaded));
    }

    public void LoadMarkdownFromResource(string resourceId, Assembly? asm = null)
    {
        if (EmbeddedResourceExtensions.TryGetText(out var value, resourceId, asm))
            MarkdownText = value;
    }

    #endregion

}
