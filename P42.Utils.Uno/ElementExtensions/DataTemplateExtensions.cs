using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Markup;
using P42.Serilog.QuickLog;

namespace P42.Utils.Uno;

public static class DataTemplateExtensions
{
    /// <param name="templateType"></param>
    extension(Type templateType)
    {
        /// <summary>
        /// Convert a type into a DataTemplate Xaml string
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string AsDataTemplateXaml()
        {
            if (templateType == null || !typeof(FrameworkElement).IsAssignableFrom(templateType))
                throw new Exception($"Cannot convert type [{templateType}] into DataTemplate");

            var markup = $"<DataTemplate \n\t xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" \n\t xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" \n\t xmlns:local=\"using:{templateType.Namespace}\"> \n\t\t<local:{templateType.Name} /> \n</DataTemplate>";
            //if (dataType.Namespace == typeof(Type).Namespace)
            //    markup = $"<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:tlocal=\"using:{templateType.Namespace}\" xmlns:system=\"using:System\" x:DataType=\"system:{dataType.Name}\"><tlocal:{templateType.Name} /></DataTemplate>";
            //else
            //    markup = $"<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" \n\t xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" \n\t xmlns:tlocal=\"using:{templateType.Namespace}\" \n\t xmlns:dlocal=\"using:{dataType.Namespace}\" \n\t x:DataType=\"dlocal:{dataType.Name}\"> \n\t\t<tlocal:{templateType.Name} /> \n</DataTemplate>";
            // System.Diagnostics.Debug.WriteLine("BcGroupView.GenerateDataTemplate: markup: " + markup);
            //template.
            return markup;
        }

        /// <summary>
        /// Convert type to a DataTemplate
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataTemplate? AsDataTemplate([CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            try
            {
                var markup = templateType.AsDataTemplateXaml();
                return (DataTemplate)XamlReader.Load(markup);
            }
            catch (Exception e)
            {
                QLog.Error(e, $"{filePath}:{lineNumber}");
            }
            return null;
        }
    }
}
