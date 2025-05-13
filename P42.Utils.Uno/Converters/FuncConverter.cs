using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Data;
using P42.Serilog.QuickLog;

namespace P42.Utils.Uno;

/// <summary>
/// Convert values using Functions
/// </summary>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TDest"></typeparam>
/// <typeparam name="TParam"></typeparam>
public class FuncConverter<TSource, TDest, TParam> : IValueConverter
{
    private readonly Func<TSource?, TDest?>? _convert;
    private readonly Func<TDest?, TSource?>? _convertBack;

    private readonly Func<TSource?, TParam?, TDest?>? _convertWithParam;
    private readonly Func<TDest?, TParam?, TSource?>? _convertBackWithParam;

    private readonly Func<TSource?, TParam?, string, TDest?>? _convertWithParamAndLanguage;
    private readonly Func<TDest?, TParam?, string, TSource?>? _convertBackWithParamAndLanguage;

    private readonly string? _filePath;
    private readonly int _lineNumber;
    
    public FuncConverter(
        Func<TSource?, TParam?, string, TDest?>? convertWithParamAndLanguage = null, 
        Func<TDest?, TParam?, string, TSource?>? convertBackWithParamAndCulture = null,
        [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    { _convertWithParamAndLanguage = convertWithParamAndLanguage; _convertBackWithParamAndLanguage = convertBackWithParamAndCulture; _filePath = filePath; _lineNumber = lineNumber; }

    public FuncConverter(
        Func<TSource?, TParam?, TDest?>? convertWithParam = null, 
        Func<TDest?, TParam?, TSource?>? convertBackWithParam = null,
        [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    { _convertWithParam = convertWithParam; _convertBackWithParam = convertBackWithParam; _filePath = filePath; _lineNumber = lineNumber; }

    public FuncConverter(
        Func<TSource?, TDest?>? convert = null, 
        Func<TDest?, TSource?>? convertBack = null,
        [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    { _convert = convert; _convertBack = convertBack; _filePath = filePath; _lineNumber = lineNumber; }

    /// <summary>
    /// Convert value to target type
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    public object? Convert(object? value, Type targetType, object? parameter, string language)
    {
        try
        {
            if (_convert != null)
                return _convert.Invoke(
                    value != null 
                        ? (TSource)value 
                        : default);

            if (_convertWithParam != null)
                return _convertWithParam.Invoke(
                    value != null 
                        ? (TSource)value 
                        : default,
                    parameter != null 
                        ? (TParam)parameter 
                        : default);

            if (_convertWithParamAndLanguage != null)
                return _convertWithParamAndLanguage.Invoke(
                    value != null 
                        ? (TSource)value 
                        : default,
                    parameter != null 
                        ? (TParam)parameter 
                        : default,
                    language);
        }
        catch (Exception ex)
        {
            QLog.Error(ex, $"FuncConverter.Convert: {ex.Message} at {_filePath}:{_lineNumber}");
        }
        
        return null;
    }

    /// <summary>
    /// Convert value back to target type
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        try
        {
            if (_convertBack != null)
                return _convertBack.Invoke( 
                    value != null 
                        ? (TDest)value 
                        : default);

            if (_convertBackWithParam != null)
                return _convertBackWithParam.Invoke(
                    value != null 
                        ? (TDest)value 
                        : default,
                    parameter != null 
                        ? (TParam)parameter 
                        : default);

            if (_convertBackWithParamAndLanguage != null)
                return _convertBackWithParamAndLanguage.Invoke(
                    value != null 
                        ? (TDest)value 
                        : default,
                    parameter != null 
                        ? (TParam)parameter 
                        : default,
                    language);
        }
        catch (Exception ex)
        {
            QLog.Error(ex, $"FuncConverter.ConvertBack: {ex.Message} at {_filePath}:{_lineNumber}");
        }
        
        return null;
    }
}


/// <summary>
/// Convert values using Functions
/// </summary>
/// <param name="convert"></param>
/// <param name="convertBack"></param>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TDest"></typeparam>
public class FuncConverter<TSource, TDest>(
    Func<TSource?, TDest?>? convert = null,
    Func<TDest?, TSource?>? convertBack = null,
    [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    : FuncConverter<TSource, TDest, object>(convert, convertBack, filePath, lineNumber);

/// <summary>
/// Convert values using Functions
/// </summary>
/// <param name="convert"></param>
/// <param name="convertBack"></param>
/// <typeparam name="TSource"></typeparam>
public class FuncConverter<TSource>(
    Func<TSource?, object?>? convert = null,
    Func<object?, TSource?>? convertBack = null,
    [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    : FuncConverter<TSource, object, object>(convert, convertBack, filePath, lineNumber);

/// <summary>
/// Convert values using Functions
/// </summary>
/// <param name="convert"></param>
/// <param name="convertBack"></param>
public class FuncConverter(
    Func<object?, object?>? convert = null, 
    Func<object?, object?>? convertBack = null,
    [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    : FuncConverter<object, object, object>(convert, convertBack, filePath, lineNumber);

/// <summary>
/// Convert to string
/// </summary>
/// <param name="format"></param>
public class ToStringConverter(string format = "{0}", [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    : FuncConverter<object, string>(o => string.Format(CultureInfo.InvariantCulture, format, o), s => s, filePath, lineNumber);

/// <summary>
/// Simple NOT converter
/// </summary>
public class NotConverter([CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1) : FuncConverter<bool, bool>(t => !t, t => !t, filePath, lineNumber)
{
    private static NotConverter? _instance;
    public static NotConverter Instance => _instance ??= new NotConverter();
}
