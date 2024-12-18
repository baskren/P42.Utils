﻿using System;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using ElementType = Microsoft.UI.Xaml.Controls.TextBlock;
using System.Runtime.CompilerServices;

namespace P42.Utils.Uno
{
    [Microsoft.UI.Xaml.Data.Bindable]
    //[System.ComponentModel.Bindable(System.ComponentModel.BindableSupport.Yes)]
    public static class TextBlockExtensions
    {
        #region Link Tapped Property
        public static readonly DependencyProperty LinkTappedProperty =
            DependencyProperty.RegisterAttached("LinkTapped", typeof(Action<string,string>), typeof(TextBlockExtensions), new PropertyMetadata(null));

        public static TextBlock SetLinkTappedHandler(this TextBlock textBlock, Action<string, string> value)
        {
            textBlock.SetValue(LinkTappedProperty, value);
            return textBlock;
        }

        public static Action<string, string> GetLinkTappedHandler(this TextBlock textBlock)
            => (Action<string,string>)textBlock.GetValue(LinkTappedProperty);
        #endregion


        #region Html Property
        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.RegisterAttached("Html", typeof(string), typeof(TextBlockExtensions), new PropertyMetadata(null, HtmlChanged));

        public static TextBlock SetHtml(this TextBlock textBlock, string value)
        {
            textBlock.SetValue(HtmlProperty, value ?? string.Empty);
            return textBlock;
        }

        public static string GetHtml(this TextBlock textBlock)
            => (string)textBlock.GetValue(HtmlProperty);


        public static ElementType Html(this ElementType element, string value)
        { element.SetHtml(value); return element; }

        public static ElementType BindHtml(this ElementType element, object source, string path,
            BindingMode mode = BindingMode.OneWay,
            IValueConverter converter = null,
            object converterParameter = null,
            string converterLanguage = null,
            UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
            object targetNullValue = null,
            object fallbackValue = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = -1
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
        internal static HtmlDependencyObject GetHtmlDependencyObject(this TextBlock element)
            => (HtmlDependencyObject)element.GetValue(HtmlDependencyObjectProperty);
        internal static void SetHtmlDependencyObject(this TextBlock element, HtmlDependencyObject value)
            => element.SetValue(HtmlDependencyObjectProperty, value);
        #endregion HtmlDependencyObject Property


        private static void HtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBlock textBlock)
                return;

            var text = (string)e.NewValue ?? string.Empty;
            try
            {
                var htmlSpans = new HtmlSpans(text);
                if (textBlock.GetHtmlDependencyObject() is HtmlDependencyObject html)
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
        public static TextBlock ApplyBackgroundColor(this TextBlock textBlock, Color color, int startIndex = 0, int length = -1)
        {
            if (TextHighLighterPresent)
            {

                if (length < 0)
                {
                    if (startIndex != 0)
                        return textBlock;
                    length = textBlock.Text.Length;
                    length += textBlock.Inlines.Count;
                }

                var highlighter = new TextHighlighter
                {
                    Background = new SolidColorBrush(color),
                    //Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(metaFont.TextColor.ToWindowsColor()),
                };
                highlighter.Ranges.Add(new Microsoft.UI.Xaml.Documents.TextRange
                {
                    StartIndex = startIndex,
                    Length = length
                });
                textBlock.TextHighlighters.Add(highlighter);
            }
            return textBlock;
        }


        public static double DefaultFontSize(this TextBlock textBlock)
            => (double)Application.Current.Resources["ControlContentThemeFontSize"];

        public static TextBlock Copy(this Microsoft.UI.Xaml.Controls.TextBlock textBlock)
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

        public static HorizontalAlignment AsHorizontalAlignment(this TextAlignment textAlignment)
        {
            switch(textAlignment)
            {
                case TextAlignment.Center: 
                    return HorizontalAlignment.Center;
                case TextAlignment.Left: 
                    return HorizontalAlignment.Left;
                case TextAlignment.Right: 
                    return HorizontalAlignment.Right;
                case TextAlignment.Justify:
                    return HorizontalAlignment.Stretch;
            }
            return HorizontalAlignment.Left;
        }

        public static double EnabledToOpacity(bool isEnabled)
            => isEnabled ? 1.0 : 0.5;

        public static double FloorFontSize(double fontSize)
            => Math.Max(fontSize, Platform.MinFontSize);

        static bool _textHighlighterPresent;
        static bool _textHighlighterPresentSet;
        public static bool TextHighLighterPresent
        {
            get
            {
                if (!_textHighlighterPresentSet)
                {
                    _textHighlighterPresent = Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Microsoft.UI.Xaml.Documents.TextHighlighter");
                    _textHighlighterPresentSet = true;
                }
                return _textHighlighterPresent;
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


    }
}
