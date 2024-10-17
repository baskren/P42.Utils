using System;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using ElementType = Microsoft.UI.Xaml.Controls.TextBlock;
using System.Runtime.CompilerServices;

namespace P42.Utils.Uno;

[Bindable]
//[System.ComponentModel.Bindable(System.ComponentModel.BindableSupport.Yes)]
public static class TextBlockExtensions
{
    /// <summary>
    /// Minimum Font Size used in Html Strings
    /// </summary>
    public static double MinFontSize { get; set; } = 10.0;

    #region Link Tapped Property
    /// <summary>
    /// DependencyProperty for TextBlock LinkTapped Event Handler extension property
    /// </summary>
    public static readonly DependencyProperty LinkTappedProperty =
        DependencyProperty.RegisterAttached("LinkTapped", typeof(Action<string?,string?>), typeof(TextBlockExtensions), new PropertyMetadata(null));

    /// <summary>
    /// Code for setting LinkTapped Event Handler
    /// </summary>
    /// <param name="textBlock"></param>
    /// <param name="handler"></param>
    /// <returns>textBlock</returns>
    public static ElementType SetLinkTappedHandler(this ElementType textBlock, Action<string?, string?>? handler)
    {
        textBlock.SetValue(LinkTappedProperty, handler);
        return textBlock;
    }

    /// <summary>
    /// Code for getting LinkTapped Event Handler
    /// </summary>
    /// <param name="textBlock"></param>
    /// <returns>handler</returns>
    public static Action<string?, string?>? GetLinkTappedHandler(this ElementType textBlock)
        => (Action<string?,string?>)textBlock.GetValue(LinkTappedProperty);
    #endregion


    #region Html Property
    /// <summary>
    /// DependencyProperty for TextBlock Html extension property 
    /// </summary>
    public static readonly DependencyProperty HtmlProperty =
        DependencyProperty.RegisterAttached("Html", typeof(string), typeof(TextBlockExtensions), new PropertyMetadata(null, HtmlChanged));

    /// <summary>
    /// Code for setting TextBlock HTML
    /// </summary>
    /// <param name="textBlock"></param>
    /// <param name="value"></param>
    /// <returns>textBlock</returns>
    public static ElementType SetHtml(this ElementType textBlock, string? value)
    {
        textBlock.SetValue(HtmlProperty, value ?? string.Empty);
        return textBlock;
    }

    /// <summary>
    /// Code for getting TextBlock HTML
    /// </summary>
    /// <param name="textBlock"></param>
    /// <returns>HTML</returns>
    public static string GetHtml(this ElementType textBlock)
        => (string)textBlock.GetValue(HtmlProperty);


    /// <summary>
    /// Code for setting TextBlock HTML
    /// </summary>
    /// <param name="element"></param>
    /// <param name="value"></param>
    /// <returns>textBlock</returns>
    public static ElementType Html(this ElementType element, string value)
    { element.SetHtml(value); return element; }

    /// <summary>
    /// Code for Binding string to TextBlock.Html extension property
    /// </summary>
    /// <param name="element"></param>
    /// <param name="source"></param>
    /// <param name="path"></param>
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
    public static ElementType BindHtml(this ElementType element, object source, string path,
        BindingMode mode = BindingMode.OneWay,
        IValueConverter? converter = null,
        object? converterParameter = null,
        string converterLanguage = "",
        UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
        object? targetNullValue = null,
        object? fallbackValue = null, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1
    )
    {
        element.Bind(HtmlProperty, source, path, mode, converter, converterParameter, converterLanguage, updateSourceTrigger, targetNullValue, fallbackValue, filePath, lineNumber);
        return element;
    }
    #endregion

        
    #region HtmlDependencyObject Property
    internal static readonly DependencyProperty HtmlDependencyObjectProperty = DependencyProperty.RegisterAttached(
        nameof(HtmlDependencyObject),
        typeof(HtmlDependencyObject),
        typeof(TextBlockExtensions),
        new PropertyMetadata(default(HtmlDependencyObject))
    );
    internal static HtmlDependencyObject GetHtmlDependencyObject(this ElementType element)
        => (HtmlDependencyObject)element.GetValue(HtmlDependencyObjectProperty);
    internal static void SetHtmlDependencyObject(this ElementType element, HtmlDependencyObject value)
        => element.SetValue(HtmlDependencyObjectProperty, value);
    #endregion HtmlDependencyObject Property


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
            // if anything goes wrong just show the html
            Console.WriteLine($"TextBlockExtensions.HtmlChanged Could not convert to Html [{text}]");
            textBlock.Text = Windows.Data.Html.HtmlUtilities.ConvertToText(text);
        }
    }




    /// <summary>
    /// WARNING!  Calling this will cause a crash IF target version of APP is not set to Windows10 FallCreatorsUpdate (10.0.16299.0) or greater
    /// </summary>
    /// <param name="textBlock"></param>
    /// <param name="color"></param>
    /// <param name="startIndex"></param>
    /// <param name="length"></param>
    public static ElementType ApplyBackgroundColor(this ElementType textBlock, Color color, int startIndex = 0, int length = -1)
    {
        if (!TextHighLighterPresent)
            return textBlock;

        if (length < 0)
        {
            if (startIndex != 0)
                return textBlock;
            length = textBlock.Text.Length;
            length += textBlock.Inlines.Count;
        }

        var highlighter = new TextHighlighter
        {
            //Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(metaFont.TextColor.ToWindowsColor()),
            Background = new SolidColorBrush(color)
        };
        highlighter.Ranges.Add(new TextRange
        {
            StartIndex = startIndex,
            Length = length
        });
        textBlock.TextHighlighters.Add(highlighter);
        return textBlock;
    }


    /// <summary>
    /// Get the default fond size for the TextBlock
    /// </summary>
    /// <param name="_"></param>
    /// <returns>default TextBlock.FontSize</returns>
    public static double DefaultFontSize(this ElementType _)
        => (double)Application.Current.Resources["ControlContentThemeFontSize"];

    /// <summary>
    /// Create a copy of a TextBlock
    /// </summary>
    /// <param name="textBlock"></param>
    /// <returns>new TextBlock</returns>
    public static ElementType Copy(this ElementType textBlock)
    {
        var result = new ElementType
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
            Name = textBlock.Name + ".Copy",
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
        return result;
    }

    /// <summary>
    /// Converts a HorizontalAlignment value to a TextAlignment value
    /// </summary>
    /// <param name="horizontalAlignment"></param>
    /// <returns>TextAlignment</returns>
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
    /// Converts a TextAlignment value to a HorizontalAlignment value
    /// </summary>
    /// <param name="textAlignment"></param>
    /// <returns>HorizontalAlignment</returns>
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
    /// Returns opacity value based upon isEnabled value
    /// </summary>
    /// <param name="isEnabled"></param>
    /// <returns>opacity value</returns>
    public static double EnabledToOpacity(bool isEnabled)
        => isEnabled ? 1.0 : 0.5;

    /// <summary>
    /// Returns font size, no lower than MinFontSize
    /// </summary>
    /// <param name="fontSize"></param>
    /// <returns>FontSize value</returns>
    public static double FloorFontSize(double fontSize)
        => Math.Max(fontSize, MinFontSize);

    private static bool _textHighlighterPresent;
    private static bool _textHighlighterPresentSet;
    
    /// <summary>
    /// Tests if Microsoft.UI.Xaml.Documents.TextHighlighter type is in this version of WinAppSDK
    /// </summary>
    public static bool TextHighLighterPresent
    {
        get
        {
            if (_textHighlighterPresentSet)
                return _textHighlighterPresent;

            _textHighlighterPresent = Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Microsoft.UI.Xaml.Documents.TextHighlighter");
            _textHighlighterPresentSet = true;
            return _textHighlighterPresent;
        }
    }

    private static bool _textDecorationsPresent;
    private static bool _textDecorationsPresentSet;

    /// <summary>
    /// Tests if Windows.UI.Text.TextDecorations.None is in this version of WinAppSDK
    /// </summary>
    public static bool TextDecorationsPresent
    {
        get
        {
            if (_textDecorationsPresentSet)
                return _textDecorationsPresent;

            //_textDecorationsPresent = Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.Text.TextDecorations");
            //var run = new Run();
            try
            {
                _textDecorationsPresent = TestTextDecorations();
            }
            catch (Exception)
            {
                _textDecorationsPresent = false;
            }
            _textDecorationsPresentSet = true;
            return _textDecorationsPresent;
        }
    }

    private static bool TestTextDecorations()
    {
        try
        {
            _ = new Run
            {
                TextDecorations = Windows.UI.Text.TextDecorations.None
            };
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }


}
