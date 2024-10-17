using System;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace P42.Utils;

/// <summary>
/// Thread safe observable collection
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObservableConcurrentCollection<T> : ObservableCollection<T>, IEnumerable
{
    /// <summary>
    /// Access lock
    /// </summary>
    protected readonly object Lock = new();

    /// <summary>
    /// Clear collection
    /// </summary>
    protected override void ClearItems()
    {
        lock (Lock)
        {
            while (Count > 0)
                RemoveAt(Count - 1);
            // Change made to address Xamarin.Forms.iOS ListView crash:
            // NSInternalInconsistencyException Reason: Invalid update: invalid number of rows in section X.
            // The number of rows contained in an existing section after the update (i) must be equal to the number
            // of rows contained in that section before the update (j), plus or minus the number of rows inserted or
            // deleted from that section (k inserted, l deleted) and plus or minus the number of rows moved into or
            // out of that section (0 moved in, 0 moved out).
        }
    }

    /// <summary>
    /// Insert item at index
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    protected override void InsertItem(int index, T item)
    {
        lock (Lock)
        {
            if (index > Count)
                index = Count;
            base.InsertItem(index, item);
        }
    }

    /// <summary>
    /// Remove item at index
    /// </summary>
    /// <param name="index"></param>
    protected override void RemoveItem(int index)
    {
        if (index >= Count)
            return;
        lock (Lock)
            base.RemoveItem(index);
    }

    /// <summary>
    /// Set item at index
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    protected override void SetItem(int index, T item)
    {
        lock (Lock)
        {
            if (index > Count)
                index = Count;
            base.SetItem(index, item);
        }
    }

    /// <summary>
    /// output as an array
    /// </summary>
    /// <returns></returns>
    public T[] ToArray()
    {
        lock (Lock)
        {
            return [..this];
        }
    }

    /// <summary>
    /// output as a List
    /// </summary>
    /// <returns></returns>
    public List<T> ToList()
    {
        lock (Lock)
        {
            return [..this];
        }
    }

    /// <summary>
    /// editing range flag
    /// </summary>
    protected bool EditingRange;

    /// <summary>
    /// AddRange
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual NotifyCollectionChangedEventArgs? AddRange(IEnumerable<T>? range)
    {
        if (range is null)
            return null;
        if ((range as T[] ?? range.ToArray()) is not { } rangeArray)
            return null;
        if (rangeArray.Length == 0)
            return null;

        var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Array.AsReadOnly(rangeArray));
        lock (Lock)
        {
            EditingRange = true;
            foreach (var item in rangeArray)
                Add(item);
            EditingRange = false;
        }
        OnCollectionChanged(args);
        return args;
    }


    /// <summary>
    /// Remove range
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual NotifyCollectionChangedEventArgs? RemoveRange(IEnumerable<T>? range)
    {
        if (range is null)
            return null;
        if ((range as T[] ?? range.ToArray()) is not { } rangeArray)
            return null;
        if (rangeArray.Length == 0)
            return null;

        var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, Array.AsReadOnly(rangeArray));
        lock (Lock)
        {
            EditingRange = true;
            foreach (var item in rangeArray)
                Remove(item);
            EditingRange = false;
        }
        OnCollectionChanged(args);
        return args;
    }


    /// <summary>
    /// Called when item(s) added or removed from collection
    /// </summary>
    /// <param name="e"></param>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (!EditingRange)
            base.OnCollectionChanged(e);
    }

    /// <summary>
    /// Get generic IEnumerator
    /// </summary>
    /// <returns></returns>
    public new IEnumerator<T> GetEnumerator()
        => ToList().GetEnumerator();

    /// <summary>
    /// returns index of item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public new int IndexOf(T item)
    {
        lock (Lock)
            return base.IndexOf(item);
    }

    /// <summary>
    /// Get IEnumerable
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator()
        => ToArray().GetEnumerator();

    #region Linq

    #region Aggregate
    /// <summary>
    /// Aggregate value via function
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public T Aggregate(Func<T?, T, T> func)
        => ToArray().Aggregate(func);

    /// <summary>
    /// Aggregate value via function
    /// </summary>
    /// <typeparam name="TAccumulate"></typeparam>
    /// <param name="seed"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func)
        => ToArray().Aggregate(seed, func);
    

    /// <summary>
    /// Aggregate value via function and selector
    /// </summary>
    /// <typeparam name="TAccumulate"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="seed"></param>
    /// <param name="func"></param>
    /// <param name="resultSelector"></param>
    /// <returns></returns>
    public TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        => resultSelector(ToArray().Aggregate(seed, func));
    
    #endregion


    #region Any/All
    /// <summary>
    /// Any test
    /// </summary>
    /// <returns></returns>
    public bool Any()
        => Count > 0;

    /// <summary>
    /// Any, with predicate, test
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Any(Func<T, bool> predicate)
        => ToArray().Any(predicate);
    

    /// <summary>
    /// All, with predicate, test
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool All(Func<T, bool> predicate)
        => ToArray().All(predicate);
    
    #endregion


    #region Append / Prepend
    /// <summary>
    /// Append
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public IEnumerable<T> Append(T element)
    {
        var result = ToList();
        result.Add(element);
        return result;
    }

    /// <summary>
    /// Prepend
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public IEnumerable<T> Prepend(T element)
    {
        var result = ToList();
        result.Insert(0, element);
        return result;
    }
    #endregion


    #region Average
    /// <summary>
    /// Average
    /// </summary>
    /// <returns></returns>
    public double? Average()
    {
        var type = typeof(T);
        if (type == typeof(int))
            return ToArray().Cast<int>().Average();
        if (type == typeof(int?))
            return ToArray().Cast<int?>().Average();
        if (type == typeof(long))
            return ToArray().Cast<long>().Average();
        if (type == typeof(long?))
            return ToArray().Cast<long?>().Average();
        if (type == typeof(float))
            return ToArray().Cast<float>().Average();
        if (type == typeof(float?))
            return ToArray().Cast<float?>().Average();
        if (type == typeof(double))
            return ToArray().Cast<double>().Average();
        if (type == typeof(double?))
            return ToArray().Cast<double?>().Average();
        if (type == typeof(decimal))
            return (double?)ToArray().Cast<decimal>().Average();
        if (type == typeof(decimal?))
            return (double?)ToArray().Cast<decimal?>().Average();
        return null;
    }

    /// <summary>
    /// Average, with selector
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public double Average(Func<T, int> selector)
        => ToArray().Average(selector);

    /// <summary>
    /// Average, with selector
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public double? Average(Func<T, int?> selector)
        => ToArray().Average(selector);

    /// <summary>
    /// Average, with selector
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public double Average(Func<T, long> selector)
        => ToArray().Average(selector);

    /// <summary>
    /// Average, with selector
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public double? Average(Func<T, long?> selector)
        => ToArray().Average(selector);

    /// <summary>
    /// Average, with selector
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public double Average(Func<T, float> selector)
        => ToArray().Average(selector);

    /// <summary>
    /// Average, with selector
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public double? Average(Func<T, float?> selector)
        => ToArray().Average(selector);

    /// <summary>
    /// Average, with selector
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public double Average(Func<T, double> selector)
        => ToArray().Average(selector);

    /// <summary>
    /// Average, with selector
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public double? Average(Func<T, double?> selector)
        => ToArray().Average(selector);

    /// <summary>
    /// Average, with selector
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public decimal Average(Func<T, decimal> selector)
        => ToArray().Average(selector);

    /// <summary>
    /// Average, with selector
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public decimal? Average(Func<T, decimal?> selector)
        => ToArray().Average(selector);
    #endregion


    #region Cast
    /// <summary>
    /// Cast
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public IEnumerable<TResult> OfType<TResult>()
    {
        foreach (var item in ToArray())
            if (item is TResult result)
                yield return result;
    }

    /// <summary>
    /// Cast
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public IEnumerable<TResult> Cast<TResult>()
        => OfType<TResult>();
    #endregion


    #region Concat
    /// <summary>
    /// Concat
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public IEnumerable<T> Concat(IEnumerable<T> other)
        => ToArray().Concat(other);
    
    #endregion


    #region Contains
    /// <summary>
    /// Contains
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public new bool Contains(T value)
    {
        lock (Lock)
            return base.Contains(value);
    }

    /// <summary>
    /// Contains, with comparer
    /// </summary>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public bool Contains(T value, IEqualityComparer<T> comparer)
        => AsEnumerable().Contains(value, comparer);
    
    #endregion


    #region DefaultIfEmpty
    /// <summary>
    /// Default if empty
    /// </summary>
    /// <returns>Enumerator</returns>
    public IEnumerable<T?> DefaultIfEmpty() 
        => ToArray().DefaultIfEmpty();

    /// <summary>
    /// Default, with alternative default, if empty
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public IEnumerable<T> DefaultIfEmpty(T defaultValue)
        => ToArray().DefaultIfEmpty(defaultValue);
    #endregion


    #region Distinct
    /// <summary>
    /// Returns distinct elements from a sequence.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<T> Distinct() 
        => ToArray().Distinct();

    /// <summary>
    /// Returns distinct, with comparer, elements from a sequence.
    /// </summary>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public IEnumerable<T> Distinct(IEqualityComparer<T> comparer)
        => ToArray().Distinct(comparer);
    #endregion


    #region Element At
    /// <summary>
    /// Element at index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public T ElementAt(int index)
        => this[index];

    /// <summary>
    /// Element at index, or default
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public T? ElementAtOrDefault(int index)
    {
        if (index < 0)
            return default;

        return index < Count ? this[index] : default;
    }
    #endregion


    #region Enumerable
    /// <summary>
    /// As enumerable
    /// </summary>
    /// <returns></returns>
    public IEnumerable<T> AsEnumerable() 
        => ToArray();
    #endregion


    #region Except
    /// <summary>
    /// Produces the set difference of two sequences.
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public IEnumerable<T> Except(IEnumerable<T> second)
        => ToArray().Except(second);

    /// <summary>
    /// Produces the set difference, with comparer, of two sequences.
    /// </summary>
    /// <param name="second"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public IEnumerable<T> Except(IEnumerable<T> second, IEqualityComparer<T> comparer)
        => ToArray().Except(second, comparer);
    #endregion


    #region First
    /// <summary>
    /// First item
    /// </summary>
    /// <returns></returns>
    public T First()
        => ToArray().First();

    /// <summary>
    /// First item meeting predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public T First(Func<T, bool> predicate)
        => ToArray().First(predicate);

    /// <summary>
    /// First item, or default
    /// </summary>
    /// <returns></returns>
    public T? FirstOrDefault()
        => ToArray().FirstOrDefault();

    /// <summary>
    /// First item, or default, meeting predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public T? FirstOrDefault(Func<T, bool> predicate)
        => ToArray().FirstOrDefault(predicate);
    #endregion


    #region Group Join
    /// <summary>
    /// Correlates the elements of two sequences based on matching keys
    /// </summary>
    /// <typeparam name="TInner"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="inner"></param>
    /// <param name="outerKeySelector"></param>
    /// <param name="innerKeySelector"></param>
    /// <param name="resultSelector"></param>
    /// <returns></returns>
    public IEnumerable<TResult> GroupJoin<TInner, TKey, TResult>(IEnumerable<TInner> inner, Func<T, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<T, IEnumerable<TInner>, TResult> resultSelector)
        => ToArray().GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector);

    /// <summary>
    /// Correlates the elements of two sequences based on matching keys
    /// </summary>
    /// <typeparam name="TInner"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="inner"></param>
    /// <param name="outerKeySelector"></param>
    /// <param name="innerKeySelector"></param>
    /// <param name="resultSelector"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public IEnumerable<TResult> GroupJoin<TInner, TKey, TResult>(IEnumerable<TInner> inner, Func<T, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<T, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        => ToArray().GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
    #endregion


    #region Grouping
    /// <summary>
    /// Groups the elements of a sequence
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="keySelector"></param>
    /// <returns></returns>
    public IEnumerable<IGrouping<TKey, T>> GroupBy<TKey>(Func<T, TKey> keySelector) =>
        ToArray().GroupBy(keySelector);

    /// <summary>
    /// Groups the elements of a sequence
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="keySelector"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public IEnumerable<IGrouping<TKey, T>> GroupBy<TKey>(Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer) =>
        ToArray().GroupBy(keySelector, comparer);

    /// <summary>
    /// Groups the elements of a sequence
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <param name="keySelector"></param>
    /// <param name="elementSelector"></param>
    /// <returns></returns>
    public IEnumerable<IGrouping<TKey, TElement>> GroupBy<TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector) =>
        ToArray().GroupBy(keySelector, elementSelector);

    /// <summary>
    /// Groups the elements of a sequence
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <param name="keySelector"></param>
    /// <param name="elementSelector"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public IEnumerable<IGrouping<TKey, TElement>> GroupBy<TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector, IEqualityComparer<TKey> comparer) =>
        ToArray().GroupBy(keySelector, elementSelector, comparer);

    /// <summary>
    /// Groups the elements of a sequence
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="keySelector"></param>
    /// <param name="resultSelector"></param>
    /// <returns></returns>
    public IEnumerable<TResult> GroupBy<TKey, TResult>(Func<T, TKey> keySelector, Func<TKey, IEnumerable<T>, TResult> resultSelector) =>
        ToArray().GroupBy(keySelector, resultSelector, null);

    /// <summary>
    /// Groups the elements of a sequence
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="keySelector"></param>
    /// <param name="elementSelector"></param>
    /// <param name="resultSelector"></param>
    /// <returns></returns>
    public IEnumerable<TResult> GroupBy<TKey, TElement, TResult>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector) =>
        ToArray().GroupBy(keySelector, elementSelector, resultSelector, null);

    /// <summary>
    /// Groups the elements of a sequence
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="keySelector"></param>
    /// <param name="resultSelector"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public IEnumerable<TResult> GroupBy<TKey, TResult>(Func<T, TKey> keySelector, Func<TKey, IEnumerable<T>, TResult> resultSelector, IEqualityComparer<TKey> comparer) =>
        ToArray().GroupBy(keySelector, resultSelector, comparer);

    /// <summary>
    /// Groups the elements of a sequence
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="keySelector"></param>
    /// <param name="elementSelector"></param>
    /// <param name="resultSelector"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public IEnumerable<TResult> GroupBy<TKey, TElement, TResult>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer) =>
        ToArray().GroupBy(keySelector, elementSelector, resultSelector, comparer);
    #endregion


    #region Intersect
    /// <summary>
    /// Produces the set intersection of two sequences
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public IEnumerable<T> Intersect(IEnumerable<T> second)
        => ToArray().Intersect(second);

    /// <summary>
    /// Produces the set intersection of two sequences
    /// </summary>
    /// <param name="second"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public IEnumerable<T> Intersect(IEnumerable<T> second, IEqualityComparer<T> comparer)
        => ToArray().Intersect(second, comparer);
    #endregion


    #region Join
    /// <summary>
    /// Correlates the elements of two sequences based on matching keys
    /// </summary>
    /// <typeparam name="TInner"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="inner"></param>
    /// <param name="outerKeySelector"></param>
    /// <param name="innerKeySelector"></param>
    /// <param name="resultSelector"></param>
    /// <returns></returns>
    public IEnumerable<TResult> Join<TInner, TKey, TResult>(IEnumerable<TInner> inner, Func<T, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<T, TInner, TResult> resultSelector)
        => ToArray().Join(inner, outerKeySelector, innerKeySelector, resultSelector);

    /// <summary>
    /// Correlates the elements of two sequences based on matching keys
    /// </summary>
    /// <typeparam name="TInner"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="inner"></param>
    /// <param name="outerKeySelector"></param>
    /// <param name="innerKeySelector"></param>
    /// <param name="resultSelector"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public IEnumerable<TResult> Join<TInner, TKey, TResult>(IEnumerable<TInner> inner, Func<T, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<T, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        => ToArray().Join(inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
    #endregion


    #region Last
    /// <summary>
    /// Returns the last element of a sequence
    /// </summary>
    /// <returns></returns>
    public T Last()
        => ToArray().Last();

    /// <summary>
    /// Returns the last element of a sequence
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public T Last(Func<T, bool> predicate)
        => ToArray().Last(predicate);
    #endregion


    #region Lookup
    /// <summary>
    /// Creates a generic Lookup&lt;TKey,TElement&gt; from an IEnumerable&lt;T&gt;
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="keySelector"></param>
    /// <returns></returns>
    public ILookup<TKey, T> ToLookup<TKey>(Func<T, TKey> keySelector)
        => ToArray().ToLookup(keySelector);

    /// <summary>
    /// Creates a generic Lookup&lt;TKey,TElement&gt; from an IEnumerable&lt;T&gt;
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="keySelector"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public ILookup<TKey, T> ToLookup<TKey>(Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer)
        => ToArray().ToLookup(keySelector, comparer);

    /// <summary>
    /// Creates a generic Lookup&lt;TKey,TElement&gt; from an IEnumerable&lt;T&gt;
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <param name="keySelector"></param>
    /// <param name="elementSelector"></param>
    /// <returns></returns>
    public ILookup<TKey, TElement> ToLookup<TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector)
        => ToArray().ToLookup(keySelector, elementSelector);

    /// <summary>
    /// Creates a generic Lookup&lt;TKey,TElement&gt; from an IEnumerable&lt;T&gt;
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <param name="keySelector"></param>
    /// <param name="elementSelector"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public ILookup<TKey, TElement> ToLookup<TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        => ToArray().ToLookup(keySelector, elementSelector, comparer);
    #endregion


    #region Max
    /// <summary>
    /// Returns the maximum value in a sequence of values
    /// </summary>
    /// <returns></returns>
    public double? Max()
    {
        var type = typeof(T);
        if (type == typeof(int))
            return ToArray().Cast<int>().Max();
        if (type == typeof(int?))
            return ToArray().Cast<int?>().Max();
        if (type == typeof(long))
            return ToArray().Cast<long>().Max();
        if (type == typeof(long?))
            return ToArray().Cast<long?>().Max();
        if (type == typeof(float))
            return ToArray().Cast<float>().Max();
        if (type == typeof(float?))
            return ToArray().Cast<float?>().Max();
        if (type == typeof(double))
            return ToArray().Cast<double>().Max();
        if (type == typeof(double?))
            return ToArray().Cast<double?>().Max();
        if (type == typeof(decimal))
            return (double?)ToArray().Cast<decimal>().Max();
        if (type == typeof(decimal?))
            return (double?)ToArray().Cast<decimal?>().Max();
        return null;
    }

    /// <summary>
    /// Returns the maximum value in a sequence of values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public int Max(Func<T, int> selector)
        => ToArray().Max(selector);

    /// <summary>
    /// Returns the maximum value in a sequence of values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public int? Max(Func<T, int?> selector)
        => ToArray().Max(selector);

    /// <summary>
    /// Returns the maximum value in a sequence of values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public long Max(Func<T, long> selector)
        => ToArray().Max(selector);

    /// <summary>
    /// Returns the maximum value in a sequence of values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public long? Max(Func<T, long?> selector)
        => ToArray().Max(selector);

    /// <summary>
    /// Returns the maximum value in a sequence of values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public float Max(Func<T, float> selector)
        => ToArray().Max(selector);

    /// <summary>
    /// Returns the maximum value in a sequence of values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public float? Max(Func<T, float?> selector)
        => ToArray().Max(selector);

    /// <summary>
    /// Returns the maximum value in a sequence of values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public double Max(Func<T, double> selector)
        => ToArray().Max(selector);

    /// <summary>
    /// Returns the maximum value in a sequence of values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public double? Max(Func<T, double?> selector)
        => ToArray().Max(selector);

    /// <summary>
    /// Returns the maximum value in a sequence of values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public decimal Max(Func<T, decimal> selector)
        => ToArray().Max(selector);

    /// <summary>
    /// Returns the maximum value in a sequence of values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public decimal? Max(Func<T, decimal?> selector)
        => ToArray().Max(selector);
    #endregion


    #region Min
    /// <summary>
    /// Returns the minimum value in a sequence of values
    /// </summary>
    /// <returns></returns>
    public double? Min()
    {
        var type = typeof(T);
        if (type == typeof(int))
            return ToArray().Cast<int>().Min();
        if (type == typeof(int?))
            return ToArray().Cast<int?>().Min();
        if (type == typeof(long))
            return ToArray().Cast<long>().Min();
        if (type == typeof(long?))
            return ToArray().Cast<long?>().Min();
        if (type == typeof(float))
            return ToArray().Cast<float>().Min();
        if (type == typeof(float?))
            return ToArray().Cast<float?>().Min();
        if (type == typeof(double))
            return ToArray().Cast<double>().Min();
        if (type == typeof(double?))
            return ToArray().Cast<double?>().Min();
        if (type == typeof(decimal))
            return (double?)ToArray().Cast<decimal>().Min();
        if (type == typeof(decimal?))
            return (double?)ToArray().Cast<decimal?>().Min();
        return null;
    }
        
    /// <summary>
    /// Returns the minimum value in a sequence of values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public int Min(Func<T, int> selector)
        => ToArray().Min(selector);

    /// <summary>
    /// Returns the minimum value in a sequence of values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public int? Min(Func<T, int?> selector)
        => ToArray().Min(selector);

    /// <summary>
    /// Returns the minimum value in a sequence of values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public long Min(Func<T, long> selector)
        => ToArray().Min(selector);

    /// <summary>
    /// Returns the minimum value in a sequence of values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public long? Min(Func<T, long?> selector)
        => ToArray().Min(selector);

    /// <summary>
    /// Returns the minimum value in a sequence of values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public float Min(Func<T, float> selector)
        => ToArray().Min(selector);

    /// <summary>
    /// Returns the minimum value in a sequence of values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public float? Min(Func<T, float?> selector)
        => ToArray().Min(selector);

    /// <summary>
    /// Returns the minimum value in a sequence of values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public double Min(Func<T, double> selector)
        => ToArray().Min(selector);

    /// <summary>
    /// Returns the minimum value in a sequence of values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public double? Min(Func<T, double?> selector)
        => ToArray().Min(selector);

    /// <summary>
    /// Returns the minimum value in a sequence of values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public decimal Min(Func<T, decimal> selector)
        => ToArray().Min(selector);

    /// <summary>
    /// Returns the minimum value in a sequence of values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public decimal? Min(Func<T, decimal?> selector)
        => ToArray().Min(selector);
    #endregion


    #region OrderBy
    /// <summary>
    /// Sorts the elements of a sequence in ascending order
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="keySelector"></param>
    /// <returns></returns>
    public IOrderedEnumerable<T> OrderBy<TKey>(Func<T, TKey> keySelector)
        => ToArray().OrderBy(keySelector);

    /// <summary>
    /// Sorts the elements of a sequence in ascending order
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="keySelector"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public IOrderedEnumerable<T> OrderBy<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
        => ToArray().OrderBy(keySelector, comparer);

    /// <summary>
    /// Sorts the elements of a sequence in ascending order
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="keySelector"></param>
    /// <returns></returns>
    public IOrderedEnumerable<T> OrderByDescending<TKey>(Func<T, TKey> keySelector)
        => ToArray().OrderByDescending(keySelector);

    /// <summary>
    /// Sorts the elements of a sequence in ascending order
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="keySelector"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public IOrderedEnumerable<T> OrderByDescending<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
        => ToArray().OrderByDescending(keySelector, comparer);
    #endregion


    #region Reverse
    /// <summary>
    /// Inverts the order of the elements in a sequence
    /// </summary>
    /// <returns></returns>
    public IEnumerable<T> Reverse()
        => ToArray().Reverse();
    #endregion


    #region Select
    /// <summary>
    /// Projects each element of a sequence into a new form
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="selector"></param>
    /// <returns></returns>
    public IEnumerable<TResult> Select<TResult>(Func<T, TResult> selector)
        => ToArray().Select(selector);

    /// <summary>
    /// Projects each element of a sequence into a new form
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="selector"></param>
    /// <returns></returns>
    public IEnumerable<TResult> Select<TResult>(Func<T, int, TResult> selector)
        => ToArray().Select(selector);
    #endregion


    #region SelectMany
    /// <summary>
    /// Projects each element of a sequence to an IEnumerable&lt;T&gt; and flattens the resulting sequences into one sequence
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="selector"></param>
    /// <returns></returns>
    public IEnumerable<TResult> SelectMany<TResult>(Func<T, IEnumerable<TResult>> selector)
        => ToArray().SelectMany(selector);

    /// <summary>
    /// Projects each element of a sequence to an IEnumerable&lt;T&gt; and flattens the resulting sequences into one sequence
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="selector"></param>
    /// <returns></returns>
    public IEnumerable<TResult> SelectMany<TResult>(Func<T, int, IEnumerable<TResult>> selector)
        => ToArray().SelectMany(selector);

    /// <summary>
    /// Projects each element of a sequence to an IEnumerable&lt;T&gt; and flattens the resulting sequences into one sequence
    /// </summary>
    /// <typeparam name="TCollection"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="collectionSelector"></param>
    /// <param name="resultSelector"></param>
    /// <returns></returns>
    public IEnumerable<TResult> SelectMany<TCollection, TResult>(Func<T, int, IEnumerable<TCollection>> collectionSelector, Func<T, TCollection, TResult> resultSelector)
        => ToArray().SelectMany(collectionSelector, resultSelector);

    /// <summary>
    /// Projects each element of a sequence to an IEnumerable&lt;T&gt; and flattens the resulting sequences into one sequence
    /// </summary>
    /// <typeparam name="TCollection"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="collectionSelector"></param>
    /// <param name="resultSelector"></param>
    /// <returns></returns>
    public IEnumerable<TResult> SelectMany<TCollection, TResult>(Func<T, IEnumerable<TCollection>> collectionSelector, Func<T, TCollection, TResult> resultSelector)
        => ToArray().SelectMany(collectionSelector, resultSelector);
    #endregion


    #region Sequence Equal
    /// <summary>
    /// Determines whether two sequences are equal according to an equality comparer
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public bool SequenceEqual(IEnumerable<T> second)
        => ToArray().SequenceEqual(second);

    /// <summary>
    /// Determines whether two sequences are equal according to an equality comparer
    /// </summary>
    /// <param name="second"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public bool SequenceEqual(IEnumerable<T> second, IEqualityComparer<T> comparer)
        => ToArray().SequenceEqual(second, comparer);
    #endregion


    #region Single
    /// <summary>
    /// Returns a single, specific element of a sequence
    /// </summary>
    /// <returns></returns>
    public T Single()
        => ToArray().Single();

    /// <summary>
    /// Returns a single, specific element of a sequence
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public T Single(Func<T, bool> predicate)
        => ToArray().Single(predicate);

    /// <summary>
    /// Returns a single, specific element of a sequence, or a default value if that element is not found.
    /// </summary>
    /// <returns></returns>
    public T? SingleOrDefault()
        => ToArray().SingleOrDefault();

    /// <summary>
    /// Returns a single, specific element of a sequence, or a default value if that element is not found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public T? SingleOrDefault(Func<T, bool> predicate)
        => ToArray().SingleOrDefault(predicate);
    #endregion


    #region Skip
    /// <summary>
    /// Bypasses a specified number of elements in a sequence and then returns the remaining elements
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public IEnumerable<T> Skip(int count)
        => ToArray().Skip(count);

    /// <summary>
    /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IEnumerable<T> SkipWhile(Func<T, bool> predicate)
        => ToArray().SkipWhile(predicate);

    /// <summary>
    /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IEnumerable<T> SkipWhile(Func<T, int, bool> predicate)
        => ToArray().SkipWhile(predicate);

    //public IEnumerable<T> SkipLast(int count)
    //    => ToArray().SkipLast();
    #endregion


    #region Sum
    /// <summary>
    /// Computes the sum of a sequence of numeric values
    /// </summary>
    /// <returns></returns>
    public double? Sum()
    {
        var type = typeof(T);
        if (type == typeof(int))
            return ToArray().Cast<int>().Sum();
        if (type == typeof(int?))
            return ToArray().Cast<int?>().Sum();
        if (type == typeof(long))
            return ToArray().Cast<long>().Sum();
        if (type == typeof(long?))
            return ToArray().Cast<long?>().Sum();
        if (type == typeof(float))
            return ToArray().Cast<float>().Sum();
        if (type == typeof(float?))
            return ToArray().Cast<float?>().Sum();
        if (type == typeof(double))
            return ToArray().Cast<double>().Sum();
        if (type == typeof(double?))
            return ToArray().Cast<double?>().Sum();
        if (type == typeof(decimal))
            return (double?)ToArray().Cast<decimal>().Sum();
        if (type == typeof(decimal?))
            return (double?)ToArray().Cast<decimal?>().Sum();
        return null;
    }

    /// <summary>
    /// Computes the sum of a sequence of numeric values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public int Sum(Func<T, int> selector)
        => ToArray().Sum(selector);

    /// <summary>
    /// Computes the sum of a sequence of numeric values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public int? Sum(Func<T, int?> selector)
        => ToArray().Sum(selector);

    /// <summary>
    /// Computes the sum of a sequence of numeric values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public long Sum(Func<T, long> selector)
        => ToArray().Sum(selector);

    /// <summary>
    /// Computes the sum of a sequence of numeric values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public long? Sum(Func<T, long?> selector)
        => ToArray().Sum(selector);

    /// <summary>
    /// Computes the sum of a sequence of numeric values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public float Sum(Func<T, float> selector)
        => ToArray().Sum(selector);

    /// <summary>
    /// Computes the sum of a sequence of numeric values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public float? Sum(Func<T, float?> selector)
        => ToArray().Sum(selector);

    /// <summary>
    /// Computes the sum of a sequence of numeric values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public double Sum(Func<T, double> selector)
        => ToArray().Sum(selector);

    /// <summary>
    /// Computes the sum of a sequence of numeric values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public double? Sum(Func<T, double?> selector)
        => ToArray().Sum(selector);

    /// <summary>
    /// Computes the sum of a sequence of numeric values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public decimal Sum(Func<T, decimal> selector)
        => ToArray().Sum(selector);

    /// <summary>
    /// Computes the sum of a sequence of numeric values
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public decimal? Sum(Func<T, decimal?> selector)
        => ToArray().Sum(selector);
    #endregion


    #region Take
    /// <summary>
    /// Returns a specified number of contiguous elements from the start of a sequence
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public IEnumerable<T> Take(int count)
        => ToArray().Take(count);

    /// <summary>
    /// Returns elements from a sequence as long as a specified condition is true, and then skips the remaining elements.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IEnumerable<T> TakeWhile(Func<T, bool> predicate)
        => ToArray().TakeWhile(predicate);

    /// <summary>
    /// Returns elements from a sequence as long as a specified condition is true, and then skips the remaining elements.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IEnumerable<T> TakeWhile(Func<T, int, bool> predicate)
        => ToArray().TakeWhile(predicate);

    //public IEnumerable<T> TakeLast(int count)
    //{ }
    #endregion


    #region Collection
    /// <summary>
    /// Creates a Dictionary&lt;TKey,TValue&gt; from an IEnumerable&lt;T&gt;.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="keySelector"></param>
    /// <returns></returns>
    public Dictionary<TKey, T> ToDictionary<TKey>(Func<T, TKey> keySelector) where TKey : notnull
        => ToArray().ToDictionary(keySelector);

    /// <summary>
    /// Creates a Dictionary&lt;TKey,TValue&gt; from an IEnumerable&lt;T&gt;.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="keySelector"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public Dictionary<TKey, T> ToDictionary<TKey>(Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer) where TKey : notnull
        => ToArray().ToDictionary(keySelector, comparer);

    /// <summary>
    /// Creates a Dictionary&lt;TKey,TValue&gt; from an IEnumerable&lt;T&gt;.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <param name="keySelector"></param>
    /// <param name="elementSelector"></param>
    /// <returns></returns>
    public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector) where TKey : notnull
        => ToArray().ToDictionary(keySelector, elementSelector);

    /// <summary>
    /// Creates a Dictionary&lt;TKey,TValue&gt; from an IEnumerable&lt;T&gt;.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <param name="keySelector"></param>
    /// <param name="elementSelector"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector, IEqualityComparer<TKey>? comparer) where TKey : notnull
        => ToArray().ToDictionary(keySelector, elementSelector, comparer);

    //public HashSet<T> ToHashSet() => source.ToHashSet(comparer: null);

    //public HashSet<T> ToHashSet(IEqualityComparer<T>? comparer) { }
    #endregion


    #region Union
    /// <summary>
    /// Produces the set union of two sequences
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public IEnumerable<T> Union(IEnumerable<T> second)
        => ToArray().Union(second);

    /// <summary>
    /// Produces the set union of two sequences
    /// </summary>
    /// <param name="second"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public IEnumerable<T> Union(IEnumerable<T> second, IEqualityComparer<T> comparer)
        => ToArray().Union(second, comparer);
    #endregion


    #region Where
    /// <summary>
    /// Filters a sequence of values based on a predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IEnumerable<T> Where(Func<T, bool> predicate)
        => ToArray().Where(predicate);

    /// <summary>
    /// Filters a sequence of values based on a predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IEnumerable<T> Where(Func<T, int, bool> predicate)
        => ToArray().Where(predicate);

    #endregion


    #region Zip
    //public IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector) { }

    //public IEnumerable<(TFirst First, TSecond Second)> Zip<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second) { }
    #endregion

    #endregion

}
