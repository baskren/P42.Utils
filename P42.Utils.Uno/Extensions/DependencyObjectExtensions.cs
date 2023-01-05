using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using P42.Utils;

namespace P42.Utils.Uno
{
    public static class DependencyObjectExtensions
    {
		//const string bindingContextPath = Binding.SelfPath;

		/// <summary>Bind to a specified property</summary>
		internal static TBindable		Bind<TBindable>(
			this TBindable target,
			DependencyProperty targetProperty,
			object source,
			string path,
			BindingMode mode = BindingMode.OneWay,
			IValueConverter converter = null,
			object converterParameter = null,
			string converterLanguage = null,
			UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
			object targetNullValue = null,
			object fallbackValue = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = -1
		) where TBindable : DependencyObject
		{
			var binding = new Binding
			{
				Source = source,
				Mode = mode,
				Converter = converter,
				ConverterParameter = converterParameter,
				UpdateSourceTrigger = updateSourceTrigger,
				TargetNullValue = targetNullValue,
				FallbackValue = fallbackValue
			};
			if (!string.IsNullOrWhiteSpace(converterLanguage))
				binding.ConverterLanguage = converterLanguage;
			if (!string.IsNullOrWhiteSpace(path))
				binding.Path = new PropertyPath(path);
#if !HAS_UNO

			if (target is FrameworkElement element)
			{
				//element.ClearValue();
				element.SetBinding(targetProperty, binding);
				return target;
			}
#endif
			target.ClearValue(targetProperty);
			BindingOperations.SetBinding(target, targetProperty, binding);
			return target;
		}

		/// <summary>Bind to a specified property with inline conversion</summary>
		internal static TBindable Bind<TBindable, TSource, TDest>(
			this TBindable target,
			DependencyProperty targetProperty,
			object source = null,
			string path = null,
			BindingMode mode = BindingMode.OneWay,
			Func<TSource, TDest> convert = null,
			Func<TDest, TSource> convertBack = null,
			object converterParameter = null,
			string converterLanguage = null,
			UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
			object targetNullValue = null,
			object fallbackValue = null
		) where TBindable : DependencyObject
		{
			var converter = new FuncConverter<TSource, TDest, object>(convert, convertBack);
			var binding = new Binding
			{
				Source = source,
				Mode = mode,
				Converter = converter,
				ConverterParameter = converterParameter,
				UpdateSourceTrigger = updateSourceTrigger,
				TargetNullValue = targetNullValue,
				FallbackValue = fallbackValue
			};
			if (!string.IsNullOrWhiteSpace(converterLanguage))
				binding.ConverterLanguage = converterLanguage;
			if (!string.IsNullOrWhiteSpace(path))
				binding.Path = new PropertyPath(path);
#if !HAS_UNO

			if (target is FrameworkElement element)
			{
				//element.ClearValue();
				element.SetBinding(targetProperty, binding);
				return target;
			}
#endif
			target.ClearValue(targetProperty);
			BindingOperations.SetBinding(target, targetProperty, binding);
			return target;
		}

		/// <summary>Bind to a specified property with inline conversion and conversion parameter</summary>
		internal static TBindable Bind<TBindable, TSource, TParam, TDest>(
			this TBindable target,
			DependencyProperty targetProperty,
			object source = null,
			string path = null,
			BindingMode mode = BindingMode.OneWay,
			Func<TSource, TParam, TDest> convert = null,
			Func<TDest, TParam, TSource> convertBack = null,
			object converterParameter = null,
			string converterLanguage = null,
			UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
			object targetNullValue = null,
			object fallbackValue = null
		) where TBindable : DependencyObject
		{
			var converter = new FuncConverter<TSource, TDest, TParam>(convert, convertBack);
			var binding = new Binding
			{
				Source = source,
				Mode = mode,
				Converter = converter,
				ConverterParameter = converterParameter,
				UpdateSourceTrigger = updateSourceTrigger,
				TargetNullValue = targetNullValue,
				FallbackValue = fallbackValue
			};
			if (!string.IsNullOrWhiteSpace(converterLanguage))
				binding.ConverterLanguage = converterLanguage;
			if (!string.IsNullOrWhiteSpace(path))
				binding.Path = new PropertyPath(path);

#if !HAS_UNO

			if (target is FrameworkElement element)
            {
				//element.ClearValue();
				element.SetBinding(targetProperty, binding);
				return target;
            }
#endif
			target.ClearValue(targetProperty);
			BindingOperations.SetBinding(target, targetProperty, binding);
			return target;
		}

		/*
		/// <summary>Bind to the default property</summary>
		public static TBindable Bind<TBindable>(
			this TBindable bindable,
			string path = bindingContextPath,
			BindingMode mode = BindingMode.OneWay,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object targetNullValue = null,
			object fallbackValue = null
		) where TBindable : DependencyObject
		{
			bindable.Bind(
				DefaultBindableProperties.GetFor(bindable),
				path, mode, converter, converterParameter, stringFormat, source, targetNullValue, fallbackValue
			);
			return bindable;
		}

		/// <summary>Bind to the default property with inline conversion</summary>
		public static TBindable Bind<TBindable, TSource, TDest>(
			this TBindable bindable,
			string path = bindingContextPath,
			BindingMode mode = BindingMode.OneWay,
			Func<TSource, TDest> convert = null,
			Func<TDest, TSource> convertBack = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object targetNullValue = null,
			object fallbackValue = null
		) where TBindable : DependencyObject
		{
			var converter = new FuncConverter<TSource, TDest, object>(convert, convertBack);
			bindable.Bind(
				DefaultBindableProperties.GetFor(bindable),
				path, mode, converter, converterParameter, stringFormat, source, targetNullValue, fallbackValue
			);
			return bindable;
		}

		/// <summary>Bind to the default property with inline conversion and conversion parameter</summary>
		public static TBindable Bind<TBindable, TSource, TParam, TDest>(
			this TBindable bindable,
			string path = bindingContextPath,
			BindingMode mode = BindingMode.OneWay,
			Func<TSource, TParam, TDest> convert = null,
			Func<TDest, TParam, TSource> convertBack = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object targetNullValue = null,
			object fallbackValue = null
		) where TBindable : DependencyObject
		{
			var converter = new FuncConverter<TSource, TDest, TParam>(convert, convertBack);
			bindable.Bind(
				DefaultBindableProperties.GetFor(bindable),
				path, mode, converter, converterParameter, stringFormat, source, targetNullValue, fallbackValue
			);
			return bindable;
		}

		/// <summary>Bind to the <typeparamref name="TBindable"/>'s default Command and CommandParameter properties </summary>
		/// <param name="parameterPath">If null, no binding is created for the CommandParameter property</param>
		public static TBindable BindCommand<TBindable>(
			this TBindable bindable,

			string path = bindingContextPath,
			object source = null,
			string parameterPath = bindingContextPath,
			object parameterSource = null
		) where TBindable : DependencyObject
		{
			(var commandProperty, var parameterProperty) = DefaultBindableProperties.GetForCommand(bindable);

			bindable.SetBinding(commandProperty, new Binding(path: path, source: source));

			if (parameterPath != null)
				bindable.SetBinding(parameterProperty, new Binding(path: parameterPath, source: parameterSource));

			return bindable;
		}
		*/


	}
}
