using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;

namespace Qhta.ObservableObjects
{
  /// <summary>
  /// Multithread version of List<typeparamref name="T"/> with CollectionChanged notification.
  /// It should be used instead of ObservableCollection<typeparamref name="T"/> in MVVM architecture model.
  /// To bind it to CollectionView, <c>BindingOperator.EnableCollectionSynchronization(itemsCollection, itemsCollection.SyncRoot)</c>
  /// must be invoked. It can be assured in XAML using CollectionViewBehavior class from Qhta.WPF.Utils assembly.
  /// Syntax is:
  /// <c>xmlns:utils="clr-namespace:Qhta.WPF.Utils;assembly=Qhta.WPF.Utils"</c>
  /// <c>utils:CollectionViewBehavior.EnableCollectionSynchronization="True"</c>
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class ObservableList<T> : ObservableCollectionObject,
    IEnumerable,
    ICollection,
    IList, 
    IEnumerable<T>,
    ICollection<T>,
    IList<T>,
    INotifyCollectionChanged, INotifyPropertyChanged
  {

    protected List<T> _items = new List<T>();

    #region constructors

    public ObservableList() : this(new T[0]) { }

    public ObservableList(Dispatcher dispatcher) : this(new T[0], dispatcher)
    {
    }

    public ObservableList(IEnumerable<T> items, Dispatcher dispatcher) : base(dispatcher)
    {
      foreach (var item in items)
        _items.Add(item);
    }

    public ObservableList(IEnumerable<T> items) : base()
    {
      foreach (var item in items)
        _items.Add(item);
    }
    #endregion

    #region List<T> wrappers

    public bool IsFixedSize => false;

    public bool IsReadOnly => false;

    /// <summary>
    ///   Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   index is less than 0. -or- index is equal to or greater than list count.
    /// </exception>
    public T this[int index]
    {
      get
      {
        //Debug.WriteLine($"\tthis[{index}]" + $" {DateTime.Now.ToString(dateTimeFormat)}");
        return _items[index];
      }
      set
      {
        lock (LockObject)
          SetItem(index, value);
      }
    }

    /// <summary>
    ///   Gets the number of elements contained in the list
    /// </summary>
    public int Count
    {
      get
      {
        lock (LockObject)
        {
          var count = _items.Count;
          //Debug.WriteLine($"{this}.Count = {count}" + $" {DateTime.Now.ToString(dateTimeFormat)}");
          //Debug.WriteLine($"{Environment.StackTrace}");
          return count;
        }
      }
    }


    public void SetItem(int index, T value)
    {
      lock (LockObject)
      {
        var oldItem = _items[index];
        _items[index] = value;
        NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldItem, value, index));
      }
    }

    /// <summary>
    ///   Adds an object to the end of the list
    /// </summary>
    /// <param name="item">
    ///   The object to be added to the end of the list. The value can be null for reference types.
    /// </param>
    public void Add(T item)
    {
      //Debug.WriteLine($"Add({item})" + $" {DateTime.Now.ToString(dateTimeFormat)}");
      lock (LockObject)
      {
        var index = _items.Count;
        _items.Add(item);
        NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
      }
    }

    /// <summary>
    ///   Adds the elements of the specified collection to the end of the list.
    /// </summary>
    /// <param name="collection">
    ///   The collection whose elements should be added to the end of the list.
    ///   The collection itself cannot be null, but it can contain elements that are null,
    ///   if type T is a reference type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   collection is null.
    /// </exception>
    public void AddRange(IEnumerable<T> collection)
    {
      lock (LockObject)
      {
        var index = _items.Count;
        foreach (var item in collection)
          _items.Add(item);
        NotifyCollectionChanged(NotifyCollectionChangedAction.Add, collection, index);
      }
    }

    /// <summary>
    ///   Returns a read-only ReadOnlyCollection wrapper for the current collection.
    /// </summary>
    /// <returns>
    ///   An object that acts as a read-only wrapper around the current list.
    /// </returns>
    public ReadOnlyCollection<T> AsReadOnly()
    {
      lock (LockObject)
        return _items.AsReadOnly();
    }

    /// <summary>
    ///   Searches the entire sorted list for an element using
    ///   the default comparer and returns the zero-based index of the element.
    /// </summary>
    /// <param name="item">
    ///   The object to locate. The value can be null for reference types.
    /// </param>
    /// <returns>
    ///    The zero-based index of item in the sorted list,
    ///    if item is found; otherwise, a negative number that is the bitwise complement
    ///    of the index of the next element that is larger than item or, if there is no
    ///    larger element, the bitwise complement of list Count.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///    The default comparer cannot find an implementation of the IComparable&lt;T&gt; generic interface 
    ///    or the IComparable interface for type T.
    /// </exception>
    public int BinarySearch(T item)
    {
      lock (LockObject)
        return _items.BinarySearch(item);
    }

    /// <summary>
    ///   Searches the entire sorted list for an element using
    ///   the specified comparer and returns the zero-based index of the element.
    /// </summary>
    /// <param name="item">
    ///   The object to locate. The value can be null for reference types.
    /// </param>
    /// <param name="comparer">
    ///   The IComparer&lt;T$gt; implementation to use when comparing
    ///   elements. -or- null to use the default comparer.
    /// </param>
    /// <returns>
    ///  The zero-based index of item in the sorted list,
    ///   if item is found; otherwise, a negative number that is the bitwise complement
    ///   of the index of the next element that is larger than item or, if there is no
    ///   larger element, the bitwise complement of Count.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///   comparer is null, and the default comparer
    ///   cannot find an implementation of the IComparable&lt;T$gt; generic interface or
    ///   the IComparable interface for type T.
    /// </exception>
    public int BinarySearch(T item, IComparer<T> comparer)
    {
      lock (LockObject)
        return _items.BinarySearch(item, comparer);
    }

    /// <summary>
    ///   Searches a range of elements in the sorted list 
    ///   for an element using the specified comparer and returns the zero-based index
    ///   of the element.
    /// </summary>
    /// <param name="index">The zero-based starting index of the range to search.</param>
    /// <param name="count">The length of the range to search.</param>
    /// <param name="item">The object to locate. The value can be null for reference types.</param>
    /// <param name="comparer">
    ///   The IComparer implementation to use when comparing
    ///   elements, or null to use the default comparer.
    /// </param>
    /// <returns>
    ///   The zero-based index of item in the sorted list,
    ///   if item is found; otherwise, a negative number that is the bitwise complement
    ///   of the index of the next element that is larger than item or, if there is no
    ///   larger element, the bitwise complement of Count.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   index is less than 0. -or- count is less than 0.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   index and count do not denote a valid range in the list.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   comparer is null, and the default comparer
    ///   cannot find an implementation of the IComparable&lt;T$gt; generic interface or
    ///   the IComparable interface for type T.
    /// </exception>
    public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
    {
      lock (LockObject)
        return _items.BinarySearch(index, count, item, comparer);
    }

    /// <summary>
    ///   Removes all elements from the list.
    /// </summary>
    public void Clear()
    {
      lock (LockObject)
      {
        //Debug.WriteLine($"{this}.Clear" + $" {DateTime.Now.ToString(dateTimeFormat)}");
        var priorItems = _items.ToList();
        foreach (var item in priorItems)
          _items.Remove(item);
        _items.Clear();
        var enumerator = GetEnumerator();
        enumerator.Dispose();
        //Debug.WriteLine($"{this}.Count = {Count}" + $" {DateTime.Now.ToString(dateTimeFormat)}");
        NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
      }
    }

    /// <summary>
    ///   Determines whether an element is in the list.
    /// </summary>
    /// <param name="item">
    ///   The object to locate in the list. The value can be null for reference types.
    /// </param>
    /// <returns>true if item is found in the list; otherwise, false.</returns>
    public bool Contains(T item)
    {
      lock (LockObject)
        return _items.Contains(item);
    }

    /// <summary>
    ///   Converts the elements in the current list to another type, and returns a list containing the converted elements.
    /// </summary>
    /// <typeparam name="TOutput">The type of the elements of the target array.</typeparam>
    /// <param name="converter">A delegate that converts each element from one type to another type.</param>
    /// <returns>
    ///   A list of the target type containing the converted elements from the current list.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   converter is null.
    /// </exception>
    public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
    {
      lock (LockObject)
        return _items.ConvertAll(converter);
    }

    /// <summary>
    ///   Copies a range of elements from the list to a compatible
    ///   one-dimensional array, starting at the specified index of the target array.
    /// </summary>
    /// <param name="index">The zero-based index in the source list at which copying begins.</param>
    /// <param name="array">
    ///   The one-dimensional Array that is the destination of the elements copied
    ///   from list. The Array must have zero-based indexing.
    /// </param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <param name="count">The number of elements to copy.</param>
    /// <exception cref="ArgumentNullException">
    ///   array is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   index is less than 0. -or- arrayIndex is less than 0. -or- count is less than 0.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   index is equal to or greater than the Count
    ///   of the source list. -or- The number of elements
    ///   from index to the end of the source list is greater
    ///   than the available space from arrayIndex to the end of the destination array.
    /// </exception>
    public void CopyTo(int index, T[] array, int arrayIndex, int count)
    {
      lock (LockObject)
        _items.CopyTo(index, array, arrayIndex, count);
    }

    /// <summary>
    ///   Copies the entire list to a compatible one-dimensional
    ///   array, starting at the specified index of the target array.
    /// </summary>
    /// <param name="array">
    ///   The one-dimensional Array that is the destination of the elements copied
    ///   from list. The Array must have zero-based indexing.
    /// </param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="ArgumentNullException">
    ///   array is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   arrayIndex is less than 0.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   The number of elements in the source list is greater
    ///   than the available space from arrayIndex to the end of the destination array.
    /// </exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
      lock (LockObject)
        _items.CopyTo(array, arrayIndex);
    }

    /// <summary>
    ///   Copies the entire list to a compatible one-dimensional
    ///   array, starting at the beginning of the target array.
    /// </summary>
    /// <param name="array">
    ///   The one-dimensional Array that is the destination of the elements copied
    ///   from list. The Array must have zero-based indexing.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   array is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   The number of elements in the source list is greater
    ///   than the number of elements that the destination array can contain.
    /// </exception>
    public void CopyTo(T[] array)
    {
      lock (LockObject)
        _items.CopyTo(array);
    }

    /// <summary>
    ///    Determines whether the list contains elements that
    ///    match the conditions defined by the specified predicate.
    /// </summary>
    /// <param name="match">
    ///    The Predicate&lt;T$gt; delegate that defines the conditions of the elements to
    ///    search for.
    /// </param>
    /// <returns>
    ///    true if the list contains one or more elements that
    ///    match the conditions defined by the specified predicate; otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   match is null.
    /// </exception>
    public bool Exists(Predicate<T> match)
    {
      lock (LockObject)
        return _items.Exists(match);
    }

    ///<summary>
    ///    Searches for an element that matches the conditions defined by the specified
    ///    predicate, and returns the first occurrence within the entire list.
    ///</summary>
    ///<param name="match">
    ///    The Predicate&lt;T$gt; delegate that defines the conditions of the element to
    ///    search for.
    ///</param>
    ///<returns>
    ///    The first element that matches the conditions defined by the specified predicate,
    ///    if found; otherwise, the default value for type T.
    ///</returns>
    ///<exception cref="ArgumentNullException">
    ///    match is null.
    ///</exception>
    public T Find(Predicate<T> match)
    {
      lock (LockObject)
        return _items.Find(match);
    }

    ///<summary>
    ///    Retrieves all the elements that match the conditions defined by the specified
    ///    predicate.
    ///</summary>
    ///<param name="match">
    ///    The Predicate&lt;T$gt; delegate that defines the conditions of the elements to
    ///    search for.
    ///</param>
    ///<returns>
    ///    A list containing all the elements that match the
    ///    conditions defined by the specified predicate, if found; otherwise, an empty
    ///    list.
    ///</returns>
    ///<exception cref="ArgumentNullException">
    ///    match is null.
    ///</exception>
    public List<T> FindAll(Predicate<T> match)
    {
      lock (LockObject)
        return _items.FindAll(match);
    }
    //
    ///<summary>
    ///    Searches for an element that matches the conditions defined by the specified
    ///    predicate, and returns the zero-based index of the first occurrence within the
    ///    range of elements in the list that starts at the
    ///    specified index and contains the specified number of elements.
    ///</summary>
    ///<param name="startIndex">
    ///    The zero-based starting index of the search.
    ///</param>
    ///<param name="count">
    ///    The number of elements in the section to search.
    ///</param>
    ///<param name="match">
    ///    The Predicate&lt;T$gt; delegate that defines the conditions of the elements to
    ///    search for.
    ///</param>
    ///<returns>
    ///    The zero-based index of the first occurrence of an element that matches the conditions
    ///    defined by match, if found; otherwise, –1.
    ///</returns>
    ///<exception cref="ArgumentNullException">
    ///    match is null.
    ///</exception>
    ///<exception cref="ArgumentOutOfRangeException">
    ///    startIndex is outside the range of valid indexes for the list.
    ///    -or- count is less than 0. -or- startIndex and count do not specify a valid section
    ///    in the list.
    ///</exception>
    public int FindIndex(int startIndex, int count, Predicate<T> match)
    {
      lock (LockObject)
        return _items.FindIndex(startIndex, count, match);
    }


    ///<summary>
    ///    Searches for an element that matches the conditions defined by the specified
    ///    predicate, and returns the zero-based index of the first occurrence within the
    ///    range of elements in the list that extends from
    ///    the specified index to the last element.
    ///</summary>
    ///<param name="startIndex">
    ///    The zero-based starting index of the search.
    ///</param>
    ///<param name="match">
    ///    The Predicate&lt;T$gt; delegate that defines the conditions of the elements to
    ///    search for.
    ///</param>
    ///<returns>
    ///    The zero-based index of the first occurrence of an element that matches the conditions
    ///    defined by match, if found; otherwise, –1.
    ///</returns>
    ///<exception cref="ArgumentNullException">
    ///    match is null.
    ///</exception>
    ///<exception cref="ArgumentOutOfRangeException">
    ///    startIndex is outside the range of valid indexes for the list.
    ///</exception>
    public int FindIndex(int startIndex, Predicate<T> match)
    {
      lock (LockObject)
        return _items.FindIndex(startIndex, match);
    }

    ///<summary>
    ///    Searches for an element that matches the conditions defined by the specified
    ///    predicate, and returns the zero-based index of the first occurrence within the
    ///    entire list.
    ///</summary>
    ///<param name="match">
    ///    The Predicate&lt;T$gt; delegate that defines the conditions of the element to
    ///    search for.
    ///</param>
    ///<returns>
    ///    The zero-based index of the first occurrence of an element that matches the conditions
    ///    defined by match, if found; otherwise, –1.
    ///</returns>
    ///<exception cref="ArgumentNullException">
    ///    match is null.
    ///</exception>
    public int FindIndex(Predicate<T> match)
    {
      lock (LockObject)
        return _items.FindIndex(match);
    }

    ///<summary>
    ///    Searches for an element that matches the conditions defined by the specified
    ///    predicate, and returns the last occurrence within the entire list.
    ///</summary>
    ///<param name="match">
    ///    The Predicate&lt;T$gt; delegate that defines the conditions of the element to
    ///    search for.
    ///</param>
    ///<returns>
    ///    The last element that matches the conditions defined by the specified predicate,
    ///    if found; otherwise, the default value for type T.
    ///</returns>
    ///<exception cref="ArgumentNullException">
    ///    match is null.
    ///</exception>
    public T FindLast(Predicate<T> match)
    {
      lock (LockObject)
        return _items.FindLast(match);
    }

    ///<summary>
    ///    Searches for an element that matches the conditions defined by the specified
    ///    predicate, and returns the zero-based index of the last occurrence within the
    ///    range of elements in the list that contains the
    ///    specified number of elements and ends at the specified index.
    ///<param name="startIndex">
    ///    The zero-based starting index of the search.
    ///</param>
    ///<param name="count">
    ///    The number of elements in the section to search.
    ///</param>
    ///<param name="match">
    ///    The Predicate&lt;T$gt; delegate that defines the conditions of the elements to
    ///    search for.
    ///</param>
    ///<returns>
    ///    The zero-based index of the first occurrence of an element that matches the conditions
    ///    defined by match, if found; otherwise, –1.
    ///</returns>
    ///<exception cref="ArgumentNullException">
    ///    match is null.
    ///</exception>
    ///<exception cref="ArgumentOutOfRangeException">
    ///    startIndex is outside the range of valid indexes for the list.
    ///    -or- count is less than 0. -or- startIndex and count do not specify a valid section
    ///    in the list.
    ///</exception>
    public int FindLastIndex(int startIndex, int count, Predicate<T> match)
    {
      lock (LockObject)
        return _items.FindLastIndex(startIndex, count, match);
    }

    ///<summary>
    ///    Searches for an element that matches the conditions defined by the specified
    ///    predicate, and returns the zero-based index of the last occurrence within the
    ///    range of elements in the list that extends from
    ///    the first element to the specified index.
    ///</summary>
    ///<param name="startIndex">
    ///    The zero-based starting index of the search.
    ///</param>
    ///<param name="match">
    ///    The Predicate&lt;T$gt; delegate that defines the conditions of the elements to
    ///    search for.
    ///</param>
    ///<returns>
    ///    The zero-based index of the first occurrence of an element that matches the conditions
    ///    defined by match, if found; otherwise, –1.
    ///</returns>
    ///<exception cref="ArgumentNullException">
    ///    match is null.
    ///</exception>
    ///<exception cref="ArgumentOutOfRangeException">
    ///    startIndex is outside the range of valid indexes for the list.
    ///</exception>
    public int FindLastIndex(int startIndex, Predicate<T> match)
    {
      lock (LockObject)
        return _items.FindLastIndex(startIndex, match);
    }

    ///<summary>
    ///    Searches for an element that matches the conditions defined by the specified
    ///    predicate, and returns the zero-based index of the last occurrence within the
    ///    entire list.
    ///</summary>
    ///<param name="match">
    ///    The Predicate&lt;T$gt; delegate that defines the conditions of the element to
    ///    search for.
    ///</param>
    ///<returns>
    ///    The last element that matches the conditions defined by the specified predicate,
    ///    if found; otherwise, the default value for type T.
    ///</returns>
    ///<exception cref="ArgumentNullException">
    ///    match is null.
    ///</exception>
    public int FindLastIndex(Predicate<T> match)
    {
      lock (LockObject)
        return _items.FindLastIndex(match);
    }

    ///<summary>
    ///    Performs the specified action on each element of the list.
    ///</summary>
    ///<param name="action">
    ///    The Action&lt;T$gt; delegate to perform on each element of the list.
    ///</param>
    ///<exception cref="ArgumentNullException">
    ///    action is null.
    ///</exception>
    ///<exception cref="InvalidOperationException">
    ///    An element in the collection has been modified.
    ///</exception>
    public void ForEach(Action<T> action)
    {
      lock (LockObject)
      {
        _items.ForEach(action);
        NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
      }
    }
    //
    ///<summary>
    ///    Returns an enumerator that iterates through the list.
    ///</summary>
    ///<returns>
    ///    A list.Enumerator for the list.
    ///</returns>
    public IEnumerator<T> GetEnumerator()
    {
      //Debug.WriteLine($"GetEnumerator" + $" {DateTime.Now.ToString(dateTimeFormat)}");
      lock (LockObject)
      {
        for (int i = 0; i < _items.Count; i++)
        {
          var item = _items[i];
          yield return item;
        }
      }
    }

    ///<summary>
    ///    Creates a shallow copy of a range of elements in the source list.
    ///</summary>
    ///<param name="index">
    ///    The zero-based list index at which the range starts.
    ///</param>
    ///<param name="count">
    ///    The number of elements in the range.
    ///</param>
    ///<returns>
    ///    A shallow copy of a range of elements in the source list.
    ///</returns>
    ///<exception cref="ArgumentOutOfRangeException">
    ///    index is less than 0. -or- count is less than 0.
    ///</exception>
    ///<exception cref="ArgumentException">
    ///    index and count do not denote a valid range of elements in the list.
    ///</exception>
    public List<T> GetRange(int index, int count)
    {
      lock (LockObject)
        return _items.GetRange(index, count);
    }
    //
    ///<summary>
    ///    Searches for the specified object and returns the zero-based index of the first
    ///    occurrence within the range of elements in the list
    ///    that starts at the specified index and contains the specified number of elements.
    ///</summary>
    ///<param name="item">
    ///    The object to locate in the list. The value can
    ///    be null for reference types.
    ///</param>
    ///<param name="index">
    ///    The zero-based starting index of the search. 0 (zero) is valid in an empty list.
    ///</param>
    ///<param name="count">
    ///    The number of elements in the section to search.
    ///</param>
    ///<returns>
    ///    The zero-based index of the first occurrence of item within the range of elements
    ///    in the list that starts at index and contains count
    ///    number of elements, if found; otherwise, –1.
    ///</returns>
    ///<exception cref="ArgumentOutOfRangeException">
    ///    index is outside the range of valid indexes for the list.
    ///    -or- count is less than 0. -or- index and count do not specify a valid section
    ///    in the list.
    ///</exception>
    public int IndexOf(T item, int index, int count)
    {
      lock (LockObject)
        return _items.IndexOf(item, index, count);
    }

    ///<summary>
    ///    Searches for the specified object and returns the zero-based index of the first
    ///    occurrence within the range of elements in the list
    ///    that extends from the specified index to the last element.
    ///</summary>
    ///<param name="item">
    ///    The object to locate in the list. The value can
    ///    be null for reference types.
    ///</param>
    ///<param name="index">
    ///    The zero-based starting index of the search. 0 (zero) is valid in an empty list.
    ///</param>
    ///<returns>
    ///    The zero-based index of the first occurrence of item within the range of elements
    ///    in the list that extends from index to the last
    ///    element, if found; otherwise, –1.
    ///</returns>
    ///<exception cref="ArgumentOutOfRangeException">
    ///    index is outside the range of valid indexes for the list.
    ///</exception>
    public int IndexOf(T item, int index)
    {
      lock (LockObject)
        return _items.IndexOf(item, index);
    }

    ///<summary>
    ///    Searches for the specified object and returns the zero-based index of the first
    ///    occurrence within the entire list.
    ///</summary>
    ///<param name="item">
    ///    The object to locate in the list. The value can
    ///    be null for reference types.
    ///</param>
    ///<returns>
    ///    The zero-based index of the first occurrence of item within the entire list,
    ///    if found; otherwise, –1.
    ///</returns>
    public int IndexOf(T item)
    {
      lock (LockObject)
        return _items.IndexOf(item);
    }

    ///<summary>
    ///    Inserts an element into the list at the specified
    ///    index.
    ///</summary>
    ///<param name="index">
    ///    The zero-based index at which item should be inserted.
    ///</param>
    ///<param name="item">
    ///    The object to insert. The value can be null for reference types.
    ///</param>
    ///<exception cref="ArgumentOutOfRangeException">
    ///    index is less than 0. -or- index is greater than Count.
    ///</exception>
    public void Insert(int index, T item)
    {
      lock (LockObject)
      {
        _items.Insert(index, item);
        NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
      }
    }


    ///<summary>
    ///    Inserts the elements of a collection into the list
    ///    at the specified index.
    ///</summary>
    ///<param name="index">
    ///    The zero-based index at which the new elements should be inserted.
    ///</param>
    ///<param name="collection">
    ///    The collection whose elements should be inserted into the list.
    ///    The collection itself cannot be null, but it can contain elements that are null,
    ///    if type T is a reference type.
    ///</param>
    ///<exception cref="ArgumentNullException">
    ///    collection is null.
    ///</exception>
    ///<exception cref="ArgumentOutOfRangeException">
    ///    index is less than 0. -or- index is greater than Count.
    ///</exception>
    public void InsertRange(int index, IEnumerable<T> collection)
    {
      lock (LockObject)
      {
        foreach (var item in collection)
          _items.Insert(index++, item);
        NotifyCollectionChanged(NotifyCollectionChangedAction.Add, collection, index);
      }
    }

    ///<summary>
    ///    Searches for the specified object and returns the zero-based index of the last
    ///    occurrence within the entire list.
    ///</summary>
    ///<param name="item">
    ///    The object to locate in the list. The value can
    ///    be null for reference types.
    ///</param>
    ///<returns>
    ///    The zero-based index of the last occurrence of item within the entire the list,
    ///    if found; otherwise, –1.
    ///</returns>
    public int LastIndexOf(T item)
    {
      lock (LockObject)
        return _items.LastIndexOf(item);
    }

    ///<summary>
    ///    Searches for the specified object and returns the zero-based index of the last
    ///    occurrence within the range of elements in the list
    ///    that extends from the first element to the specified index.
    ///</summary>
    ///<param name="item">
    ///    The object to locate in the list. The value can
    ///    be null for reference types.
    ///</param>
    ///<param name="index">
    ///    The zero-based starting index of the backward search.
    ///</param>
    ///<returns>
    ///    The zero-based index of the last occurrence of item within the range of elements
    ///    in the list that extends from the first element
    ///    to index, if found; otherwise, –1.
    ///</returns>
    ///<exception cref="ArgumentOutOfRangeException">
    ///    index is outside the range of valid indexes for the list.
    ///</exception>
    public int LastIndexOf(T item, int index)
    {
      lock (LockObject)
        return _items.LastIndexOf(item, index);
    }

    ///<summary>
    ///    Searches for the specified object and returns the zero-based index of the last
    ///    occurrence within the range of elements in the list
    ///    that contains the specified number of elements and ends at the specified index.
    ///</summary>
    ///<param name="item">
    ///    The object to locate in the list. The value can
    ///    be null for reference types.
    ///</param>
    ///<param name="index">
    ///    The zero-based starting index of the backward search.
    ///</param>
    ///<param name="count">
    ///    The number of elements in the section to search.
    ///</param>
    ///<returns>
    ///    The zero-based index of the last occurrence of item within the range of elements
    ///    in the list that contains count number of elements
    ///    and ends at index, if found; otherwise, –1.
    ///<exception cref="ArgumentOutOfRangeException">
    ///    index is outside the range of valid indexes for the list.
    ///    -or- count is less than 0. -or- index and count do not specify a valid section
    ///    in the list.
    ///</exception>
    public int LastIndexOf(T item, int index, int count)
    {
      lock (LockObject)
        return _items.LastIndexOf(item, index, count);
    }

    ///<summary>
    ///    Removes the first occurrence of a specific object from the list.
    ///</summary>
    ///<param name="item">
    ///    The object to remove from the list. The value can
    ///    be null for reference types.
    ///</param>
    ///<returns>
    ///    true if item is successfully removed; otherwise, false. This method also returns
    ///    false if item was not found in the list.
    ///</returns>
    public bool Remove(T item)
    {
      lock (LockObject)
      {
        int index = _items.IndexOf(item);
        if (index < 0)
          return false;

        _items.RemoveAt(index);
        NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
        return true;
      }
    }

    ///<summary>
    ///    Removes all the elements that match the conditions defined by the specified predicate.
    ///</summary>
    ///<param name="match">
    ///    The Predicate&lt;T$gt; delegate that defines the conditions of the elements to
    ///    remove.
    ///</param>
    ///<returns>
    ///    The number of elements removed from the list .
    ///</returns>
    ///<exception cref="ArgumentNullException">
    ///    match is null.
    ///</exception>
    public int RemoveAll(Predicate<T> match)
    {
      lock (LockObject)
      {
        var removeList = this.Where(item => match(item)).ToList();
        foreach (var item in removeList)
          _items.Remove(item);
        NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
        return removeList.Count;
      }
    }

    ///<summary>
    ///    Removes the element at the specified index of the list.
    ///
    ///<param name="index">
    ///    The zero-based index of the element to remove.
    ///</param>
    ///<exception cref="ArgumentOutOfRangeException">
    ///    index is less than 0. -or- index is equal to or greater than Count.
    ///</exception>
    public void RemoveAt(int index)
    {
      lock (LockObject)
      {
        var value = _items[index];
        _items.RemoveAt(index);
        NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, value, index);
      }
    }

    ///<summary>
    ///    Removes a range of elements from the list.
    ///</summary>
    ///<param name="index">
    ///    The zero-based starting index of the range of elements to remove.
    ///</param>
    ///<param name="count">
    ///    The number of elements to remove.
    ///</param>
    ///<exception cref="ArgumentOutOfRangeException">
    ///    index is less than 0. -or- count is less than 0.
    ///</exception>
    ///<exception cref="ArgumentException">
    ///    index and count do not denote a valid range of elements in the list.
    ///</exception>
    public void RemoveRange(int index, int count)
    {
      lock (LockObject)
      {
        var count1 = _items.Count - index;
        if (count1 < count)
          count = count1;
        var items = new T[count];
        _items.CopyTo(index, items, 0, count);
        _items.RemoveRange(index, count);
        NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, items, index);
      }
    }

    ///<summary>
    ///    Reverses the order of the elements in the specified range.
    ///</summary>
    ///<param name="index">
    ///    The zero-based starting index of the range to reverse.
    ///</param>
    ///<param name="count">
    ///    The number of elements in the range to reverse.
    ///</param>
    ///<exception cref="ArgumentOutOfRangeException">
    ///    index is less than 0. -or- count is less than 0.
    ///</exception>
    ///<exception cref="ArgumentException">
    ///    index and count do not denote a valid range of elements in the list.
    ///</exception>
    public void Reverse(int index, int count)
    {
      lock (LockObject)
      {
        _items.Reverse(index, count);
        NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
      }
    }


    ///<summary>
    ///    Reverses the order of the elements in the entire list.
    ///</summary>
    public void Reverse()
    {
      lock (LockObject)
      {
        _items.Reverse();
        NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
      }
    }

    ///<summary>
    ///    Sorts the elements in the entire list using the
    ///    specified Comparison&lt;T$gt;.
    ///</summary>
    ///<param name="comparison">
    ///    The Comparison&lt;T$gt; to use when comparing elements.
    ///</param>
    ///<exception cref="ArgumentNullException">
    ///    comparison is null.
    ///</exception>
    ///<exception cref="ArgumentException">
    ///    The implementation of comparison caused an error during the sort. For example,
    ///    comparison might not return 0 when comparing an item with itself.
    ///</exception>
    public void Sort(Comparison<T> comparison)
    {
      lock (LockObject)
      {
        _items.Sort(comparison);
        NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
      }
    }

    ///<summary>
    ///    Sorts the elements in a range of elements in list
    ///    using the specified comparer.
    ///</summary>
    ///<param name="index">
    ///    The zero-based starting index of the range to sort.
    ///</param>
    ///<param name="count">
    ///    The length of the range to sort.
    ///</param>
    ///<param name="comparer">
    ///    The IComparer&lt;T$gt; implementation to use when comparing
    ///    elements, or null to use the default comparer.
    ///</param>
    ///<exception cref="ArgumentOutOfRangeException">
    ///    index is less than 0. -or- count is less than 0.
    ///</exception>
    ///<exception cref="ArgumentException">
    ///    index and count do not specify a valid range in the list.
    ///    -or- The implementation of comparer caused an error during the sort. For example,
    ///    comparer might not return 0 when comparing an item with itself.
    ///</exception>
    ///<exception cref="InvalidOperationException">
    ///    comparer is null, and the default comparer
    ///    cannot find implementation of the IComparable&lt;T$gt; generic interface or the
    ///    IComparable interface for type T.
    ///</exception>
    public void Sort(int index, int count, IComparer<T> comparer)
    {
      lock (LockObject)
      {
        _items.Sort(index, count, comparer);
        NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
      }
    }

    ///<summary>
    ///    Sorts the elements in the entire list using the
    ///    default comparer.
    ///</summary>
    ///<exception cref="InvalidOperationException">
    ///    The default comparer cannot find
    ///    an implementation of the IComparable&lt;T$gt; generic interface or the IComparable
    ///    interface for type T.
    ///</exception>
    public void Sort()
    {
      lock (LockObject)
      {
        _items.Sort();
        NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
      }
    }

    ///<summary>
    ///    Sorts the elements in the entire list using the
    ///    specified comparer.
    ///</summary>
    ///<param name="comparer">
    ///    The IComparer&lt;T$gt; implementation to use when comparing
    ///    elements, or null to use the default comparer.
    ///</param>
    ///<exception cref="InvalidOperationException">
    ///    comparer is null, and the default comparer
    ///    cannot find implementation of the IComparable&lt;T$gt; generic interface or the
    ///    IComparable interface for type T.
    ///</exception>
    ///<exception cref="ArgumentException">
    ///    The implementation of comparer caused an error during the sort. For example,
    ///    comparer might not return 0 when comparing an item with itself.
    ///</exception>
    public void Sort(IComparer<T> comparer)
    {
      lock (LockObject)
      {
        _items.Sort(comparer);
        NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
      }
    }


    /// <summary>
    /// Copies the elements of the list to a new array.
    /// </summary>
    /// <returns>
    /// An array containing copies of the elements of the list.
    /// </returns>
    public T[] ToArray()
    {
      lock (LockObject)
        return _items.ToArray();
    }

    ///<summary>
    ///    This method in List&lt;T&gt; implementation sets the capacity 
    ///    to the actual number of elements in the list,
    ///    if that number is less than a threshold value.
    ///    In this class this method does nothing.
    ///</summary>
    public void TrimExcess()
    {
      lock (LockObject)
        _items.TrimExcess();
    }

    ///<summary>
    ///    Determines whether every element in the list matches
    ///    the conditions defined by the specified predicate.
    ///</summary>
    ///<param name="match">
    ///    The Predicate&lt;T$gt; delegate that defines the conditions to check against
    ///    the elements.
    ///</param>
    ///<returns>
    ///    true if every element in the list matches the conditions
    ///    defined by the specified predicate; otherwise, false. If the list has no elements,
    ///    the return value is true.
    ///</returns>
    ///<exception cref="ArgumentNullException">
    ///    match is null.
    ///</exception>
    public bool TrueForAll(Predicate<T> match)
    {
      lock (LockObject)
        return _items.TrueForAll(match);
    }

    #endregion

    #region IEnumerable, ICollection implicit implementation
    IEnumerator IEnumerable.GetEnumerator()
    {
      //Debug.WriteLine($"IEnumerable.GetEnumerator" + $" {DateTime.Now.ToString(dateTimeFormat)}");
      return this.GetEnumerator();
    }

    void ICollection.CopyTo(Array array, int index)
    {
      if (array is T[] items)
        this.CopyTo(items, index);
      else
      {
        (_items as ICollection).CopyTo(array, index);
      }
    }
    #endregion

    #region IList explicit implementation
    object IList.this[int index]
    {
      get
      {
        //Debug.WriteLine($"IList.this[{index}]" + $" {DateTime.Now.ToString(dateTimeFormat)}");
        return this[index];
      }
      set
      {
        if (value is T item)
          this[index] = item;
      }
    }

    int IList.Add(object value)
    {
      if (value is T item)
      {
        var index = this.Count;
        this.Add(item);
        return index;
      }
      return -1;
    }

    bool IList.Contains(object value)
    {
      if (value is T item)
        return this.Contains(item);
      return false;
    }

    int IList.IndexOf(object value)
    {
      if (value is T item)
        return this.IndexOf(item);
      return -1;
    }

    void IList.Insert(int index, object value)
    {
      if (value is T item)
        this.Insert(index, item);
    }

    void IList.Remove(object value)
    {
      if (value is T item)
        this.Remove(item);
    }

    #endregion

  }
}
