﻿#if __ANDROID__
using Java.Interop;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Xaml.Media;

namespace P42.Utils.Uno
{
    public static class UIElementExtensions
    {
        public static bool HasPrescribedWidth(this FrameworkElement element) => !double.IsNaN(element.Width) && element.Width >= 0;
        public static bool HasPrescribedHeight(this FrameworkElement element) => !double.IsNaN(element.Height) && element.Height >= 0;

        public static bool HasMinWidth(this FrameworkElement element) => !double.IsNaN(element.MinWidth) && element.MinWidth >= 0;
        public static bool HasMinHeight(this FrameworkElement element) => !double.IsNaN(element.MinHeight) && element.MinHeight >= 0;

        public static bool HasMaxWidth(this FrameworkElement element) => !double.IsNaN(element.MaxWidth) && element.MaxWidth >= 0;
        public static bool HasMaxHeight(this FrameworkElement element) => !double.IsNaN(element.MaxHeight) && element.MaxHeight >= 0;


#if __ANDROID__

        static double _scale = -1;
        static double DisplayScale
        {
            get
            {
                if (_scale > 0)
                    return _scale;
                using var displayMetrics = new Android.Util.DisplayMetrics();
                using var service = global::Uno.UI.ContextHelper.Current.GetSystemService(Android.Content.Context.WindowService);
                using var windowManager = service?.JavaCast<Android.Views.IWindowManager>();
                var display = windowManager?.DefaultDisplay;
                display?.GetRealMetrics(displayMetrics);
                _scale = (double)displayMetrics?.Density;
                return _scale;
            }
        }

        public static Rect GetBounds(this UIElement element)
        {
            if (element is Android.Views.View view)
            {
                int[] nativeLocation = new int[2];
                //view.GetLocationOnScreen(nativeLocation);
                view.GetLocationInWindow(nativeLocation);
                var x = nativeLocation[0] / DisplayScale;
                var y = nativeLocation[1] / DisplayScale;
                return new Rect(x, y, element.ActualSize.X, element.ActualSize.Y);
            }
            return WinUIGetBounds(element);
        }

#elif __IOS__ || __MACOS__

        public static Rect GetBounds(this FrameworkElement element)
        {

            //var ttv = element.TransformToVisual(Platform.Window.Content);
            //var location = ttv.TransformPoint(new Point(0, 0));
            var rect = element.ConvertRectToView(element.Bounds, null);
            return new Rect(rect.X, rect.Y, rect.Width,rect.Height);
        }

        public static Rect GetBounds(this UIElement element)
        {
            var rect = element.ConvertRectToView(element.Bounds, null);
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }


#else
        public static Rect GetBounds(this FrameworkElement element)
            => WinUIGetBounds(element);

        public static Rect GetBounds(this UIElement element)
            => WinUIGetBounds(element);
#endif

        static Rect WinUIGetBounds(this FrameworkElement element)
        {
            var frame = Platform.Window.Content;
            var ttv = element.TransformToVisual(frame);
            var location = ttv.TransformPoint(new Point(0, 0));
            return new Rect(location, new Size(element.ActualWidth, element.ActualHeight));
        }

        static Rect WinUIGetBounds(this UIElement element)
        {
            if (element is FrameworkElement fwElement)
                return fwElement.WinUIGetBounds();
            var ttv = element.TransformToVisual(Platform.Window.Content);
            var location = ttv.TransformPoint(new Point(0, 0));
            return new Rect(location, new Size(element.DesiredSize.Width, element.DesiredSize.Height));
        }



        public static Rect GetBoundsRelativeTo(this FrameworkElement element, UIElement relativeToElement)
        {
            var ttv = element.TransformToVisual(relativeToElement);
            var location = ttv.TransformPoint(new Point(0, 0));
            return new Rect(location, new Size(element.ActualWidth, element.ActualHeight));
        }

        public static T FindAncestor<T>(this FrameworkElement element) where T : FrameworkElement
        {
            var parent = element.Parent as FrameworkElement;
            while (parent != null)
            {
                if (parent is T)
                    return parent as T;
                if (parent is FrameworkElement fe)
                    parent = fe.Parent as FrameworkElement;
                else
                    parent = null;
            }
            return null;
        }

        public static string AsDataTemplateXaml(this Type templateType)
        {
            if (templateType == null || !typeof(FrameworkElement).IsAssignableFrom(templateType))
                throw new Exception("Cannot convert type [" + templateType + "] into DataTemplate");
            string markup = string.Empty;
            //if (dataType is null)
            markup = $"<DataTemplate \n\t xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" \n\t xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" \n\t xmlns:local=\"using:{templateType.Namespace}\"> \n\t\t<local:{templateType.Name} /> \n</DataTemplate>";
            //if (dataType.Namespace == typeof(Type).Namespace)
            //    markup = $"<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:tlocal=\"using:{templateType.Namespace}\" xmlns:system=\"using:System\" x:DataType=\"system:{dataType.Name}\"><tlocal:{templateType.Name} /></DataTemplate>";
            //else
            //    markup = $"<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" \n\t xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" \n\t xmlns:tlocal=\"using:{templateType.Namespace}\" \n\t xmlns:dlocal=\"using:{dataType.Namespace}\" \n\t x:DataType=\"dlocal:{dataType.Name}\"> \n\t\t<tlocal:{templateType.Name} /> \n</DataTemplate>";
            // System.Diagnostics.Debug.WriteLine("BcGroupView.GenerateDataTemplate: markup: " + markup);
            //template.
            return markup;
        }

        public static DataTemplate AsDataTemplate(this Type templateType)
        {
            try
            {
                if (templateType == null || !typeof(FrameworkElement).IsAssignableFrom(templateType))
                    throw new Exception("Cannot convert type [" + templateType + "] into DataTemplate");
                var markup = $"<DataTemplate \n\t xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" \n\t xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" \n\t xmlns:local=\"using:{templateType.Namespace}\"> \n\t\t<local:{templateType.Name} /> \n</DataTemplate>";
                //System.Diagnostics.Trace.WriteLine($"AsDataTemplate : [{markup}]");
                var template = (DataTemplate)XamlReader.Load(markup);
                //template.
                return template;
            }
            catch (Exception e)
            {
                //System.Console.WriteLine($"EXCEPTION [{e.Message}] [{e.StackTrace}]");
                System.Diagnostics.Trace.WriteLine($"EXCEPTION [{e.Message}] [{e.StackTrace}]");
            }
            return null;
        }

#if __WASM__
        public static UIElement GetFirstHtmlDescendent(this FrameworkElement element)
        {
            var enumerator = element.GetEnumerator();
            while (enumerator.MoveNext())
            {
                //System.Console.WriteLine($"UIElementExtensions. enum.cur [{enumerator}] [{enumerator.Current.GetType()}] [{enumerator.Current.GetType().BaseType}]");
                if (enumerator.Current is UIElement fe)
                {
                    //System.Console.WriteLine($"UIElementExtensions. fe [{fe.GetHtmlAttribute("style")}]");
                    /*
                    foreach (var property in fe.GetProperties())
                    {
                        System.Console.WriteLine($"UIElementExtensions. property [{property.Name}] [{property.PropertyType}] [{property}] [{fe.GetPropertyValue(property.Name)}]");
                    }
                    */
                    return fe;
                }
            }
            //System.Console.WriteLine($"UIElementExtensions. NOTHING");
            return null;
        }
#endif

        public static DependencyObject FindChildByName(this DependencyObject parent, string ControlName)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is FrameworkElement && ((FrameworkElement)child).Name == ControlName)
                    return child;

                var FindResult = FindChildByName(child, ControlName);
                if (FindResult != null)
                    return FindResult;
            }

            return null;
        }


    }
}
