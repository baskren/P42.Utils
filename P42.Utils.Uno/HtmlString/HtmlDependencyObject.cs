using System;
using System.Collections.Generic;
using System.Text;
using Uno.UI;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Data;

namespace P42.Utils.Uno
{
    [Windows.UI.Xaml.Data.Bindable]
    //[System.ComponentModel.Bindable(System.ComponentModel.BindableSupport.Yes)]
    partial class HtmlDependencyObject : DependencyObject
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
        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            nameof(Foreground),
            typeof(Brush),
            typeof(HtmlDependencyObject),
            new PropertyMetadata(default(Brush), OnFontPropertyChanged)
        );
        public Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }
        #endregion Foreground Property

        #region FontWeight Property
        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register(
            nameof(FontWeight),
            typeof(FontWeight),
            typeof(HtmlDependencyObject),
            new PropertyMetadata(default(FontWeight), OnFontPropertyChanged)
        );
        public FontWeight FontWeight
        {
            get => (FontWeight)GetValue(FontWeightProperty);
            set => SetValue(FontWeightProperty, value);
        }
        #endregion FontWeight Property

        #region FontStyle Property
        public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register(
            nameof(FontStyle),
            typeof(FontStyle),
            typeof(HtmlDependencyObject),
            new PropertyMetadata(default(FontStyle), OnFontPropertyChanged)
        );
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
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
            nameof(FontSize),
            typeof(double),
            typeof(HtmlDependencyObject),
            new PropertyMetadata(default(double), OnFontPropertyChanged)
        );
        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }
        #endregion FontSize Property

        #region FontFamily Property
        public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register(
            nameof(FontFamily),
            typeof(FontFamily),
            typeof(HtmlDependencyObject),
            new PropertyMetadata(default(FontFamily), OnFontPropertyChanged)
        );
        public FontFamily FontFamily
        {
            get => (FontFamily)GetValue(FontFamilyProperty);
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
        public static readonly DependencyProperty HtmlSpansProperty = DependencyProperty.Register(
            nameof(HtmlSpans),
            typeof(HtmlSpans),
            typeof(HtmlDependencyObject),
            new PropertyMetadata(default(HtmlSpans),OnFontPropertyChanged)
        );
        public HtmlSpans HtmlSpans
        {
            get => (HtmlSpans)GetValue(HtmlSpansProperty);
            set => SetValue(HtmlSpansProperty, value);
        }
        #endregion HtmlSpans Property


        private static void OnFontPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HtmlDependencyObject html)
            {
                SetAndFormatText(html.targetTextBlock, html.HtmlSpans);
            }
        }



        TextBlock targetTextBlock;
        

        internal HtmlDependencyObject(TextBlock textBlock, HtmlSpans spans)
        {
            targetTextBlock = textBlock;
            HtmlSpans = spans;
            this.BindFont(textBlock);
        }

        #region Binding
        static List<string> GetExcepts<T>(object except)
        {
            if (except is null)
                return null;
            if (except is string str)
                return new List<string> { str };
            if (except is IEnumerable<string> enumerable)
                return new List<string>(enumerable);
            throw new Exception("BindFont except: argument must be null, string, or IEnumerable<string>");
        }

        void BindFont(Control source, BindingMode bindingMode = BindingMode.OneWay, object except = null)
        {
            var excepts = GetExcepts<Control>(except);
            if (excepts is null || !excepts.Contains(nameof(Control.FontFamily)))
                this.Bind(HtmlDependencyObject.FontFamilyProperty, source, nameof(Control.FontFamily), bindingMode);
            if (excepts is null || !excepts.Contains(nameof(Control.FontSize)))
                this.Bind(HtmlDependencyObject.FontSizeProperty, source, nameof(Control.FontSize), bindingMode);
            //if (excepts is null || !excepts.Contains(nameof(Control.FontStretch)))
            //    this.Bind(HtmlDependencyObject.FontStretchProperty, source, nameof(Control.FontStretch), bindingMode);
            if (excepts is null || !excepts.Contains(nameof(Control.FontStyle)))
                this.Bind(HtmlDependencyObject.FontStyleProperty, source, nameof(Control.FontStyle), bindingMode);
            if (excepts is null || !excepts.Contains(nameof(Control.FontWeight)))
                this.Bind(HtmlDependencyObject.FontWeightProperty, source, nameof(Control.FontWeight), bindingMode);
            if (excepts is null || !excepts.Contains(nameof(Control.Foreground)))
                this.Bind(HtmlDependencyObject.ForegroundProperty, source, nameof(Control.Foreground), bindingMode);
        }
        void BindFont(TextBlock source, BindingMode bindingMode = BindingMode.OneWay, object except = null)
        {
            var excepts = GetExcepts<TextBlock>(except);
            if (excepts is null || !excepts.Contains(nameof(TextBlock.FontFamily)))
                this.Bind(HtmlDependencyObject.FontFamilyProperty, source, nameof(TextBlock.FontFamily), bindingMode);
            if (excepts is null || !excepts.Contains(nameof(TextBlock.FontSize)))
                this.Bind(HtmlDependencyObject.FontSizeProperty, source, nameof(TextBlock.FontSize), bindingMode);
            //if (excepts is null || !excepts.Contains(nameof(TextBlock.FontStretch)))
            //    this.Bind(HtmlDependencyObject.FontStretchProperty, source, nameof(TextBlock.FontStretch), bindingMode);
            if (excepts is null || !excepts.Contains(nameof(TextBlock.FontStyle)))
                this.Bind(HtmlDependencyObject.FontStyleProperty, source, nameof(TextBlock.FontStyle), bindingMode);
            if (excepts is null || !excepts.Contains(nameof(TextBlock.FontWeight)))
                this.Bind(HtmlDependencyObject.FontWeightProperty, source, nameof(TextBlock.FontWeight), bindingMode);
            if (excepts is null || !excepts.Contains(nameof(TextBlock.Foreground)))
                this.Bind(HtmlDependencyObject.ForegroundProperty, source, nameof(TextBlock.Foreground), bindingMode);
        }
        void BindFont(ContentPresenter source, BindingMode bindingMode = BindingMode.OneWay, object except = null)
        {
            var excepts = GetExcepts<ContentPresenter>(except);
            if (excepts is null || !excepts.Contains(nameof(ContentPresenter.FontFamily)))
                this.Bind(HtmlDependencyObject.FontFamilyProperty, source, nameof(ContentPresenter.FontFamily), bindingMode);
            if (excepts is null || !excepts.Contains(nameof(ContentPresenter.FontSize)))
                this.Bind(HtmlDependencyObject.FontSizeProperty, source, nameof(ContentPresenter.FontSize), bindingMode);
            //if (excepts is null || !excepts.Contains(nameof(ContentPresenter.FontStretch)))
            //    this.Bind(HtmlDependencyObject.FontStretchProperty, source, nameof(ContentPresenter.FontStretch), bindingMode);
            if (excepts is null || !excepts.Contains(nameof(ContentPresenter.FontStyle)))
                this.Bind(HtmlDependencyObject.FontStyleProperty, source, nameof(ContentPresenter.FontStyle), bindingMode);
            if (excepts is null || !excepts.Contains(nameof(ContentPresenter.FontWeight)))
                this.Bind(HtmlDependencyObject.FontWeightProperty, source, nameof(ContentPresenter.FontWeight), bindingMode);
            if (excepts is null || !excepts.Contains(nameof(ContentPresenter.Foreground)))
                this.Bind(HtmlDependencyObject.ForegroundProperty, source, nameof(ContentPresenter.Foreground), bindingMode);
        }

        #endregion



        static readonly FontFamily MathMetaFontFamily = new FontFamily("ms-appx:///Assets/Fonts/STIXGeneral.otf#STIXGeneral"); //FontExtensions.GetFontFamily("STIXGeneral");
        //static readonly FontFamily ScriptFontFamily = new FontFamily("ms-appx:///Assets/Fonts/segoeui.ttf#Segoe UI"); // new FontFamily("Segoe UI#Regular");
        static readonly FontFamily ScriptFontFamily = new FontFamily("Segoe UI#Regular");

        internal static void SetAndFormatText(TextBlock textBlock, HtmlSpans htmlSpans, double altFontSize = -1)
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
            //textBlock.LineStackingStrategy = Windows.UI.Xaml.LineStackingStrategy.BaselineToBaseline;

            var text = htmlSpans?.UnmarkedText;

            if (string.IsNullOrWhiteSpace(text) || text == htmlSpans?.Text)
            {
                // there isn't any markup!
                textBlock.Text = text ?? string.Empty;
                return;
            }

            Color tColor = default;
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
            var baseMetaFont = new MetaFont(
                textBlock.FontFamily,
                textBlock.FontSize, //(altFontSize > 0 ? altFontSize : label.DecipheredFontSize()), //(label.FontSize < 0 ? (double)Windows.UI.Xaml.Application.Current.Resources["ControlContentThemeFontSize"] : label.FontSize)), //label.FontSize,
                textBlock.FontWeight.Weight >= Windows.UI.Text.FontWeights.Bold.Weight,
                (textBlock.FontStyle & Windows.UI.Text.FontStyle.Italic) > 0,
                textColor: textBlock.Foreground is SolidColorBrush brush
                    ? brush.Color
                    : SystemColors.WindowTextColor //(Color)Application.Current.Resources["SystemBaseHighColor"]
                );

            var MathMetaFont = new MetaFont(baseMetaFont)
            {
                Family = MathMetaFontFamily //FontService.ReconcileFontFamily("Forms9Patch.Resources.Fonts.STIXGeneral.otf", P42.Utils.ReflectionExtensions.GetAssembly(typeof(Forms9Patch.Label)))
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


            if (TextBlockExtensions.TextDecorationsPresent && metaFont.Strikethrough)
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
                if (TextBlockExtensions.TextDecorationsPresent && metaFont.Underline)
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



    }
}
