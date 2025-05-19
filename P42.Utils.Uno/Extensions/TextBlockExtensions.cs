using System;
using System.ComponentModel;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using ElementType = Microsoft.UI.Xaml.Controls.TextBlock;
using System.Runtime.CompilerServices;
using P42.Serilog.QuickLog;

namespace P42.Utils.Uno;

[Microsoft.UI.Xaml.Data.Bindable]
public static class TextBlockExtensions
{
 

    private static bool _applyBackgroundColorTested;
    private static bool _applyBackgroundColorSupported;


    /// <summary>
    /// WARNING!  Calling this will cause a crash IF target version of APP is not set to Windows10 FallCreatorsUpdate (10.0.16299.0) or greater
    /// </summary>
    /// <param name="textBlock"></param>
    /// <param name="color"></param>
    /// <param name="startIndex"></param>
    /// <param name="length"></param>
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static ElementType ApplyBackgroundColor(this ElementType textBlock, Color color, int startIndex = 0, int length = -1)
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
    /// <param name="source"></param>
    /// <returns></returns>
    public static ElementType Copy(this ElementType source)
         => new ()
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
                Name = $"{source.Name}.Copy",
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

    /// <summary>
    /// Convert HorizontalAlignment to TextAlignment
    /// </summary>
    /// <param name="horizontalAlignment"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Convert TextAlignment to HorizontalAlignment
    /// </summary>
    /// <param name="textAlignment"></param>
    /// <returns></returns>
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

    private static bool _textHighlighterPresent;
    private static bool _textHighlighterPresentSet;
    /// <summary>
    /// Is the TextHighlighter class available in this version of WinUI?
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
    /// Is TextDecorations available in this version of WinUI?
    /// </summary>
    public static bool TextDecorationsPresent
    {
        get
        {
            if (_textDecorationsPresentSet)
                return _textDecorationsPresent;

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
