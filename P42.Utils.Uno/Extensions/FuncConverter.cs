using System;
using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace P42.Utils.Uno;

/// <summary>
/// Converter using provided functions
/// </summary>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TDest"></typeparam>
/// <typeparam name="TParam"></typeparam>
public class FuncConverter<TSource, TDest, TParam> : IValueConverter
{
    private readonly Func<TSource?, TDest>? _convert;
    private readonly Func<TDest?, TSource>? _convertBack;

    private readonly Func<TSource?, TParam?, TDest>? _convertWithParam;
    private readonly Func<TDest?, TParam?, TSource>? _convertBackWithParam;

    private readonly Func<TSource?, TParam?, string?, TDest>? _convertWithParamAndLanguage;
    private readonly Func<TDest?, TParam?, string?, TSource>? _convertBackWithParamAndLanguage;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="convertWithParamAndLanguage"></param>
    /// <param name="convertBackWithParamAndCulture"></param>
    public FuncConverter(Func<TSource?, TParam?, string?, TDest>? convertWithParamAndLanguage = null, Func<TDest?, TParam?, string?, TSource>? convertBackWithParamAndCulture = null)
    { this._convertWithParamAndLanguage = convertWithParamAndLanguage; this._convertBackWithParamAndLanguage = convertBackWithParamAndCulture; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="convertWithParam"></param>
    /// <param name="convertBackWithParam"></param>
    public FuncConverter(Func<TSource?, TParam?, TDest>? convertWithParam = null, Func<TDest?, TParam?, TSource>? convertBackWithParam = null)
    { this._convertWithParam = convertWithParam; this._convertBackWithParam = convertBackWithParam; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="convert"></param>
    /// <param name="convertBack"></param>
    public FuncConverter(Func<TSource?, TDest>? convert = null, Func<TDest?, TSource>? convertBack = null)
    { this._convert = convert; this._convertBack = convertBack; }

    /// <summary>
    /// IValueConverter.Convert method
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    public object? Convert(object? value, Type targetType, object? parameter, string? language)
    {
        if (_convert != null)
            return _convert.Invoke(
                value != null ? (TSource)value : default);

        if (_convertWithParam != null)
            return _convertWithParam.Invoke(
                value != null ? (TSource)value : default,
                parameter != null ? (TParam)parameter : default);

        if (_convertWithParamAndLanguage != null)
            return _convertWithParamAndLanguage.Invoke(
                value != null ? (TSource)value : default,
                parameter != null ? (TParam)parameter : default,
                language);

        return default(TDest);
    }

    /// <summary>
    /// IValueConverter.ConvertBack method
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, string? language)
    {
        if (_convertBack != null)
            return _convertBack.Invoke(
                value != null ? (TDest)value : default);

        if (_convertBackWithParam != null)
            return _convertBackWithParam.Invoke(
                value != null ? (TDest)value : default,
                parameter != null ? (TParam)parameter : default);

        if (_convertBackWithParamAndLanguage != null)
            return _convertBackWithParamAndLanguage.Invoke(
                value != null ? (TDest)value : default,
                parameter != null ? (TParam)parameter : default,
                language);

        return default(TSource);
    }
}



/// <summary>
/// FuncConvert overload
/// </summary>
/// <param name="convert"></param>
/// <param name="convertBack"></param>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TDest"></typeparam>
public class FuncConverter<TSource, TDest>(Func<TSource?, TDest>? convert = null, Func<TDest?, TSource>? convertBack = null)
    : FuncConverter<TSource?, TDest?, object>(convert, convertBack);

/// <summary>
/// FuncConvert overload
/// </summary>
/// <param name="convert"></param>
/// <param name="convertBack"></param>
/// <typeparam name="TSource"></typeparam>
public class FuncConverter<TSource>(Func<TSource?, object>? convert = null, Func<object?, TSource>? convertBack = null)
    : FuncConverter<TSource?, object?, object>(convert, convertBack);

/// <summary>
/// FuncConvert overload
/// </summary>
/// <param name="convert"></param>
/// <param name="convertBack"></param>
public class FuncConverter(Func<object?, object>? convert = null, Func<object?, object>? convertBack = null)
    : FuncConverter<object?, object?, object>(convert, convertBack);

/// <summary>
/// ToString Conversion Function
/// </summary>
/// <param name="format"></param>
public class ToStringConverter(string format = "{0}")
    : FuncConverter<object?, string>(o => string.Format(CultureInfo.InvariantCulture, format, o));

/// <summary>
/// Not function converter
/// </summary>
public class NotConverter() : FuncConverter<bool, bool>(t => !t, t => !t)
{
    private static readonly Lazy<NotConverter> LazyInstance = new (() => new NotConverter());
    public static NotConverter Instance => LazyInstance.Value;
}
