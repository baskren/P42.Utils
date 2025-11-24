using System.ComponentModel;
using Windows.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using ElementType = Microsoft.UI.Xaml.Controls.TextBlock;
using System.Runtime.CompilerServices;
using Markdig;

namespace P42.Utils.Uno;

[Microsoft.UI.Xaml.Data.Bindable]
public static class TextBlockExtensions
{
    #region Link Tapped Property
    /// <summary>
    /// LinkTapped Attached Dependency Property
    /// </summary>
    public static readonly DependencyProperty LinkTappedProperty =
        DependencyProperty.RegisterAttached("LinkTapped", typeof(Action<string,string>), typeof(TextBlockExtensions), new PropertyMetadata(null));

    /// <param name="textBlock"></param>
    extension(ElementType textBlock)
    {
        /// <summary>
        /// Link Tapped Handler Setter
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ElementType SetLinkTappedHandler(Action<string, string> value)
        {
            textBlock.SetValue(LinkTappedProperty, value);
            return textBlock;
        }

        /// <summary>
        /// Link Tapped Handler Getter
        /// </summary>
        /// <returns></returns>
        public Action<string, string> GetLinkTappedHandler()
            => (Action<string,string>)textBlock.GetValue(LinkTappedProperty);
    }

    #endregion


    #region Markdown Property
    /// <summary>
    /// HTML Attached Dependency Property
    /// </summary>
    public static readonly DependencyProperty MarkdownProperty =
        DependencyProperty.RegisterAttached("Markdown", typeof(string), typeof(TextBlockExtensions), new PropertyMetadata(null, MarkdownChanged));

    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseEmojiAndSmiley()
        .UseAutoLinks()
        //.UseBootstrap()
        //.UseGenericAttributes()
        .UseListExtras()
        //.UsePragmaLines()
        //.UseReferralLinks()
        //.UseSelfPipeline()
        //.UseSmartyPants()
        //.UseYamlFrontMatter()
        .Build();
    private static void MarkdownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ElementType textBlock)
            return;

        var markdown = (string)e.NewValue ?? string.Empty;

        var html = Markdig.Markdown.ToHtml(markdown, Pipeline);
        textBlock.Html(html);
    }

    /// <param name="textBlock"></param>
    extension(ElementType textBlock)
    {
        /// <summary>
        /// HTML Setter
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public ElementType SetMarkdown(string? value)
        {
            textBlock.SetValue(MarkdownProperty, value ?? string.Empty);
            return textBlock;
        }

        /// <summary>
        /// Markdown Getter
        /// </summary>
        /// <returns></returns>
        public string GetMarkdown()
            => (string)textBlock.GetValue(MarkdownProperty);

        /// <summary>
        /// Markdown Setter Extension Method
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ElementType Markdown(string value)
        { textBlock.SetMarkdown(value); return textBlock; }

        /// <summary>
        /// Markdown Work-around Binding 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourcePropertyName"></param>
        /// <param name="mode"></param>
        /// <param name="converter"></param>
        /// <param name="converterParameter"></param>
        /// <param name="converterLanguage"></param>
        /// <param name="updateSourceTrigger"></param>
        /// <param name="targetNullValue"></param>
        /// <param name="fallbackValue"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        public ElementType AltBindMarkdown(INotifyPropertyChanged source,
            string sourcePropertyName,
            BindingMode mode = BindingMode.OneWay,
            IValueConverter? converter = null,
            object? converterParameter = null,
            string? converterLanguage = null,
            UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
            object? targetNullValue = null,
            object? fallbackValue = null,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1
        )
        {
            textBlock.AltBind(MarkdownProperty, source, sourcePropertyName, mode, converter, converterParameter, converterLanguage, updateSourceTrigger, targetNullValue, fallbackValue, filePath, lineNumber);
            return textBlock;
        }

        /// <summary>
        /// Markdown Work-around Binding
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceProperty"></param>
        /// <param name="mode"></param>
        /// <param name="converter"></param>
        /// <param name="converterParameter"></param>
        /// <param name="converterLanguage"></param>
        /// <param name="updateSourceTrigger"></param>
        /// <param name="targetNullValue"></param>
        /// <param name="fallbackValue"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        public ElementType AltBindMarkdown(DependencyObject source,
            DependencyProperty sourceProperty,
            BindingMode mode = BindingMode.OneWay,
            IValueConverter? converter = null,
            object? converterParameter = null,
            string? converterLanguage = null,
            UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
            object? targetNullValue = null,
            object? fallbackValue = null,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1
        )
        {
            textBlock.AltBind(MarkdownProperty, source, sourceProperty, mode, converter, converterParameter, converterLanguage, updateSourceTrigger, targetNullValue, fallbackValue, filePath, lineNumber);
            return textBlock;
        }

        /// <summary>
        /// Markdown Work-around Binding
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceProperty"></param>
        /// <param name="mode"></param>
        /// <param name="convert"></param>
        /// <param name="convertBack"></param>
        /// <param name="converterParameter"></param>
        /// <param name="converterLanguage"></param>
        /// <param name="updateSourceTrigger"></param>
        /// <param name="targetNullValue"></param>
        /// <param name="fallbackValue"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public ElementType AltBindMarkdown<TSource>(DependencyObject source,
            DependencyProperty sourceProperty,
            BindingMode mode = BindingMode.OneWay,
            Func<TSource?, string?>? convert = null,
            Func<string?, TSource?>? convertBack = null,
            object? converterParameter = null,
            string? converterLanguage = null,
            UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
            object? targetNullValue = null,
            object? fallbackValue = null,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1
        )
        {
            textBlock.AltBind(MarkdownProperty, source, sourceProperty, mode, convert, convertBack, converterParameter, converterLanguage, updateSourceTrigger, targetNullValue, fallbackValue, filePath, lineNumber);
            return textBlock;
        }

        /// <summary>
        /// Markdown Work-around Binding
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourcePropertyName"></param>
        /// <param name="mode"></param>
        /// <param name="convert"></param>
        /// <param name="convertBack"></param>
        /// <param name="converterParameter"></param>
        /// <param name="converterLanguage"></param>
        /// <param name="updateSourceTrigger"></param>
        /// <param name="targetNullValue"></param>
        /// <param name="fallbackValue"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public ElementType AltBindMarkdown<TSource>(INotifyPropertyChanged source,
            string sourcePropertyName,
            BindingMode mode = BindingMode.OneWay,
            Func<TSource?, string?>? convert = null,
            Func<string?, TSource?>? convertBack = null,
            object? converterParameter = null,
            string? converterLanguage = null,
            UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
            object? targetNullValue = null,
            object? fallbackValue = null,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1
        )
        {
            textBlock.AltBind(MarkdownProperty, source, sourcePropertyName, mode, convert, convertBack, converterParameter, converterLanguage, updateSourceTrigger, targetNullValue, fallbackValue, filePath, lineNumber);
            return textBlock;
        }
    }

    #endregion


    #region Html Property
    /// <summary>
    /// HTML Attached Dependency Property
    /// </summary>
    public static readonly DependencyProperty HtmlProperty =
        DependencyProperty.RegisterAttached("Html", typeof(string), typeof(TextBlockExtensions), new PropertyMetadata(null, HtmlChanged));

    private static void HtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ElementType textBlock)
            return;

        var text = (string)e.NewValue ?? string.Empty;
        try
        {
            var htmlSpans = new HtmlSpans(text);
            if (textBlock.GetHtmlDependencyObject() is { } html)
            {
                html.HtmlSpans = htmlSpans;
                html.FontFamily = textBlock.FontFamily;
            }
            else
            {
                html = new HtmlDependencyObject(textBlock, htmlSpans);
                textBlock.SetHtmlDependencyObject(html);
            }
        }
        catch (Exception)
        {
            // if anything goes wrong just show the HTML
            Console.WriteLine($"TextBlockExtensions.HtmlChanged Could not convert to Html [{text}]");
            textBlock.Text = HtmlExtensions.ConvertToPlainText(text);
        }
    }


    /// <param name="textBlock"></param>
    extension(ElementType textBlock)
    {
        /// <summary>
        /// HTML Setter
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public ElementType SetHtml(string? value)
        {
            textBlock.SetValue(HtmlProperty, value ?? string.Empty);
            return textBlock;
        }

        /// <summary>
        /// HTML Getter
        /// </summary>
        /// <returns></returns>
        public string GetHtml()
            => (string)textBlock.GetValue(HtmlProperty);

        /// <summary>
        /// HTML Setter Extension Method
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public ElementType Html(string value)
        { textBlock.SetHtml(value); return textBlock; }

        /// <summary>
        /// HTML Work-around Binding 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourcePropertyName"></param>
        /// <param name="mode"></param>
        /// <param name="converter"></param>
        /// <param name="converterParameter"></param>
        /// <param name="converterLanguage"></param>
        /// <param name="updateSourceTrigger"></param>
        /// <param name="targetNullValue"></param>
        /// <param name="fallbackValue"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        public ElementType AltBindHtml(INotifyPropertyChanged source, 
            string sourcePropertyName,
            BindingMode mode = BindingMode.OneWay,
            IValueConverter? converter = null,
            object? converterParameter = null,
            string? converterLanguage = null,
            UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
            object? targetNullValue = null,
            object? fallbackValue = null, 
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1
        )
        {
            textBlock.AltBind(HtmlProperty, source, sourcePropertyName, mode, converter, converterParameter, converterLanguage, updateSourceTrigger, targetNullValue, fallbackValue, filePath, lineNumber);
            return textBlock;
        }

        /// <summary>
        /// HTML Work-around Binding
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceProperty"></param>
        /// <param name="mode"></param>
        /// <param name="converter"></param>
        /// <param name="converterParameter"></param>
        /// <param name="converterLanguage"></param>
        /// <param name="updateSourceTrigger"></param>
        /// <param name="targetNullValue"></param>
        /// <param name="fallbackValue"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        public ElementType AltBindHtml(DependencyObject source, 
            DependencyProperty sourceProperty,
            BindingMode mode = BindingMode.OneWay,
            IValueConverter? converter = null,
            object? converterParameter = null,
            string? converterLanguage = null,
            UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
            object? targetNullValue = null,
            object? fallbackValue = null, 
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1
        )
        {
            textBlock.AltBind(HtmlProperty, source, sourceProperty, mode, converter, converterParameter, converterLanguage, updateSourceTrigger, targetNullValue, fallbackValue, filePath, lineNumber);
            return textBlock;
        }

        /// <summary>
        /// HTML Work-around Binding
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceProperty"></param>
        /// <param name="mode"></param>
        /// <param name="convert"></param>
        /// <param name="convertBack"></param>
        /// <param name="converterParameter"></param>
        /// <param name="converterLanguage"></param>
        /// <param name="updateSourceTrigger"></param>
        /// <param name="targetNullValue"></param>
        /// <param name="fallbackValue"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public ElementType AltBindHtml<TSource>(DependencyObject source, 
            DependencyProperty sourceProperty,
            BindingMode mode = BindingMode.OneWay,
            Func<TSource?, string?>? convert = null,
            Func<string?, TSource?>? convertBack = null,
            object? converterParameter = null,
            string? converterLanguage = null,
            UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
            object? targetNullValue = null,
            object? fallbackValue = null, 
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1
        )
        {
            textBlock.AltBind(HtmlProperty, source, sourceProperty, mode, convert, convertBack, converterParameter, converterLanguage, updateSourceTrigger, targetNullValue, fallbackValue, filePath, lineNumber);
            return textBlock;
        }

        /// <summary>
        /// HTML Work-around Binding
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourcePropertyName"></param>
        /// <param name="mode"></param>
        /// <param name="convert"></param>
        /// <param name="convertBack"></param>
        /// <param name="converterParameter"></param>
        /// <param name="converterLanguage"></param>
        /// <param name="updateSourceTrigger"></param>
        /// <param name="targetNullValue"></param>
        /// <param name="fallbackValue"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public ElementType AltBindHtml<TSource>(INotifyPropertyChanged source,
            string sourcePropertyName,
            BindingMode mode = BindingMode.OneWay,
            Func<TSource?, string?>? convert = null,
            Func<string?, TSource?>? convertBack = null,
            object? converterParameter = null,
            string? converterLanguage = null,
            UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
            object? targetNullValue = null,
            object? fallbackValue = null, 
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1
        )
        {
            textBlock.AltBind(HtmlProperty, source, sourcePropertyName, mode, convert, convertBack, converterParameter, converterLanguage, updateSourceTrigger, targetNullValue, fallbackValue, filePath, lineNumber);
            return textBlock;
        }
    }

    #endregion

        
    #region HtmlDependencyObject Property
    internal static readonly DependencyProperty HtmlDependencyObjectProperty = DependencyProperty.RegisterAttached(
        nameof(HtmlDependencyObject),
        typeof(HtmlDependencyObject),
        typeof(TextBlockExtensions),
        new PropertyMetadata(default(HtmlDependencyObject))
    );
    extension(ElementType element)
    {
        internal HtmlDependencyObject? GetHtmlDependencyObject()
            => (HtmlDependencyObject)element.GetValue(HtmlDependencyObjectProperty);

        internal void SetHtmlDependencyObject(HtmlDependencyObject? value)
            => element.SetValue(HtmlDependencyObjectProperty, value);
    }

    #endregion HtmlDependencyObject Property



    private static bool _applyBackgroundColorTested;
    private static bool _applyBackgroundColorSupported;


    /// <param name="textBlock"></param>
    extension(ElementType textBlock)
    {
        /// <summary>
        /// WARNING!  Calling this will cause a crash IF target version of APP is not set to Windows10 FallCreatorsUpdate (10.0.16299.0) or greater
        /// </summary>
        /// <param name="color"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public ElementType ApplyBackgroundColor(Color color, int startIndex = 0, int length = -1)
        {
            if (!TextHighLighterPresent || ( _applyBackgroundColorTested && !_applyBackgroundColorSupported) )
                return textBlock;

            if (length < 0)
            {
                if (startIndex != 0)
                    return textBlock;
                length = textBlock.Text.Length;
                length += textBlock.Inlines.Count;
            }

            try
            {
                var highlighter = new TextHighlighter
                {
                    Background = new SolidColorBrush(color)
                    //Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(metaFont.TextColor.ToWindowsColor()),
                };
                highlighter.Ranges.Add(new TextRange
                {
                    StartIndex = startIndex,
                    Length = length
                });
                textBlock.TextHighlighters.Add(highlighter);
                _applyBackgroundColorSupported = true;
            }
            catch (Exception)
            {
                _applyBackgroundColorSupported = false;
            }
            finally
            {
                _applyBackgroundColorTested = true;
            }

            return textBlock;
        }

        /// <summary>
        /// Make a copy of a TextBlock
        /// </summary>
        /// <returns></returns>
        public ElementType Copy()
            => new ()
            {
                FontSize = textBlock.FontSize,
                LineStackingStrategy = textBlock.LineStackingStrategy,
                LineHeight = textBlock.LineHeight,
                CharacterSpacing = textBlock.CharacterSpacing,
                IsTextSelectionEnabled = textBlock.IsTextSelectionEnabled,
                FontWeight = textBlock.FontWeight,
                Padding = textBlock.Padding,
                Foreground = textBlock.Foreground,
                FontStyle = textBlock.FontStyle,
                FontStretch = textBlock.FontStretch,
                FontFamily = textBlock.FontFamily,
                TextWrapping = textBlock.TextWrapping,
                TextTrimming = textBlock.TextTrimming,
                TextAlignment = textBlock.TextAlignment,
                Text = textBlock.Text,
                OpticalMarginAlignment = textBlock.OpticalMarginAlignment,
                TextReadingOrder = textBlock.TextReadingOrder,
                TextLineBounds = textBlock.TextLineBounds,
                SelectionHighlightColor = textBlock.SelectionHighlightColor,
                MaxLines = textBlock.MaxLines,
                IsColorFontEnabled = textBlock.IsColorFontEnabled,
                IsTextScaleFactorEnabled = textBlock.IsTextScaleFactorEnabled,
                TextDecorations = textBlock.TextDecorations,
                HorizontalTextAlignment = textBlock.HorizontalTextAlignment,
                FlowDirection = textBlock.FlowDirection,
                DataContext = textBlock.DataContext,
                Name = $"{textBlock.Name}.Copy",
                MinWidth = textBlock.MinWidth,
                MinHeight = textBlock.MinHeight,
                MaxWidth = textBlock.MaxWidth,
                MaxHeight = textBlock.MaxHeight,
                Margin = textBlock.Margin,
                Language = textBlock.Language,
                HorizontalAlignment = textBlock.HorizontalAlignment,
                VerticalAlignment = textBlock.VerticalAlignment,
                Width = textBlock.Width,
                Height = textBlock.Height,
                Style = textBlock.Style,
                RequestedTheme = textBlock.RequestedTheme,
                FocusVisualSecondaryThickness = textBlock.FocusVisualSecondaryThickness,
                FocusVisualSecondaryBrush = textBlock.FocusVisualSecondaryBrush,
                FocusVisualPrimaryThickness = textBlock.FocusVisualPrimaryThickness,
                FocusVisualPrimaryBrush = textBlock.FocusVisualPrimaryBrush,
                FocusVisualMargin = textBlock.FocusVisualMargin,
                AllowFocusWhenDisabled = textBlock.AllowFocusWhenDisabled,
                AllowFocusOnInteraction = textBlock.AllowFocusOnInteraction,
                Clip = textBlock.Clip
            };
    }


    /// <summary>
    /// Convert HorizontalAlignment to TextAlignment
    /// </summary>
    /// <param name="horizontalAlignment"></param>
    /// <returns></returns>
    public static TextAlignment AsTextAlignment(this HorizontalAlignment horizontalAlignment)
    {
        return horizontalAlignment switch
        {
            HorizontalAlignment.Center => TextAlignment.Center,
            HorizontalAlignment.Left => TextAlignment.Left,
            HorizontalAlignment.Right => TextAlignment.Right,
            HorizontalAlignment.Stretch => TextAlignment.Justify,
            _ => TextAlignment.Left
        };
    }

    /// <summary>
    /// Convert TextAlignment to HorizontalAlignment
    /// </summary>
    /// <param name="textAlignment"></param>
    /// <returns></returns>
    public static HorizontalAlignment AsHorizontalAlignment(this TextAlignment textAlignment)
    {
        return textAlignment switch
        {
            TextAlignment.Center => HorizontalAlignment.Center,
            TextAlignment.Left => HorizontalAlignment.Left,
            TextAlignment.Right => HorizontalAlignment.Right,
            TextAlignment.Justify => HorizontalAlignment.Stretch,
            _ => HorizontalAlignment.Left
        };
    }

    /// <summary>
    /// Get opacity based on isEnabled
    /// </summary>
    /// <param name="isEnabled"></param>
    /// <returns></returns>
    public static double EnabledToOpacity(bool isEnabled)
        => isEnabled ? 1.0 : 0.5;

    /// <summary>
    /// Set FontSize to no less than floor value
    /// </summary>
    /// <param name="fontSize"></param>
    /// <param name="floor">default: Platform.MinFontSize</param>
    /// <returns></returns>
    public static double FloorFontSize(double fontSize, double floor = -1.0)
        => Math.Max(fontSize, floor < 0 ? Platform.MinFontSize : floor);

    private static bool _textHighlighterPresentSet;
    /// <summary>
    /// Is the TextHighlighter class available in this version of WinUI?
    /// </summary>
    public static bool TextHighLighterPresent
    {
        get
        {
            if (_textHighlighterPresentSet)
                return field;

            field = Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Microsoft.UI.Xaml.Documents.TextHighlighter");
            _textHighlighterPresentSet = true;
            return field;
        }
    }

    private static bool _textDecorationsPresentSet;
    /// <summary>
    /// Is TextDecorations available in this version of WinUI?
    /// </summary>
    public static bool TextDecorationsPresent
    {
        get
        {
            if (_textDecorationsPresentSet)
                return field;

            try
            {
                field = TestTextDecorations();
            }
            catch (Exception)
            {
                field = false;
            }
            _textDecorationsPresentSet = true;
            return field;
        }
    }

    private static bool TestTextDecorations()
    {
        var run = new Run();
        try
        {
            run.TextDecorations = Windows.UI.Text.TextDecorations.None;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }


}
