// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

namespace P42.Utils;

internal static class ThrowHelper
{
    internal static void ThrowArgumentNullException(ExceptionArgument argument, Type type, [CallerMemberName] string? method = null) => throw new ArgumentNullException(
        $"{type}.{method}: {GetArgumentString(argument)}");

    internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument, Type type, [CallerMemberName] string? method = null) => throw new ArgumentOutOfRangeException(
        $"{type}.{method}: {GetArgumentString(argument)}");

    internal static void ThrowMoreThanOneElementException(Type type, [CallerMemberName] string? method = null) => throw new InvalidOperationException(
        $"{type}.{method}: More than one element.  ");

    internal static void ThrowMoreThanOneMatchException(Type type, [CallerMemberName] string? method = null) => throw new InvalidOperationException(
        $"{type}.{method}: More than one match.  ");

    internal static void ThrowNoElementsException(Type type, [CallerMemberName] string? method = null) => throw new InvalidOperationException(
        $"{type}.{method}: No elements.  ");

    internal static void ThrowNoMatchException(Type type, [CallerMemberName] string? method = null) => throw new InvalidOperationException(
        $"{type}.{method}: No match.  ");

    internal static void ThrowNotSupportedException(Type type, [CallerMemberName] string? method = null) => throw new NotSupportedException(
        $"{type}.{method}: Not supported.  ");

    private static string GetArgumentString(ExceptionArgument argument)
    {
        return argument switch
        {
            ExceptionArgument.CollectionSelector => nameof(ExceptionArgument.CollectionSelector),
            ExceptionArgument.Count => nameof(ExceptionArgument.Count),
            ExceptionArgument.ElementSelector => nameof(ExceptionArgument.ElementSelector),
            ExceptionArgument.Enumerable => nameof(ExceptionArgument.Enumerable),
            ExceptionArgument.First => nameof(ExceptionArgument.First),
            ExceptionArgument.Func => nameof(ExceptionArgument.Func),
            ExceptionArgument.Index => nameof(ExceptionArgument.Index),
            ExceptionArgument.Inner => nameof(ExceptionArgument.Inner),
            ExceptionArgument.InnerKeySelector => nameof(ExceptionArgument.InnerKeySelector),
            ExceptionArgument.KeySelector => nameof(ExceptionArgument.KeySelector),
            ExceptionArgument.Outer => nameof(ExceptionArgument.Outer),
            ExceptionArgument.OuterKeySelector => nameof(ExceptionArgument.OuterKeySelector),
            ExceptionArgument.Predicate => nameof(ExceptionArgument.Predicate),
            ExceptionArgument.ResultSelector => nameof(ExceptionArgument.ResultSelector),
            ExceptionArgument.Second => nameof(ExceptionArgument.Second),
            ExceptionArgument.Selector => nameof(ExceptionArgument.Selector),
            ExceptionArgument.Source => nameof(ExceptionArgument.Source),
            _ => string.Empty
        };
    }
}

internal enum ExceptionArgument
{
    CollectionSelector,
    Count,
    ElementSelector,
    Enumerable,
    First,
    Func,
    Index,
    Inner,
    InnerKeySelector,
    KeySelector,
    Outer,
    OuterKeySelector,
    Predicate,
    ResultSelector,
    Second,
    Selector,
    Source,
}
