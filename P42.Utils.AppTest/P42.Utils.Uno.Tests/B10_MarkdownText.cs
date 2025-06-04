using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P42.UnoTestRunner;
using P42.Utils.Uno;

namespace P42.Utils.AppTest;

[TestClass]
internal class B10_MarkdownText
{


    string? _markdownSample;
    string MarkdownSample
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_markdownSample))
                return _markdownSample;
            var item = P42.Utils.LocalData.ResourceItem.Get(".markdown-it.md", assembly: GetType().Assembly);
            item.TryAssurePulled();
            _markdownSample = item.RecallText();
            return _markdownSample;
        }
    }

    string? _markdownCss;
    string MarkdownCss
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_markdownCss))
                return _markdownCss;
            var item = P42.Utils.LocalData.ResourceItem.Get(".github-markdown.css", assembly: GetType().Assembly);
            item.TryAssurePulled();
            _markdownCss = item.RecallText();
            return _markdownCss;

        }
    }

    [TestMethod]
    public void A01_TextBlock()
    {
        var textBlock = new TextBlock
        {
            TextWrapping = TextWrapping.WrapWholeWords,

        };

        textBlock.Markdown(MarkdownSample);

        UnitTestsUIContentHelper.Content = new ScrollViewer()
        {
            Content = textBlock
        };
    }

    [TestMethod]
    public void A02_MarkdownControl()
    {
        var control = new P42.Utils.Uno.MarkDownControl();
        control.MarkdownText = MarkdownSample;
        UnitTestsUIContentHelper.Content = control;
    }
}
