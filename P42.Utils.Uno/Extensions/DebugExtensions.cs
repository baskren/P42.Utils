namespace P42.Utils.Uno;

/// <summary>
/// Help with debugging
/// </summary>
// ReSharper disable once UnusedType.Global
public static class DebugExtensions
{
    /// <summary>
    /// Write ContentControl properties to a StringWriter
    /// </summary>
    /// <param name="control"></param>
    /// <param name="writer"></param>
    /// <returns></returns>
    public static StringWriter LogProperties(this ContentControl control, StringWriter? writer = null)
    {
        writer ??= new StringWriter();
        writer.WriteLine("ContentControl: ");
        writer.WriteLine($"\t ContentTemplate: {control.ContentTemplate}");
        writer.WriteLine($"\t Content: {control.Content}");
        writer.WriteLine($"\t ContentTemplateRoot: {control.ContentTemplateRoot}");
        return LogProperties((Control)control, writer);
    }

    /// <summary>
    /// Write Control properties to a StringWriter
    /// </summary>
    /// <param name="control"></param>
    /// <param name="writer"></param>
    /// <returns></returns>
    public static StringWriter LogProperties(this Control control, StringWriter? writer = null)
    {
        writer ??= new StringWriter();
        writer.WriteLine("Control: ");
        writer.WriteLine($"\t Padding: {control.Padding}");
        writer.WriteLine($"\t IsTabStop: {control.IsTabStop}");
        writer.WriteLine($"\t IsEnabled: {control.IsEnabled}");
        writer.WriteLine($"\t HorizontalContentAlignment: {control.HorizontalContentAlignment}");
        writer.WriteLine($"\t Foreground: {control.Foreground}");
        writer.WriteLine($"\t FontWeight: {control.FontWeight}");
        writer.WriteLine($"\t FontStyle: {control.FontStyle}");
        writer.WriteLine($"\t FontStretch: {control.FontStretch}");
        writer.WriteLine($"\t FontSize: {control.FontSize}");
        writer.WriteLine($"\t FontFamily: {control.FontFamily}");
        writer.WriteLine($"\t TabIndex: {control.TabIndex}");
        writer.WriteLine($"\t CharacterSpacing: {control.CharacterSpacing}");
        writer.WriteLine($"\t BorderThickness: {control.BorderThickness}");
        writer.WriteLine($"\t BorderBrush: {control.BorderBrush}");
        writer.WriteLine($"\t TabNavigation: {control.TabNavigation}");
        writer.WriteLine($"\t Background: {control.Background}");
        writer.WriteLine($"\t VerticalControlAlignment: {control.VerticalContentAlignment}");
        writer.WriteLine($"\t Template: {control.Template}");
        writer.WriteLine($"\t FocusState: {control.FocusState}");
        writer.WriteLine($"\t IsTextScaleFactorEnabled: {control.IsTextScaleFactorEnabled}");
        writer.WriteLine($"\t UseSystemFocusVisuals: {control.UseSystemFocusVisuals}");
        writer.WriteLine($"\t XYFocusUp: {control.XYFocusUp}");
        writer.WriteLine($"\t XYFocusRight: {control.XYFocusRight}");
        writer.WriteLine($"\t XYFocusLeft: {control.XYFocusLeft}");
        writer.WriteLine($"\t XYFocusDown: {control.XYFocusDown}");
        writer.WriteLine($"\t RequiresPointer: {control.RequiresPointer}");
        writer.WriteLine($"\t IsFocusEngagementEnabled: {control.IsFocusEngagementEnabled}");
        writer.WriteLine($"\t IsFocusEngaged: {control.IsFocusEngaged}");
        writer.WriteLine($"\t ElementSoundMode: {control.ElementSoundMode}");
        writer.WriteLine($"\t DefaultStyleResourceUri: {control.DefaultStyleResourceUri}");
        writer.WriteLine($"\t CornerRadius: {control.CornerRadius}");
        writer.WriteLine($"\t BackgroundSizing: {control.BackgroundSizing}");
        
        return LogProperties((FrameworkElement)control, writer);
    }

    /// <summary>
    /// Write FrameworkElement properties to a StringWriter
    /// </summary>
    /// <param name="element"></param>
    /// <param name="writer"></param>
    /// <returns></returns>
    public static StringWriter LogProperties(this FrameworkElement element, StringWriter? writer = null)
    {
        writer ??= new StringWriter();
        writer.WriteLine("FrameworkElement: ");
        writer.WriteLine($"\t ActualHeight: {element.ActualHeight}");
        writer.WriteLine($"\t ActualTheme: {element.ActualTheme}");
        writer.WriteLine($"\t ActualWidth: {element.ActualWidth}");
        writer.WriteLine($"\t AllowFocusOnInteraction: {element.AllowFocusOnInteraction}");
        writer.WriteLine($"\t AllowFocusOnDisabled: {element.AllowFocusWhenDisabled}");
        writer.WriteLine($"\t BaseUri: {element.BaseUri}");
        writer.WriteLine($"\t DataContext: {element.DataContext}");
        writer.WriteLine($"\t FlowDirection: {element.FlowDirection}");
        writer.WriteLine($"\t FocusVisualMargin: {element.FocusVisualMargin}");
        writer.WriteLine($"\t FocusVisualPrimaryBrush: {element.FocusVisualPrimaryBrush}");
        writer.WriteLine($"\t FocusVisualPrimaryThickness: {element.FocusVisualPrimaryThickness}");
        writer.WriteLine($"\t FocusVisualSecondaryBrush: {element.FocusVisualSecondaryBrush}");
        writer.WriteLine($"\t FocusVisualSecondaryThickness: {element.FocusVisualSecondaryThickness}");
        writer.WriteLine($"\t Height: {element.Height}");
        writer.WriteLine($"\t HorizontalAlignment: {element.HorizontalAlignment}");
        writer.WriteLine($"\t IsLoaded: {element.IsLoaded}");
        writer.WriteLine($"\t Language: {element.Language}");
        writer.WriteLine($"\t Margin: {element.Margin}");
        writer.WriteLine($"\t MaxHeight: {element.MaxHeight}");
        writer.WriteLine($"\t MaxWidth: {element.MaxWidth}");
        writer.WriteLine($"\t MinHeight: {element.MinHeight}");
        writer.WriteLine($"\t MinWidth: {element.MinWidth}");
        writer.WriteLine($"\t Name: {element.Name}");
        writer.WriteLine($"\t Parent: {element.Parent}");
        writer.WriteLine($"\t RequestedTheme: {element.RequestedTheme}");
        writer.WriteLine($"\t Resources: {element.Resources}");
        writer.WriteLine($"\t Style: {element.Style}");
        writer.WriteLine($"\t Tag: {element.Tag}");
        //writer.WriteLine($"\t Triggers: {element.Triggers}");
        writer.WriteLine($"\t VerticalAlignment: {element.VerticalAlignment}");
        writer.WriteLine($"\t Width: {element.Width}");
        
        return LogProperties((UIElement)element, writer);
    }


    /// <summary>
    /// Write UIElement properties to a StringWriter
    /// </summary>
    /// <param name="element"></param>
    /// <param name="writer"></param>
    /// <returns></returns>
    public static StringWriter LogProperties(this UIElement element, StringWriter? writer = null)
    {
        writer ??= new StringWriter();
        writer.WriteLine("UIElement: ");
        writer.WriteLine($"\t AccessKey: {element.AccessKey}");
        writer.WriteLine($"\t AccessKeyScopeOwner: {element.AccessKeyScopeOwner}");
        writer.WriteLine($"\t ActualOffset: {element.ActualOffset}");
        writer.WriteLine($"\t ActualSize: {element.ActualSize}");
        writer.WriteLine($"\t AllowDrop: {element.AllowDrop}");
        writer.WriteLine($"\t CacheMode: {element.CacheMode}");
        writer.WriteLine($"\t CanBeScrollAnchor: {element.CanBeScrollAnchor}");
        writer.WriteLine($"\t CanDrag: {element.CanDrag}");
        writer.WriteLine($"\t CenterPoint: {element.CenterPoint}");
        writer.WriteLine($"\t Clip: {element.Clip}");
        writer.WriteLine($"\t CompositeMode: {element.CompositeMode}");
        writer.WriteLine($"\t ContextFlyout: {element.ContextFlyout}");
        writer.WriteLine($"\t DesiredSize: {element.DesiredSize}");
        writer.WriteLine($"\t HighContrastAdjustment: {element.HighContrastAdjustment}");
        writer.WriteLine($"\t IsAccessKeyScope: {element.IsAccessKeyScope}");
        writer.WriteLine($"\t IsDoubleTapEnabled: {element.IsDoubleTapEnabled}");
        writer.WriteLine($"\t IsHitTestEnabled: {element.IsHitTestVisible}");
        writer.WriteLine($"\t IsHoldingEnabled: {element.IsHoldingEnabled}");
        writer.WriteLine($"\t IsRightTapEnabled: {element.IsRightTapEnabled}");
        writer.WriteLine($"\t IsTapEnabled: {element.IsTapEnabled}");
        writer.WriteLine($"\t KeyboardAcceleratorPlacementMode: {element.KeyboardAcceleratorPlacementMode}");
        writer.WriteLine($"\t KeyboardAcceleratorPlacementTarget: {element.KeyboardAcceleratorPlacementTarget}");
        writer.WriteLine($"\t KeyboardAccelerators: {element.KeyboardAccelerators}");
        writer.WriteLine($"\t KeyTipHorizontalOffset: {element.KeyTipHorizontalOffset}");
        writer.WriteLine($"\t KeyTipPlacementMode: {element.KeyTipPlacementMode}");
        writer.WriteLine($"\t KeyTipTarget: {element.KeyTipTarget}");
        writer.WriteLine($"\t KeyTipVerticalOffset: {element.KeyTipVerticalOffset}");
        writer.WriteLine($"\t Lights: {element.Lights}");
        writer.WriteLine($"\t ManipulationMode: {element.ManipulationMode}");
        writer.WriteLine($"\t Opacity: {element.Opacity}");
        writer.WriteLine($"\t OpacityTransition: {element.OpacityTransition}");
        writer.WriteLine($"\t RenderSize: {element.RenderSize}");
        writer.WriteLine($"\t Rotation: {element.Rotation}");
        writer.WriteLine($"\t RotationAxis: {element.RotationAxis}");
        writer.WriteLine($"\t Scale: {element.Scale}");
        writer.WriteLine($"\t Shadow: {element.Shadow}");
        writer.WriteLine($"\t TabFocusNavigation: {element.TabFocusNavigation}");
        writer.WriteLine($"\t Translation: {element.Translation}");
        // writer.WriteLine("\t UIContext: " + element.UIContext);
        writer.WriteLine($"\t UseLayoutRounding: {element.UseLayoutRounding}");
        writer.WriteLine($"\t Visibility: {element.Visibility}");
        writer.WriteLine($"\t XamlRoot: {element.XamlRoot}");
        
        return writer;
    }
}
