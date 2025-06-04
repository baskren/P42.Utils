using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Data;
using P42.Serilog.QuickLog;

namespace P42.Utils.Uno;

[Bindable]
//[System.ComponentModel.Bindable(System.ComponentModel.BindableSupport.Yes)]
internal partial class HtmlDependencyObject : DependencyObject
{
    /*
    #region TextWrapping Property
    public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register(
        nameof(TextWrapping),
        typeof(TextWrapping),
        typeof(HtmlDependencyObject),
        new PropertyMetadata(default(TextWrapping), OnFontPropertyChanged)
    );

    public TextWrapping TextWrapping
    {
        get => (TextWrapping)GetValue(TextWrappingProperty);
        set => SetValue(TextWrappingProperty, value);
    }
    #endregion TextWrapping Property

    #region TextTrimming Property
    public static readonly DependencyProperty TextTrimmingProperty = DependencyProperty.Register(
        nameof(TextTrimming),
        typeof(TextTrimming),
        typeof(HtmlDependencyObject),
        new PropertyMetadata(default(TextTrimming), OnFontPropertyChanged)
    );
    public TextTrimming TextTrimming
    {
        get => (TextTrimming)GetValue(TextTrimmingProperty);
        set => SetValue(TextTrimmingProperty, value);
    }
    #endregion TextTrimming Property

    #region TextAlignment Property
    public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(
        nameof(TextAlignment),
        typeof(TextAlignment),
        typeof(HtmlDependencyObject),
        new PropertyMetadata(default(TextAlignment), OnFontPropertyChanged)
    );
    public TextAlignment TextAlignment
    {
        get => (TextAlignment)GetValue(TextAlignmentProperty);
        set => SetValue(TextAlignmentProperty, value);
    }
    #endregion TextAlignment Property

    #region Padding Property
    public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
        nameof(Padding),
        typeof(Thickness),
        typeof(HtmlDependencyObject),
        new PropertyMetadata(default(Thickness), OnFontPropertyChanged)
    );
    public Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }
    #endregion Padding Property
    */

    #region Foreground Property
    /// <summary>
    /// Foreground Dependency Property
    /// </summary>
    public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
        nameof(Foreground),
        typeof(Brush),
        typeof(HtmlDependencyObject),
        new PropertyMetadata(null, OnFontPropertyChanged)
    );
    /// <summary>
    /// Foreground
    /// </summary>
    public Brush? Foreground
    {
        get => (Brush)GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }
    #endregion Foreground Property

    #region FontWeight Property
    /// <summary>
    /// FontWeight Dependency Property
    /// </summary>
    public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register(
        nameof(FontWeight),
        typeof(FontWeight),
        typeof(HtmlDependencyObject),
        new PropertyMetadata(default(FontWeight), OnFontPropertyChanged)
    );
    /// <summary>
    /// FontWeight
    /// </summary>
    public FontWeight FontWeight
    {
        get => (FontWeight)GetValue(FontWeightProperty);
        set => SetValue(FontWeightProperty, value);
    }
    #endregion FontWeight Property

    #region FontStyle Property
    /// <summary>
    /// FontStyle Dependency Property
    /// </summary>
    public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register(
        nameof(FontStyle),
        typeof(FontStyle),
        typeof(HtmlDependencyObject),
        new PropertyMetadata(default(FontStyle), OnFontPropertyChanged)
    );
    /// <summary>
    /// FontStyle
    /// </summary>
    public FontStyle FontStyle
    {
        get => (FontStyle)GetValue(FontStyleProperty);
        set => SetValue(FontStyleProperty, value);
    }
    #endregion FontStyle Property

    /*
    #region FontStretch Property
    public static readonly DependencyProperty FontStretchProperty = DependencyProperty.Register(
        nameof(FontStretch),
        typeof(FontStretch),
        typeof(HtmlDependencyObject),
        new PropertyMetadata(default(FontStretch), OnFontPropertyChanged)
    );
    public FontStretch FontStretch
    {
        get => (FontStretch)GetValue(FontStretchProperty);
        set => SetValue(FontStretchProperty, value);
    }
    #endregion FontStretch Property
    */

    #region FontSize Property
    /// <summary>
    /// FontSize Dependency Property
    /// </summary>
    public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
        nameof(FontSize),
        typeof(double),
        typeof(HtmlDependencyObject),
        new PropertyMetadata(0.0, OnFontPropertyChanged)
    );
    /// <summary>
    /// FontSize
    /// </summary>
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }
    #endregion FontSize Property

    #region FontFamily Property
    /// <summary>
    /// FontFamily Dependency Property
    /// </summary>
    public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register(
        nameof(FontFamily),
        typeof(Microsoft.UI.Xaml.Media.FontFamily),
        typeof(HtmlDependencyObject),
        new PropertyMetadata(null, OnFontPropertyChanged)
    );
    /// <summary>
    /// FontFamily
    /// </summary>
    public Microsoft.UI.Xaml.Media.FontFamily? FontFamily
    {
        get => (Microsoft.UI.Xaml.Media.FontFamily)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }
    #endregion FontFamily Property

    /*
    #region LineStackingStrategy Property
    public static readonly DependencyProperty LineStackingStrategyProperty = DependencyProperty.Register(
        nameof(LineStackingStrategy),
        typeof(LineStackingStrategy),
        typeof(HtmlDependencyObject),
        new PropertyMetadata(default(LineStackingStrategy), OnFontPropertyChanged)
    );
    public LineStackingStrategy LineStackingStrategy
    {
        get => (LineStackingStrategy)GetValue(LineStackingStrategyProperty);
        set => SetValue(LineStackingStrategyProperty, value);
    }
    #endregion LineStackingStrategy Property

    #region LineHeight Property
    public static readonly DependencyProperty LineHeightProperty = DependencyProperty.Register(
        nameof(LineHeight),
        typeof(double),
        typeof(HtmlDependencyObject),
        new PropertyMetadata(default(double), OnFontPropertyChanged)
    );
    public double LineHeight
    {
        get => (double)GetValue(LineHeightProperty);
        set => SetValue(LineHeightProperty, value);
    }
    #endregion LineHeight Property

    #region CharacterSpacing Property
    public static readonly DependencyProperty CharacterSpacingProperty = DependencyProperty.Register(
        nameof(CharacterSpacing),
        typeof(int),
        typeof(HtmlDependencyObject),
        new PropertyMetadata(default(int), OnFontPropertyChanged)
    );
    public int CharacterSpacing
    {
        get => (int)GetValue(CharacterSpacingProperty);
        set => SetValue(CharacterSpacingProperty, value);
    }
    #endregion CharacterSpacing Property
    */

    #region HtmlSpans Property
    /// <summary>
    /// HtmlSpans Dependency Property
    /// </summary>
    public static readonly DependencyProperty HtmlSpansProperty = DependencyProperty.Register(
        nameof(HtmlSpans),
        typeof(HtmlSpans),
        typeof(HtmlDependencyObject),
        new PropertyMetadata(null,OnFontPropertyChanged)
    );
    /// <summary>
    /// HtmlSpans (where the magic happens)
    /// </summary>
    public HtmlSpans? HtmlSpans
    {
        get => (HtmlSpans)GetValue(HtmlSpansProperty);
        set => SetValue(HtmlSpansProperty, value);
    }
    #endregion HtmlSpans Property


    private static void OnFontPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is HtmlDependencyObject html)
            SetAndFormatText(html._targetTextBlock, html.HtmlSpans);
    }


    private readonly TextBlock _targetTextBlock;
        

    internal HtmlDependencyObject(TextBlock textBlock, HtmlSpans? spans)
    {
        _targetTextBlock = textBlock;
        FontFamily = textBlock.FontFamily;
        HtmlSpans = spans;
        BindFont(textBlock);
    }

    #region Binding

    private static List<string>? GetExcepts(object? except)
        => except switch
        {
            null => null,
            string str => [str],
            IEnumerable<string> enumerable => [..enumerable],
            _ => throw new Exception("BindFont except: argument must be null, string, or IEnumerable<string>")
        };
    
    /*
    private void BindFont(Control source, BindingMode bindingMode = BindingMode.OneWay, object? except = null)
    {
        var excepts = GetExcepts(except);
        if (excepts is null || !excepts.Contains(nameof(Control.FontFamily)))
            this.WBind(FontFamilyProperty, source, Control.FontFamilyProperty, bindingMode);
        if (excepts is null || !excepts.Contains(nameof(Control.FontSize)))
            this.WBind(FontSizeProperty, source, Control.FontSizeProperty, bindingMode);
        //if (excepts is null || !excepts.Contains(nameof(Control.FontStretch)))
        //    this.Bind(HtmlDependencyObject.FontStretchProperty, source, nameof(Control.FontStretch), bindingMode);
        if (excepts is null || !excepts.Contains(nameof(Control.FontStyle)))
            this.WBind(FontStyleProperty, source, Control.FontStyleProperty, bindingMode);
        if (excepts is null || !excepts.Contains(nameof(Control.FontWeight)))
            this.WBind(FontWeightProperty, source, Control.FontWeightProperty, bindingMode);
        if (excepts is null || !excepts.Contains(nameof(Control.Foreground)))
            this.WBind(ForegroundProperty, source, Control.ForegroundProperty, bindingMode);
    }
    */

    private void BindFont(TextBlock source, BindingMode bindingMode = BindingMode.OneWay, object? except = null)
    {
        var excepts = GetExcepts(except);
        if (excepts is null || !excepts.Contains(nameof(TextBlock.FontFamily)))
            this.WBind(FontFamilyProperty, source, TextBlock.FontFamilyProperty, bindingMode);
        if (excepts is null || !excepts.Contains(nameof(TextBlock.FontSize)))
            this.WBind(FontSizeProperty, source, TextBlock.FontSizeProperty, bindingMode);
        //if (excepts is null || !excepts.Contains(nameof(TextBlock.FontStretch)))
        //    this.Bind(HtmlDependencyObject.FontStretchProperty, source, nameof(TextBlock.FontStretch), bindingMode);
        if (excepts is null || !excepts.Contains(nameof(TextBlock.FontStyle)))
            this.WBind(FontStyleProperty, source, TextBlock.FontStyleProperty, bindingMode);
        if (excepts is null || !excepts.Contains(nameof(TextBlock.FontWeight)))
            this.WBind(FontWeightProperty, source, TextBlock.FontWeightProperty, bindingMode);
        if (excepts is null || !excepts.Contains(nameof(TextBlock.Foreground)))
            this.WBind(ForegroundProperty, source, TextBlock.ForegroundProperty, bindingMode);
    }

    /*
    private void BindFont(ContentPresenter source, BindingMode bindingMode = BindingMode.OneWay, object? except = null)
    {
        var excepts = GetExcepts(except);
        if (excepts is null || !excepts.Contains(nameof(ContentPresenter.FontFamily)))
            this.WBind(FontFamilyProperty, source, ContentPresenter.FontFamilyProperty, bindingMode);
        if (excepts is null || !excepts.Contains(nameof(ContentPresenter.FontSize)))
            this.WBind(FontSizeProperty, source, ContentPresenter.FontSizeProperty, bindingMode);
        //if (excepts is null || !excepts.Contains(nameof(ContentPresenter.FontStretch)))
        //    this.Bind(HtmlDependencyObject.FontStretchProperty, source, nameof(ContentPresenter.FontStretch), bindingMode);
        if (excepts is null || !excepts.Contains(nameof(ContentPresenter.FontStyle)))
            this.WBind(FontStyleProperty, source, ContentPresenter.FontStyleProperty, bindingMode);
        if (excepts is null || !excepts.Contains(nameof(ContentPresenter.FontWeight)))
            this.WBind(FontWeightProperty, source, ContentPresenter.FontWeightProperty, bindingMode);
        if (excepts is null || !excepts.Contains(nameof(ContentPresenter.Foreground)))
            this.WBind(ForegroundProperty, source, ContentPresenter.ForegroundProperty, bindingMode);
    }
    */

    #endregion


    internal static void SetAndFormatText(TextBlock textBlock, HtmlSpans? newSpansGiven, double altFontSize = -1)
    {
        textBlock.FontSize = altFontSize > 0
            ? altFontSize
            : textBlock.FontSize > 0
                ? textBlock.FontSize
                : textBlock.DefaultFontSize();

        System.Diagnostics.Debug.WriteLine($"SetAndFormatText : [{textBlock.Text}] [{newSpansGiven?.UnmarkedText}] [{newSpansGiven?.Count ?? 0}] Spans");
        
        string newText;
        HtmlSpans oldSpans;
        var newSpans = newSpansGiven ?? [];
        try
        {
            newText = newSpans.UnmarkedText;
            var oldHtmlDependencyObject = textBlock.GetHtmlDependencyObject();
            oldSpans = oldHtmlDependencyObject?.HtmlSpans ?? [];
        }
        catch (Exception e)
        {
            QLog.Error(e);
            return;
        }
        
        if (textBlock.Text == newText && oldSpans.Count == newSpans.Count )
        {
            if (oldSpans.Count == 0)
                return;
            
            if (!oldSpans.Where((t, i) => !t.Equals(newSpans[i])).Any())
                return;
        }

        textBlock.Inlines?.Clear();
        textBlock.Text = string.Empty;
        if (string.IsNullOrWhiteSpace(newText) || newText == newSpans.Text)
        {
            // there isn't any markup!
            textBlock.Text = newText;
            return;
        }

        //textBlock.LineHeight = FontExtensions.LineHeightForFontSize(textBlock.FontSize);
        //textBlock.LineStackingStrategy = Microsoft.UI.Xaml.LineStackingStrategy.BaselineToBaseline;


        /*
        Color? tColor;
        if (textBlock.Foreground is SolidColorBrush tBrush)
        {
            tColor = tBrush.Color;
        }

        /*
        System.Diagnostics.Debug.WriteLine("TextBlockExtensions.SetAndFormatText: textColor: " + (textBlock.Foreground is null ? "null" : tColor.ToHexArgbColorString()));
        if (tColor == default)
        {
            System.Diagnostics.Debug.WriteLine("TextBlockExtensions.SetAndFormatText fallbackColor: " + SystemColors.WindowTextColor.ToHexArgbColorString());
        }
        */

        #region Layout font-spans (MetaFonts)
        var metaFonts = new List<MetaFont>();
        var baseMetaFont = new MetaFont
            (
                textBlock.FontFamily,
                textBlock.FontSize, 
                (short)Microsoft.UI.Text.FontWeights.Normal.Weight,
                false,
                textColor: textBlock.Foreground is SolidColorBrush brush
                    ? brush.Color
                    : ColorExtensions.AppColor("SystemColorWindowTextColor") //(Color)Application.Current.Resources["SystemBaseHighColor"]
            );

        var mathMetaFont = new MetaFont
            (
                Platform.MathFontFamily, 
                baseMetaFont.Size,
                (short)Microsoft.UI.Text.FontWeights.Normal.Weight,
                baseMetaFont.Italic, 
                "",
                "",
                baseMetaFont.TextColor, 
                baseMetaFont.BackgroundColor, 
                baseMetaFont.Underline,
                baseMetaFont.Strikethrough
            );

        for (var i = 0; i < newText.Length; i++)
        {
            // Are we at the start of a Unicode surrogate character?
            if (i + 1 < newText.Length && newText[i]>= '\ud800' && newText[i]<='\udbff' && newText[i + 1] >= '\udc00' && newText[i + 1] <= '\udeff')
            {
                var font = newText[i] == '\ud835'
                    ? mathMetaFont
                    : baseMetaFont;

                metaFonts.Add(new MetaFont(font));
                metaFonts.Add(new MetaFont(font));  // there are two because we're using a double byte Unicode character
                i++;
            }
            else
                metaFonts.Add(new MetaFont(baseMetaFont));
        }
        #endregion


        #region Apply non-font Spans
        foreach (var span in newSpans)
        {
            var spanStart = span.Start;
            var spanEnd = span.End;

            //spanEnd++;
            if (spanEnd >= newText.Length)
                spanEnd = newText.Length - 1;

            for (var i = spanStart; i <= spanEnd; i++)
            {
                switch (span.Key)
                {
                    case FontFamilySpan.SpanKey: // TextElement.FontFamily
                        var fontFamily = ((FontFamilySpan)span).FontFamily;
                        metaFonts[i].Family = fontFamily;
                        break;
                    case FontSizeSpan.SpanKey:  // TextElement.FontSize
                        var size = ((FontSizeSpan)span).Size;
                        metaFonts[i].Size = size < 0 ? metaFonts[i].Size * -size : size;
                        break;
                    case FontWeightSpan.SpanKey: // Bold span // TextElement.FontWeight (Thin, ExtraLight, Light, SemiLight, Normal, Medium, SemiBold, Bold, ExtraBold, Black, ExtraBlack)
                        if (span is  FontWeightSpan fwp)
                        {
                            metaFonts[i].FontWeight = (short) (fwp.IsRelativeToParent 
                                ? metaFonts[i].FontWeight + fwp.Weight
                                : fwp.Weight);
                        }
                        break;
                    case ItalicsSpan.SpanKey: // Italic span // TextElement.FontStyle (Normal, Italic, Oblique)
                        metaFonts[i].Italic = true;
                        break;
                    case FontColorSpan.SpanKey: // TextElement.Foreground
                        metaFonts[i].TextColor = ((FontColorSpan)span).Color;
                        break;
                    case UnderlineSpan.SpanKey: // Underline span  // TextElement.TextDecorations = None, Strikethrough, Underline
                        metaFonts[i].Underline = true;
                        break;
                    case StrikethroughSpan.SpanKey: // TextElement.TextDecorations = None, Strikethrough, Underline
                        metaFonts[i].Strikethrough = true;
                        break;
                    case SuperscriptSpan.SpanKey: // Run with Typographic.Variants=FontVariants.Superscript while using Cambria
                        metaFonts[i].Baseline = FontBaseline.Superscript;
                        break;
                    case SubscriptSpan.SpanKey: // Run with Typographic.Variants=FontVariants.Subscript while using Cambria
                        metaFonts[i].Baseline = FontBaseline.Subscript;
                        break;
                    case NumeratorSpan.SpanKey: // no UWP solution - need to use SuperScript
                        metaFonts[i].Baseline = FontBaseline.Numerator;
                        break;
                    case DenominatorSpan.SpanKey: // no UWP solution - need to use Subscript
                        metaFonts[i].Baseline = FontBaseline.Denominator;
                        break;
                    case HyperlinkSpan.SpanKey:  // Hyperlink span ??
                        //metaFonts[i].TextColor = Colors.Blue;
                        metaFonts[i].Action = new MetaFontAction((HyperlinkSpan)span);
                        break;
                    case BackgroundColorSpan.SpanKey: // if Win10 fall creator's update, there is a solution: create TextHighlighter, set its BackgroundColor and add the Range (Start/End) to its Ranges, and add to TextBlock.Highlighters
                        metaFonts[i].BackgroundColor = ((BackgroundColorSpan)span).Color;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Unknown span type: {span.Key}");
                }
            }
        }
        #endregion


        #region Convert MetaFonts to InLines
        // run through MetaFonts to see if we need to set new Font attributes

        var lastMetaFont = baseMetaFont;
        var startIndex = 0;
        var lastFontWeight = lastMetaFont.FontWeight;
        for (var i = 0; i < metaFonts.Count; i++)
        {
            var metaFont = metaFonts[i];
            if (lastMetaFont == metaFont)
                continue;

            // we are at the start of a new span
            if (i > 0) //&& lastMetaFont != baseMetaFont)
                AddInline(textBlock, lastMetaFont, newText, startIndex, i - startIndex);
            lastMetaFont = metaFont;
            lastFontWeight = lastMetaFont.FontWeight;
            startIndex = i;
        }
        AddInline(textBlock, lastMetaFont, newText, startIndex, newText.Length - startIndex);

        //AddInline(textBlock, lastMetaFont, lastMetaFont.Family.Source, startIndex+newText.Length, lastMetaFont.Family.Source.Length);
        #endregion

    }

    private enum Decoration
    {
        Strikethrough,
        Underline
    }

    private static void ApplyTextDecorations(Run run, Decoration decoration)
    {
        run.TextDecorations |= decoration switch
        {
            Decoration.Strikethrough => TextDecorations.Strikethrough,
            Decoration.Underline => TextDecorations.Underline,
            _ => throw new ArgumentException($"Unsupported TextDecoration: {decoration}")
        };
    }

    private static void AddInline(TextBlock textBlock, MetaFont metaFont, string text, int startIndex, int length)
    {
        if (metaFont.IsActiveAction)
        {
            var link = new Hyperlink();

            if (metaFont.Action?.Href.Trim().StartsWith("http") ?? false)
                link.NavigateUri = new Uri(metaFont.Action.Href);
            else if (textBlock.GetLinkTappedHandler() is { } action)
                link.Click += (_, _) => action.Invoke(metaFont.Action?.Id ?? string.Empty, metaFont.Action?.Href ?? string.Empty);

            var linkRun = new Run
            {
                Text = text.Substring(startIndex, length),
                FontSize = metaFont.Size,
                FontWeight = new FontWeight((ushort)metaFont.FontWeight),
                FontStyle = metaFont.Italic ? FontStyle.Italic : FontStyle.Normal
                // Foreground = new SolidColorBrush(Colors.Blue),
            };

            link.Inlines.Add(linkRun);
            textBlock.Inlines.Add(link);
            return;
        }

        var run = new Run
        {
            Text = text.Substring(startIndex, length),
            FontSize = metaFont.Size,
            FontWeight = new FontWeight((ushort)metaFont.FontWeight),
            FontStyle = metaFont.Italic ? FontStyle.Italic : FontStyle.Normal,
            Foreground = new SolidColorBrush(metaFont.TextColor)
        };


        if (TextBlockExtensions.TextDecorationsPresent && metaFont.Strikethrough)
            ApplyTextDecorations(run, Decoration.Strikethrough);

        switch (metaFont.Baseline)
        {
            case FontBaseline.Normal:
                if (metaFont.Family != null)
                    run.FontFamily = metaFont.Family;
                break;
#if WINDOWS

            // FontVariants don't work in UNO

            case FontBaseline.Numerator:
            case FontBaseline.Superscript:
                run.FontFamily = Platform.VariantsFontFamily;
                Typography.SetVariants(run, FontVariants.Superscript);
                break;
            case FontBaseline.Denominator:
            case FontBaseline.Subscript:
                run.FontFamily = Platform.VariantsFontFamily;
                Typography.SetVariants(run, FontVariants.Subscript);
                break;
            default:
                throw new ArgumentException($"Unsupported FontBaseline: {metaFont.Baseline}");
#endif
        }

        if (!metaFont.BackgroundColor.IsDefaultOrTransparent())
            textBlock.ApplyBackgroundColor(metaFont.BackgroundColor, startIndex, length);

        run.Foreground = metaFont.TextColor != default 
            ? new SolidColorBrush(metaFont.TextColor) 
            : new SolidColorBrush((Color)Application.Current.Resources["SystemBaseHighColor"]);
        
        if (TextBlockExtensions.TextDecorationsPresent && metaFont.Underline)
            ApplyTextDecorations(run, Decoration.Underline);
        textBlock.Inlines.Add(run);
            

    }



}
