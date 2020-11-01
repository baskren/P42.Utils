using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using ElementType = Windows.UI.Xaml.Controls.TextBlock;

namespace P42.Utils.Uno
{
    [System.ComponentModel.Bindable(System.ComponentModel.BindableSupport.Yes)]
    public static class TextBlockExtensions
    {
        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.RegisterAttached("Html", typeof(string), typeof(TextBlockExtensions), new PropertyMetadata(null, HtmlChanged));

        public static void SetHtml(this TextBlock textBlock, string value)
            => textBlock.SetValue(HtmlProperty, value ?? string.Empty);
        

        public static string GetHtml(this TextBlock textBlock)
            => (string)textBlock.GetValue(HtmlProperty);
        

        private static void HtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock textBlock)
            {
                var text = (string)e.NewValue ?? string.Empty;
                try
                {
                    var htmlSpans = new HtmlSpans(text);
                    SetAndFormatText(textBlock, htmlSpans);
                }
                catch (Exception)
                {
                    // if anything goes wrong just show the html
                    textBlock.Text = Windows.Data.Html.HtmlUtilities.ConvertToText(text);
                }
            }
        }

        static bool _textDecorationsPresent;
        static bool _textDecorationsPresentSet;
        public static bool TextDecorationsPresent
        {
            get
            {
                if (!_textDecorationsPresentSet)
                {
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
                }
                return _textDecorationsPresent;
            }
        }

        static bool TestTextDecorations()
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

        public static double DefaultFontSize(this TextBlock textBlock)
            => (double)Application.Current.Resources["ControlContentThemeFontSize"];

        static readonly FontFamily MathMetaFontFamily = FontExtensions.GetFontFamily("STIXGeneral");
        static readonly FontFamily ScriptFontFamily = new FontFamily("Segoe UI#Regular");

        internal static void SetAndFormatText(this TextBlock textBlock, HtmlSpans htmlSpans, double altFontSize = -1)
        {
            if (textBlock == null)
                return;

            textBlock.Text = string.Empty;
            textBlock.Inlines.Clear();
            textBlock.FontSize = altFontSize > 0 
                ? altFontSize 
                : textBlock.FontSize > 0
                    ? textBlock.FontSize
                    : textBlock.DefaultFontSize();
            //textBlock.LineHeight = FontExtensions.LineHeightForFontSize(textBlock.FontSize);
            textBlock.LineStackingStrategy = Windows.UI.Xaml.LineStackingStrategy.BaselineToBaseline;

            var text = htmlSpans?.UnmarkedText;

            if (string.IsNullOrWhiteSpace(text) || text == htmlSpans?.Text)
            {
                // there isn't any markup!
                textBlock.Text = text ?? "";
                return;
            }

            #region Layout font-spans (MetaFonts)
            var metaFonts = new List<MetaFont>();
            var baseMetaFont = new MetaFont(
                textBlock.FontFamily,
                textBlock.FontSize, //(altFontSize > 0 ? altFontSize : label.DecipheredFontSize()), //(label.FontSize < 0 ? (double)Windows.UI.Xaml.Application.Current.Resources["ControlContentThemeFontSize"] : label.FontSize)), //label.FontSize,
                textBlock.FontWeight.Weight >= Windows.UI.Text.FontWeights.Bold.Weight,
                (textBlock.FontStyle & Windows.UI.Text.FontStyle.Italic) > 0,
                textColor: textBlock.Foreground is SolidColorBrush brush 
                    ? brush.Color 
                    : (Color)Application.Current.Resources["SystemBaseHighColor"]
                );

            var MathMetaFont = new MetaFont(baseMetaFont)
            {
                Family = MathMetaFontFamily//FontService.ReconcileFontFamily("Forms9Patch.Resources.Fonts.STIXGeneral.otf", P42.Utils.ReflectionExtensions.GetAssembly(typeof(Forms9Patch.Label)))
            };

            for (int i = 0; i < text.Length; i++)
            {
                if (i + 1 < text.Length && text[i] == '\ud835' && text[i + 1] >= '\udc00' && text[i + 1] <= '\udeff')
                {
                    metaFonts.Add(new MetaFont(MathMetaFont));
                    metaFonts.Add(new MetaFont(MathMetaFont));  // there are two because we're using a double byte unicode character
                    i++;
                }
                else
                    metaFonts.Add(new MetaFont(baseMetaFont));
            }
            #endregion


            #region Apply non-font Spans
            foreach (var span in htmlSpans)
            {
                var spanStart = span.Start;
                var spanEnd = span.End;

                //spanEnd++;
                if (spanEnd >= text.Length)
                    spanEnd = text.Length - 1;

                for (int i = spanStart; i <= spanEnd; i++)
                {
                    switch (span.Key)
                    {
                        case FontFamilySpan.SpanKey: // TextElement.FontFamily
                            var fontFamily = ((FontFamilySpan)span).FontFamily;
                            metaFonts[i].Family = fontFamily;
                            break;
                        case FontSizeSpan.SpanKey:  // TextElement.FontSize
                            var size = ((FontSizeSpan)span).Size;
                            metaFonts[i].Size = (size < 0 ? metaFonts[i].Size * (-size) : size);
                            break;
                        case BoldSpan.SpanKey: // Bold span // TextElement.FontWeight (Thin, ExtraLight, Light, SemiLight, Normal, Medium, SemiBold, Bold, ExtraBold, Black, ExtraBlack)
                            metaFonts[i].Bold = true;
                            break;
                        case ItalicsSpan.SpanKey: // Italic span // TextElement.FontStyle (Normal, Italic, Oblique)
                            metaFonts[i].Italic = true;
                            break;
                        case FontColorSpan.SpanKey: // TextElement.Foreground
                            metaFonts[i].TextColor = ((FontColorSpan)span).Color;
                            break;
                        case UnderlineSpan.SpanKey: // Underline span  // TextElement.TextDecorations = None, Strikethought, Underline
                            metaFonts[i].Underline = true;
                            break;
                        case StrikethroughSpan.SpanKey: // TextElement.TextDecorations = None, Strikethought, Underline
                            metaFonts[i].Strikethrough = true;
                            break;
                        case SuperscriptSpan.SpanKey: // Run with Typographic.Variants=FontVariants.Superscript while using Cambria
                            metaFonts[i].Baseline = FontBaseline.Superscript;
                            break;
                        case SubscriptSpan.SpanKey: // Run with Typographic.Varients=FontVariants.Subscript while using Cambria
                            metaFonts[i].Baseline = FontBaseline.Subscript;
                            break;
                        case NumeratorSpan.SpanKey: // no UWP solution - need to use SuperScript
                            metaFonts[i].Baseline = FontBaseline.Numerator;
                            break;
                        case DenominatorSpan.SpanKey: // no UWP solution - need to use Subscript
                            metaFonts[i].Baseline = FontBaseline.Denominator;
                            break;
                        case HyperlinkSpan.SpanKey:  // Hyperlink span ??
                            metaFonts[i].Action = new MetaFontAction((HyperlinkSpan)span);
                            break;
                        case BackgroundColorSpan.SpanKey: // if Win10 fall creator's update, there is a solution: create TextHighlighter, set its BackgroundColor and add the Range (Start/End) to it's Ranges, and add to TextBlock.Highlighters
                            metaFonts[i].BackgroundColor = ((BackgroundColorSpan)span).Color;
                            break;
                        default:
                            break;
                    }
                }
            }
            #endregion


            #region Convert MetaFonts to InLines
            // run through MetaFonts to see if we need to set new Font attributes
            var lastMetaFont = baseMetaFont;
            var startIndex = 0;
            for (int i = 0; i < metaFonts.Count; i++)
            {
                var metaFont = metaFonts[i];
                if (lastMetaFont != metaFont)
                {
                    // we are at the start of a new span
                    if (i > 0) // && lastMetaFont != baseMetaFont)
                        AddInline(textBlock, lastMetaFont, text, startIndex, i - startIndex);
                    lastMetaFont = metaFont;
                    startIndex = i;
                }
            }
            AddInline(textBlock, lastMetaFont, text, startIndex, text.Length - startIndex);
            #endregion

        }

        enum Decoration
        {
            Strikethrough,
            Underline,
        }


        static void ApplyTextDecorations(Run run, Decoration decoration)
        {
            try
            {
                switch (decoration)
                {
                    case Decoration.Strikethrough:
                        run.TextDecorations |= Windows.UI.Text.TextDecorations.Strikethrough;
                        break;
                    case Decoration.Underline:
                        run.TextDecorations |= Windows.UI.Text.TextDecorations.Underline;
                        break;
                    default:
                        break;
                }
            }
#pragma warning disable CC0004 // Catch block cannot be empty
            catch (Exception)
            {
                // o
            }
#pragma warning restore CC0004 // Catch block cannot be empty
        }

        static void AddInline(TextBlock textBlock, MetaFont metaFont, string text, int startIndex, int length)
        {
            if (metaFont.IsActiveAction)
            {
                var link = new Hyperlink
                {
                    NavigateUri = new Uri(metaFont.Action.Href)
                };
                textBlock.Inlines.Add(link);
                return;
            }

            var run = new Run
            {
                Text = text.Substring(startIndex, length),
                FontSize = metaFont.Size,
                FontWeight = metaFont.Bold ? Windows.UI.Text.FontWeights.Bold : Windows.UI.Text.FontWeights.Normal,
                FontStyle = metaFont.Italic ? Windows.UI.Text.FontStyle.Italic : Windows.UI.Text.FontStyle.Normal
            };


            if (TextDecorationsPresent && metaFont.Strikethrough)
                ApplyTextDecorations(run, Decoration.Strikethrough);

            switch (metaFont.Baseline)
            {
                case FontBaseline.Numerator:
                    run.FontFamily = ScriptFontFamily;
                    Typography.SetVariants(run, Windows.UI.Xaml.FontVariants.Superscript);
                    break;
                case FontBaseline.Superscript:
                    run.FontFamily = ScriptFontFamily;
                    Typography.SetVariants(run, Windows.UI.Xaml.FontVariants.Superscript);
                    break;
                case FontBaseline.Denominator:
                    run.FontFamily = ScriptFontFamily;
                    Typography.SetVariants(run, Windows.UI.Xaml.FontVariants.Subscript);
                    break;
                case FontBaseline.Subscript:
                    run.FontFamily = ScriptFontFamily;
                    Typography.SetVariants(run, Windows.UI.Xaml.FontVariants.Subscript);
                    break;
                default:
                    if (metaFont.Family != null)
                        run.FontFamily = metaFont.Family;
                    break;
            }

            if (!metaFont.BackgroundColor.IsDefaultOrTransparent())
            {
                try
                {
                    textBlock.ApplyBackgroundColor(metaFont.BackgroundColor, startIndex, length);
                }
#pragma warning disable CC0004 // Catch block cannot be empty
                catch (Exception)
                {
                    //throw new Exception("It appears that this Xamarin.Forms.UWP app was built with a Windows TargetVersion < 10.0.16299.0 (Windows 10 Fall Creators Update).  10.0.16299.0 is needed to support Forms9Patch.Label.HtmlText background color attributes.", e);
                }
#pragma warning restore CC0004 // Catch block cannot be empty
            }

            //if (metaFont.IsActionEmpty())
            {
                if (metaFont.TextColor != default)
                    run.Foreground = new SolidColorBrush(metaFont.TextColor);
                else
                    run.Foreground = new SolidColorBrush((Color)Application.Current.Resources["SystemBaseHighColor"]);
                if (TextDecorationsPresent && metaFont.Underline)
                    ApplyTextDecorations(run, Decoration.Underline);
                textBlock.Inlines.Add(run);
            }
            /*
            else
            {
                run.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Xamarin.Forms.Color.Blue.ToWindowsColor());
                if (TextDecorationsPresent)
                    ApplyTextDecorations(run, Decoration.Underline);
                var hyperlink = new Hyperlink();
                hyperlink.Inlines.Add(run);
                hyperlink.Click += (Hyperlink sender, HyperlinkClickEventArgs args) => label.Tap(metaFont.Action.Id, metaFont.Action.Href);
                textBlock.Inlines.Add(hyperlink);
            }
            */
        }


        static bool _textHighlighterPresent;
        static bool _textHighlighterPresentSet;
        public static bool TextHighLighterPresent
        {
            get
            {
                if (!_textHighlighterPresentSet)
                {
                    _textHighlighterPresent = Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.Xaml.Documents.TextHighlighter");
                    _textHighlighterPresentSet = true;
                }
                return _textHighlighterPresent;
            }
        }

        /// <summary>
        /// WARNING!  Calling this will cause a crash IF target version of APP is not set to Windows10 FallCreatorsUpdate (10.0.16299.0) or greater
        /// </summary>
        /// <param name="textBlock"></param>
        /// <param name="color"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        public static void ApplyBackgroundColor(this TextBlock textBlock, Color color, int startIndex = 0, int length = -1)
        {
            if (TextHighLighterPresent)
            {

                if (length < 0)
                {
                    if (startIndex != 0)
                        return;
                    length = textBlock.Text.Length;
                    length += textBlock.Inlines.Count;
                }

                var highlighter = new TextHighlighter
                {
                    Background = new SolidColorBrush(color),
                    //Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(metaFont.TextColor.ToWindowsColor()),
                };
                highlighter.Ranges.Add(new Windows.UI.Xaml.Documents.TextRange
                {
                    StartIndex = startIndex,
                    Length = length
                });
                textBlock.TextHighlighters.Add(highlighter);
            }
        }

        public static TextBlock Copy(this Windows.UI.Xaml.Controls.TextBlock textBlock)
        {
            if (textBlock is TextBlock source)
            {
                var result = new TextBlock
                {
                    FontSize = source.FontSize,
                    LineStackingStrategy = source.LineStackingStrategy,
                    LineHeight = source.LineHeight,
                    CharacterSpacing = source.CharacterSpacing,
                    IsTextSelectionEnabled = source.IsTextSelectionEnabled,
                    FontWeight = source.FontWeight,
                    Padding = source.Padding,
                    Foreground = source.Foreground,
                    FontStyle = source.FontStyle,
                    FontStretch = source.FontStretch,
                    FontFamily = source.FontFamily,
                    TextWrapping = source.TextWrapping,
                    TextTrimming = source.TextTrimming,
                    TextAlignment = source.TextAlignment,
                    Text = source.Text,
                    OpticalMarginAlignment = source.OpticalMarginAlignment,
                    TextReadingOrder = source.TextReadingOrder,
                    TextLineBounds = source.TextLineBounds,
                    SelectionHighlightColor = source.SelectionHighlightColor,
                    MaxLines = source.MaxLines,
                    IsColorFontEnabled = source.IsColorFontEnabled,
                    IsTextScaleFactorEnabled = source.IsTextScaleFactorEnabled,
                    TextDecorations = source.TextDecorations,
                    HorizontalTextAlignment = source.HorizontalTextAlignment,
                    FlowDirection = source.FlowDirection,
                    DataContext = source.DataContext,
                    Name = source.Name + ".Copy",
                    MinWidth = source.MinWidth,
                    MinHeight = source.MinHeight,
                    MaxWidth = source.MaxWidth,
                    MaxHeight = source.MaxHeight,
                    Margin = source.Margin,
                    Language = source.Language,
                    HorizontalAlignment = source.HorizontalAlignment,
                    VerticalAlignment = source.VerticalAlignment,
                    Width = source.Width,
                    Height = source.Height,
                    Style = source.Style,
                    RequestedTheme = source.RequestedTheme,
                    FocusVisualSecondaryThickness = source.FocusVisualSecondaryThickness,
                    FocusVisualSecondaryBrush = source.FocusVisualSecondaryBrush,
                    FocusVisualPrimaryThickness = source.FocusVisualPrimaryThickness,
                    FocusVisualPrimaryBrush = source.FocusVisualPrimaryBrush,
                    FocusVisualMargin = source.FocusVisualMargin,
                    AllowFocusWhenDisabled = source.AllowFocusWhenDisabled,
                    AllowFocusOnInteraction = source.AllowFocusOnInteraction,
                    Clip = source.Clip
                };
                return result;
            }
            return null;
        }

        public static TextAlignment AsTextAlignment(this HorizontalAlignment horizontalAlignment)
        {
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    return TextAlignment.Center;
                case HorizontalAlignment.Left:
                    return TextAlignment.Left;
                case HorizontalAlignment.Right:
                    return TextAlignment.Right;
                case HorizontalAlignment.Stretch:
                    return TextAlignment.Justify;
            }
            return TextAlignment.Left;
        }

        public static double EnabledToOpacity(bool isEnabled)
            => isEnabled ? 1.0 : 0.5;

        public static double FloorFontSize(double fontSize)
            => Math.Max(fontSize, Settings.MinFontSize);
    }
}
