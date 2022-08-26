using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace P42.Utils.Uno
{
    public static class DebugExtensions
    {
        public static void DebugLogProperties(this ContentControl control)
        {
            /*
            Debug.WriteLine("\t ContentTemplate: " + control.ContentTemplate);
            Debug.WriteLine("\t Content: " + control.Content);
            Debug.WriteLine("\t ContentTemplateRoot: " + control.ContentTemplateRoot);
            */
            DebugLogProperties((Control)control);
        }

        public static void DebugLogProperties(this Control control)
        {
            /*
            Debug.WriteLine("\t Padding: " + control.Padding);
            Debug.WriteLine("\t IsTabStop: " + control.IsTabStop);
            Debug.WriteLine("\t IsEnabled: " + control.IsEnabled);
            Debug.WriteLine("\t HorizontalContentAlignment: " + control.HorizontalContentAlignment);
            Debug.WriteLine("\t Foreground: " + control.Foreground);
            Debug.WriteLine("\t FontWeight: " + control.FontWeight);
            Debug.WriteLine("\t FontStyle: " + control.FontStyle);
            Debug.WriteLine("\t FontStretch: " + control.FontStretch);
            Debug.WriteLine("\t FontSize: " + control.FontSize);
            Debug.WriteLine("\t FontFamily: " + control.FontFamily);
            Debug.WriteLine("\t TabIndex: " + control.TabIndex);
            Debug.WriteLine("\t CharacterSpacing: " + control.CharacterSpacing);
            Debug.WriteLine("\t BorderThickness: " + control.BorderThickness);
            Debug.WriteLine("\t BorderBrush: " + control.BorderBrush);
            Debug.WriteLine("\t TabNavigation: " + control.TabNavigation);
            Debug.WriteLine("\t Background: " + control.Background);
            Debug.WriteLine("\t VerticalControlAlignment: " + control.VerticalContentAlignment);
            Debug.WriteLine("\t Template: " + control.Template);
            Debug.WriteLine("\t FocusState: " + control.FocusState);
            Debug.WriteLine("\t IsTextScaleFactorEnabled: " + control.IsTextScaleFactorEnabled);
            Debug.WriteLine("\t UseSystemFocusVisuals: " + control.UseSystemFocusVisuals);
            Debug.WriteLine("\t XYFocusUp: " + control.XYFocusUp);
            Debug.WriteLine("\t XYFocusRight: " + control.XYFocusRight);
            Debug.WriteLine("\t XYFocusLeft: " + control.XYFocusLeft);
            Debug.WriteLine("\t XYFocusDown: " + control.XYFocusDown);
            Debug.WriteLine("\t RequiresPointer: " + control.RequiresPointer);
            Debug.WriteLine("\t IsFocusEngagementEnabled: " + control.IsFocusEngagementEnabled);
            Debug.WriteLine("\t IsFocusEngaged: " + control.IsFocusEngaged);
            Debug.WriteLine("\t ElementSoundMode: " + control.ElementSoundMode);
            Debug.WriteLine("\t DefaultStyleResourceUri: " + control.DefaultStyleResourceUri);
            Debug.WriteLine("\t CornerRadius: " + control.CornerRadius);
            Debug.WriteLine("\t BackgroundSizing: " + control.BackgroundSizing);
            */
            DebugLogProperties((FrameworkElement)control);
        }

        public static void DebugLogProperties(this FrameworkElement element)
        {
            /*
            Debug.WriteLine("\t ActualHeight: " + element.ActualHeight);
            Debug.WriteLine("\t ActualTheme: " + element.ActualTheme);
            Debug.WriteLine("\t ActualWidth: " + element.ActualWidth);
            Debug.WriteLine("\t AllowFocusOnInteraction: " + element.AllowFocusOnInteraction);
            Debug.WriteLine("\t AllowFocusOnDisabled: " + element.AllowFocusWhenDisabled);
            Debug.WriteLine("\t BaseUri: " + element.BaseUri);
            Debug.WriteLine("\t DataContext: " + element.DataContext);
            Debug.WriteLine("\t FlowDirection: " + element.FlowDirection);
            Debug.WriteLine("\t FocusVisualMargin: " + element.FocusVisualMargin);
            Debug.WriteLine("\t FocusVisualPrimaryBrush: " + element.FocusVisualPrimaryBrush);
            Debug.WriteLine("\t FocusVisualPrimaryThickness: " + element.FocusVisualPrimaryThickness);
            Debug.WriteLine("\t FocusVisualSecondaryBrush: " + element.FocusVisualSecondaryBrush);
            Debug.WriteLine("\t FocusVisualSecondaryThickness: " + element.FocusVisualSecondaryThickness);
            Debug.WriteLine("\t Height: " + element.Height);
            Debug.WriteLine("\t HorizontalAlignment: " + element.HorizontalAlignment);
            Debug.WriteLine("\t IsLoaded: " + element.IsLoaded);
            Debug.WriteLine("\t Language: " + element.Language);
            Debug.WriteLine("\t Margin: " + element.Margin);
            Debug.WriteLine("\t MaxHeight: " + element.MaxHeight);
            Debug.WriteLine("\t MaxWidth: " + element.MaxWidth);
            Debug.WriteLine("\t MinHeight: " + element.MinHeight);
            Debug.WriteLine("\t MinWidth: " + element.MinWidth);
            Debug.WriteLine("\t Name: " + element.Name);
            Debug.WriteLine("\t Parent: " + element.Parent);
            Debug.WriteLine("\t RequestedTheme: " + element.RequestedTheme);
            Debug.WriteLine("\t Resources: " + element.Resources);
            Debug.WriteLine("\t Style: " + element.Style);
            Debug.WriteLine("\t Tag: " + element.Tag);
            Debug.WriteLine("\t Triggers: " + element.Triggers);
            Debug.WriteLine("\t VerticalAlignment: " + element.VerticalAlignment);
            Debug.WriteLine("\t Width: " + element.Width);
            */
            DebugLogProperties((UIElement)element);
        }


        public static void DebugLogProperties(this UIElement element)
        {
            /*
            Debug.WriteLine("\t AccessKey: " + element.AccessKey);
            Debug.WriteLine("\t AccessKeyScopeOwner: " + element.AccessKeyScopeOwner);
            //Debug.WriteLine("\t ActualOffset: " + element.ActualOffset);
            //Debug.WriteLine("\t ActualSize: " + element.ActualSize);
            Debug.WriteLine("\t AllowDrop: " + element.AllowDrop);
            Debug.WriteLine("\t CacheMode: " + element.CacheMode);
            Debug.WriteLine("\t CanBeScrollAnchor: " + element.CanBeScrollAnchor);
            Debug.WriteLine("\t CanDrag: " + element.CanDrag);
            Debug.WriteLine("\t CenterPoint: " + element.CenterPoint);
            Debug.WriteLine("\t Clip: " + element.Clip);
            Debug.WriteLine("\t CompositeMode: " + element.CompositeMode);
            Debug.WriteLine("\t ContextFlyout: " + element.ContextFlyout);
            Debug.WriteLine("\t DesiredSize: " + element.DesiredSize);
            Debug.WriteLine("\t HighContrastAdjustment: " + element.HighContrastAdjustment);
            Debug.WriteLine("\t IsAccessKeyScope: " + element.IsAccessKeyScope);
            Debug.WriteLine("\t IsDoubleTapEnabled: " + element.IsDoubleTapEnabled);
            Debug.WriteLine("\t IsHitTestEnabled: " + element.IsHitTestVisible);
            Debug.WriteLine("\t IsHoldingEnabled: " + element.IsHoldingEnabled);
            Debug.WriteLine("\t IsRightTapEnabled: " + element.IsRightTapEnabled);
            Debug.WriteLine("\t IsTapEnabled: " + element.IsTapEnabled);
            Debug.WriteLine("\t KeyboardAcceleratorPlacementMode: " + element.KeyboardAcceleratorPlacementMode);
            Debug.WriteLine("\t KeyboardAcceleratorPlacementTarget: " + element.KeyboardAcceleratorPlacementTarget);
            Debug.WriteLine("\t KeyboardAccelerators: " + element.KeyboardAccelerators);
            Debug.WriteLine("\t KeyTipHorizontalOffset: " + element.KeyTipHorizontalOffset);
            Debug.WriteLine("\t KeyTipPlacementMode: " + element.KeyTipPlacementMode);
            Debug.WriteLine("\t KeyTipTarget: " + element.KeyTipTarget);
            Debug.WriteLine("\t KeyTipVerticalOffset: " + element.KeyTipVerticalOffset);
            Debug.WriteLine("\t Lights: " + element.Lights);
            Debug.WriteLine("\t ManipulationMode: " + element.ManipulationMode);
            Debug.WriteLine("\t Opacity: " + element.Opacity);
            Debug.WriteLine("\t OpacityTransition: " + element.OpacityTransition);
            Debug.WriteLine("\t RenderSize: " + element.RenderSize);
            Debug.WriteLine("\t Rotation: " + element.Rotation);
            Debug.WriteLine("\t RotationAxis: " + element.RotationAxis);
            Debug.WriteLine("\t Scale: " + element.Scale);
            //Debug.WriteLine("\t Shadow: " + element.Shadow);
            Debug.WriteLine("\t TabFocusNavigation: " + element.TabFocusNavigation);
            Debug.WriteLine("\t Translation: " + element.Translation);
            //Debug.WriteLine("\t UIContext: " + element.UIContext);
            Debug.WriteLine("\t UseLayoutRounding: " + element.UseLayoutRounding);
            Debug.WriteLine("\t Visibility: " + element.Visibility);
            //Debug.WriteLine("\t XamlRoot: " + element.XamlRoot);
            */
        }
    }
}
