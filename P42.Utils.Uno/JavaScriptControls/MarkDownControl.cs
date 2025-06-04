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
        var resource = P42.Utils.EmbeddedResourceExtensions.FindAssemblyResourceIdAndStream(StyleSheetReourceId, GetType().Assembly)
                        ?? throw new ArgumentException($"Cannot find resourceId [{StyleSheetReourceId}] in provided assembly [{GetType().Assembly?.Name() ?? "null"}]");
        var styleCss = resource.DisposableStream.ReadToEnd();
        resource.DisposableStream.Dispose();
        StyleSheets.Add(styleCss);
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
        var resource = P42.Utils.EmbeddedResourceExtensions.FindAssemblyResourceIdAndStream(resourceId, asm) 
            ?? throw new ArgumentException($"Cannot find resourceId [{resourceId}] in provided assembly [{asm?.Name() ?? "null"}]");
        MarkdownText = resource.DisposableStream.ReadToEnd();
        resource.DisposableStream.Dispose();
    }

    #endregion

}
