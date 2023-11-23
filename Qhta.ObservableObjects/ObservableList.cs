#define ImmutableItems
#define ThreadingTimer
using System;
using System.Collections;
using System.Collections.Generic;
#if ImmutableItems
using System.Collections.Immutable;
#endif
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

#if ThreadingTimer
using Timer = System.Threading.Timer;
#else
using Timer = System.Timers.Timer;
#endif

namespace Qhta.ObservableObjects
{
  /// <summary>
  /// Multithread version of List<typeparamref name="T"/> with CollectionChanged notification.
  /// It should be used instead of ObservableCollection<typeparamref name="T"/> in MVVM architecture model.
  /// To bind it to CollectionView, 
  /// <c>BindingOperator.EnableCollectionSynchronization(itemsCollection, itemsCollection.SyncRoot)</c> must be invoked. 
  /// Instead, it can be assured in XAML using CollectionViewBehavior class from Qhta.WPF.Behaviors assembly.
  /// Syntax is:
  /// <c>xmlns:bhv="clr-namespace:Qhta.WPF.Behaviors;assembly=Qhta.WPF.Behaviors"</c>
  /// <c>bhv:CollectionViewBehavior.EnableCollectionSynchronization="True"</c>
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

    private bool InNotifyCallback
    {
      get;
      set;
    }
    /// <summary>
    /// Internal ImmutableList
    /// </summary>
#if ImmutableItems
    protected ImmutableList<T> _Items;
#else
    protected List<T> _Items;
#endif
    #region constructors

    /// <summary>
    /// Default constructor.
    /// </summary>
    public ObservableList()
    {
#if ImmutableItems
      _Items = ImmutableList.Create<T>();
#else
      _Items = new List<T>();
#endif
    }

    /// <summary>
    /// Constructor with initial collection
    /// </summary>
    /// <param name="items"></param>
    public ObservableList(IEnumerable<T> items)
    {
#if ImmutableItems
      _Items = items.ToImmutableList<T>();
#else
      _Items = new List<T>(items);
#endif
    }
    #endregion

    #region List<T> wrappers

    /// <summary>
    /// Specifies whether collection size is fixed.
    /// </summary>
    public bool IsFixedSize
    {
      get
      {
          return _IsFixedSize;
      }
      set
      {
        _IsFixedSize = value;
      }
    }
    private bool _IsFixedSize;

    /// <summary>
    /// Specifies whether collection is read only.
    /// </summary>
    public bool IsReadOnly
    {
      get
      {
        return _IsReadOnly;
      }
      set
      {
        _IsReadOnly = value;
      }
    }
    private bool _IsReadOnly;

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
        //Debug.WriteLine($"Get({index})" + $" {DateTime.Now.TimeOfDay}");
        return _Items[index];
      }
      set
      {
        //Debug.WriteLine($"Set({index},{value})" + $" {DateTime.Now.TimeOfDay}");
        SetItem(index, value);
      }
    }

    /// <summary>
    ///   Gets the number of elements contained in the list.
    /// </summary>
    public int Count
    {
      get
      {
        var count = _Items.Count;
        return count;
      }
    }

    /// <summary>
    /// Sets an item changing ImmutableList.
    /// </summary>
    /// <param name="index">Index of the changed item.</param>
    /// <param name="value">Value to set.</param>
    public void SetItem(int index, T value)
    {
      //Debug.WriteLine($"SetItem({index},value)" + $" {DateTime.Now.TimeOfDay}");
      lock (LockObject)
      {
        var oldItem = _Items[index];
#if ImmutableItems
        _Items = _Items.SetItem(index, value);
#else
        _Items[index] = value;
#endif
        NotifyCollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldItem, value, index));
      }
    }

    /// <summary>
    ///   Adds an object to the end of the ImmutableList.
    /// </summary>
    /// <param name="item">
    ///   The object to be added to the end of the list. The value can be null for reference types.
    /// </param>
    public void Add(T item)
    {
      //Debug.WriteLine($"Add({item})" + $" {DateTime.Now.TimeOfDay}");
      lock (LockObject)
      {
        var index = _Items.Count;
#if ImmutableItems
        _Items = _Items.Add(item);
#else
        _Items.Add(item);
#endif
        NotifyCollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        NotifyPropertyChanged(nameof(Count));
      }
    }

    /// <summary>
    ///   Adds the elements of the specified collection to the end of the ImmutableList.
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
      //Debug.WriteLine($"AddRange({collection.Count()})" + $" {DateTime.Now.TimeOfDay}");
      lock (LockObject)
      {
        var index = _Items.Count;
        var list = new List<T>(collection);
#if ImmutableItems
        var oldItems = _Items;
        var newItems = _Items = _Items.AddRange(list);
#else
        _Items.AddRange(list);
#endif
        //NotifyCollectionChanged(
        //  this,
        //  new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        for (int i = 0; i < list.Count; i++)
          NotifyCollectionChanged(
            this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list[i], index++));

        //NotifyCollectionChanged(
        //  this,
        //  new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, index));

        NotifyPropertyChanged(nameof(Count));
      }
    }

    /// <summary>
    ///   Returns a read-only ReadOnlyCollection wrapper for the current collection.
    /// </summary>
    /// <returns>
    ///   An object that acts as a read-only wrapper around the current list.
    /// </returns>
    public ReadOnlyCollection<T>? AsReadOnly()
    {
      return _Items as ReadOnlyCollection<T>;
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
      return _Items.BinarySearch(item);
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
      return _Items.BinarySearch(item, comparer);
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
      return _Items.BinarySearch(index, count, item, comparer);
    }

    /// <summary>
    ///   Removes all elements from the list.
    /// </summary>
    public void Clear()
    {
      //Debug.WriteLine($"Clear()" + $" {DateTime.Now.TimeOfDay}");
      lock (LockObject)
      {
        //var _notificationDelay = NotificationDelay;
        //NotificationDelay = 0;
#if ImmutableItems
        _Items = ImmutableList<T>.Empty;
#else
        _Items.Clear();
#endif
        NotifyCollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        //NotificationDelay = _notificationDelay;
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
      return _Items.Contains(item);
    }

#if ImmutableItems
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
    public ImmutableList<TOutput> ConvertAll<TOutput>(Func<T, TOutput> converter)
    {
      return _Items.ConvertAll(converter);
    }
#endif

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
      _Items.CopyTo(index, array, arrayIndex, count);
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
      _Items.CopyTo(array, arrayIndex);
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
      _Items.CopyTo(array);
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
      return _Items.Exists(match);
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
    public T? Find(Predicate<T> match)
    {
      return _Items.Find(match);
    }

#if ImmutableItems
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
    public ImmutableList<T> FindAll(Predicate<T> match)
    {
      return _Items.FindAll(match);
    }
#endif

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
      return _Items.FindIndex(startIndex, count, match);
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
      return _Items.FindIndex(startIndex, match);
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
      return _Items.FindIndex(match);
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
    public T? FindLast(Predicate<T> match)
    {
      return _Items.FindLast(match);
    }

    ///<summary>
    ///    Searches for an element that matches the conditions defined by the specified
    ///    predicate, and returns the zero-based index of the last occurrence within the
    ///    range of elements in the list that contains the
    ///    specified number of elements and ends at the specified index.
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
    public int FindLastIndex(int startIndex, int count, Predicate<T> match)
    {
      return _Items.FindLastIndex(startIndex, count, match);
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
      return _Items.FindLastIndex(startIndex, match);
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
      return _Items.FindLastIndex(match);
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
        _Items.ForEach(action);
        NotifyCollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
      //Debug.WriteLine($"GetEnumerator()" + $" {DateTime.Now.TimeOfDay}");
      lock (LockObject)
      {
        for (int i = 0; i < _Items.Count; i++)
        {
          var item = _Items[i];
          yield return item;
        }
      }
    }

#if ImmutableItems
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
    public ImmutableList<T> GetRange(int index, int count)
    {
      return _Items.GetRange(index, count);
    }
#endif

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
      return _Items.IndexOf(item, index, count);
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
      return _Items.IndexOf(item, index);
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
      return _Items.IndexOf(item);
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
      //Debug.WriteLine($"Insert({index},{item})" + $" {DateTime.Now.TimeOfDay}");
      lock (LockObject)
      {
#if ImmutableItems
        _Items = _Items.Insert(index, item);
#else
        _Items.Insert(index, item);
#endif
        NotifyCollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        NotifyPropertyChanged(nameof(Count));
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
      //Debug.WriteLine($"InsertRange({index},{collection.Count()})" + $" {DateTime.Now.TimeOfDay}");
      lock (LockObject)
      {
#if ImmutableItems
        _Items = _Items.InsertRange(index, collection);
#else
        _Items.InsertRange(index, collection);
#endif
        NotifyCollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        NotifyPropertyChanged(nameof(Count));
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
      return _Items.LastIndexOf(item);
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
      return _Items.LastIndexOf(item, index);
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
    ///</returns>
    ///<exception cref="ArgumentOutOfRangeException">
    ///    index is outside the range of valid indexes for the list.
    ///    -or- count is less than 0. -or- index and count do not specify a valid section
    ///    in the list.
    ///</exception>
    public int LastIndexOf(T item, int index, int count)
    {
      return _Items.LastIndexOf(item, index, count);
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
      //Debug.WriteLine($"Remove({item})" + $" {DateTime.Now.TimeOfDay}");
      lock (LockObject)
      {
        int index = _Items.IndexOf(item);
        if (index < 0)
          return false;

#if ImmutableItems
        _Items = _Items.RemoveAt(index);
#else
        _Items.RemoveAt(index);
#endif
        NotifyCollectionChanged(
           this,
           new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        NotifyPropertyChanged(nameof(Count));
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
#if ImmutableItems
        _Items = _Items.RemoveAll(match);
#else
        _Items.RemoveAll(match);
#endif
        NotifyCollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        NotifyPropertyChanged(nameof(Count));
        return removeList.Count;
      }
    }

    ///<summary>
    ///    Removes the element at the specified index of the list.
    ///</summary>
    ///<param name="index">
    ///    The zero-based index of the element to remove.
    ///</param>
    ///<exception cref="ArgumentOutOfRangeException">
    ///    index is less than 0. -or- index is equal to or greater than Count.
    ///</exception>
    public void RemoveAt(int index)
    {
      //Debug.WriteLine($"RemoveAt({index})" + $" {DateTime.Now.TimeOfDay}");
      lock (LockObject)
      {
        var value = _Items[index];
#if ImmutableItems
        _Items = _Items.RemoveAt(index);
#else
        _Items.RemoveAt(index);
#endif
        NotifyCollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
        NotifyPropertyChanged(nameof(Count));
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
      //Debug.WriteLine($"RemoveRange({index},{count})" + $" {DateTime.Now.TimeOfDay}");
      lock (LockObject)
      {
        var count1 = _Items.Count - index;
        if (count1 < count)
          count = count1;
        var items = new T[count];
        _Items.CopyTo(index, items, 0, count);
#if ImmutableItems
        _Items = _Items.RemoveRange(index, count);
#else
        _Items.RemoveRange(index, count);
#endif
        NotifyCollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items, index));
        NotifyPropertyChanged(nameof(Count));
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
      //Debug.WriteLine($"Reverse({index},{count})" + $" {DateTime.Now.TimeOfDay}");
      lock (LockObject)
      {
#if ImmutableItems
        _Items = _Items.Reverse(index, count);
#else
        _Items.Reverse(index, count);
#endif
        NotifyCollectionChanged(
          this,
        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      }
    }


    ///<summary>
    ///    Reverses the order of the elements in the entire list.
    ///</summary>
    public void Reverse()
    {
      //Debug.WriteLine($"Reverse()" + $" {DateTime.Now.TimeOfDay}");
      lock (LockObject)
      {
#if ImmutableItems
        _Items = _Items.Reverse();
#else
        _Items.Reverse();
#endif
        NotifyCollectionChanged(
          this,
        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
      //Debug.WriteLine($"Sort({comparison})" + $" {DateTime.Now.TimeOfDay}");
      lock (LockObject)
      {
#if ImmutableItems
        _Items = _Items.Sort(comparison);
#else
        _Items.Sort(comparison);
#endif
        NotifyCollectionChanged(
          this,
        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
      //Debug.WriteLine($"Sort({index},{count},{comparer})" + $" {DateTime.Now.TimeOfDay}");
      lock (LockObject)
      {
#if ImmutableItems
        _Items = _Items.Sort(index, count, comparer);
#else
        _Items.Sort(index, count, comparer);
#endif
        NotifyCollectionChanged(
          this,
        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
      //Debug.WriteLine($"Sort()" + $" {DateTime.Now.TimeOfDay}");
      lock (LockObject)
      {
#if ImmutableItems
        _Items = _Items.Sort();
#else
        _Items.Sort();
#endif
        NotifyCollectionChanged(
          this,
        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
      //Debug.WriteLine($"Sort({comparer})" + $" {DateTime.Now.TimeOfDay}");
      lock (LockObject)
      {
#if ImmutableItems
        _Items = _Items.Sort(comparer);
#else
        _Items.Sort(comparer);
#endif
        NotifyCollectionChanged(
          this,
        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
      return _Items.ToArray();
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
      //Debug.WriteLine($"TrueForAll()" + $" {DateTime.Now.TimeOfDay}");
      return _Items.TrueForAll(match);
    }

    #endregion

    #region IEnumerable, ICollection implicit implementation
    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    void ICollection.CopyTo(Array array, int index)
    {
      if (array is T[] items)
        this.CopyTo(items, index);
      else
      {
#if ImmutableItems
        var arr = _Items.ToImmutableArray();
#else
        var arr = _Items.ToArray();
#endif
        foreach (var item in arr)
        {
          if (index >= array.Length)
            break;
          array.SetValue(item, index++);
        }
      }
    }
    #endregion

    #region IList explicit implementation
    object? IList.this[int index]
    {
      get
      {
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

    #region NotificationDelay

    /// <summary>
    /// Delay in collection change notifications (in milliseconds).
    /// If it is zero, notifications are sent immediately.
    /// If it is greater than zero, notifications are logged for this time 
    /// and after this time they are grouped and grouped notifications are sent.
    /// </summary>
    public int NotificationDelay
    {
      get
      {
        return _NotificationDelay;
      }
      set
      {
        if (_NotificationDelay != value)
        {
          _NotificationDelay = value;
          SwitchNotificationTimer(value > 0);
          NotifyPropertyChanged(nameof(NotificationDelay));
        }
      }
    }
    private int _NotificationDelay;

    private void RegisterNotification(object sender, NotifyCollectionChangedEventArgs args)
    {

      if (_CollectionChangedArgs != null)
#if ImmutableItems
        _CollectionChangedArgs = _CollectionChangedArgs.Add((sender, args));
#else
          _CollectionChangedArgs.Add((sender, args));
#endif
    }

    //private ImmutableList<NotifyCollectionChangedEventArgs> CollectionChangedArgs
    //{
    //  get
    //  {
    //    if (_CollectionChangedArgs == null)
    //      _CollectionChangedArgs = ImmutableList<NotifyCollectionChangedEventArgs>.Empty;
    //    return _CollectionChangedArgs;
    //  }
    //  set => _CollectionChangedArgs = value;
    //}
#if ImmutableItems
    private ImmutableList<(object, NotifyCollectionChangedEventArgs)>? _CollectionChangedArgs;
#else
    private List<(object, NotifyCollectionChangedEventArgs)>? _CollectionChangedArgs;
#endif

    private void SwitchNotificationTimer(bool enable)
    {

      if (enable)
      {
        _PreviousCollectionChangedCount = 0;
#if ImmutableItems
        _CollectionChangedArgs = ImmutableList<(object, NotifyCollectionChangedEventArgs)>.Empty;
#else
        _CollectionChangedArgs = new List<(object, NotifyCollectionChangedEventArgs)>();
#endif
#if ThreadingTimer
        _NotificationTimer = new Timer(_OnTimer, null, 0, _NotificationDelay);
#else
        _NotificationTimer =
          new System.Timers.Timer{ Interval = _NotificationDelay, AutoReset = true } ;
        _NotificationTimer.Elapsed += _NotificationTimer_Elapsed;
        _NotificationTimer.Start();
#endif
      }
      else
        _DisableTimer = true;
    }

#if ThreadingTimer
#else
    private void _NotificationTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      _OnTimer(e);
    }
#endif

    private Timer? _NotificationTimer;
    private bool _DisableTimer;
    private int _PreviousCollectionChangedCount;

    private void _OnTimer(object? state)
    {
      if (_CollectionChangedArgs != null)
      {
#if ImmutableItems
        var _argss = _CollectionChangedArgs.ToImmutableArray();
#else
        var _argss = _CollectionChangedArgs.ToArray();
#endif
        var argss = new List<(object, NotifyCollectionChangedEventArgs)>();
        for (int i = _PreviousCollectionChangedCount; i < _argss.Count(); i++)
        {
          var args = _argss[i];
          argss.Add(args);
          _PreviousCollectionChangedCount = i;
        }
        NotifyCollectionChangedEvents(argss);
      }
      if (_DisableTimer)
      {
        _NotificationTimer?.Dispose();
        _NotificationTimer = null;
        //_CollectionChangedArgs = null;
        //_PreviousCollectionChangedCount = 0;
      }
      else
      {
#if ThreadingTimer
        _NotificationTimer?.Change(0, _NotificationDelay);
#else
#endif
      }
    }

    private void NotifyCollectionChangedEvents(IEnumerable<(object, NotifyCollectionChangedEventArgs)> argss)
    {
      NotifyCollectionChangedAction? previousAction = null;
      foreach (var pair in argss)
      {
        var args = pair.Item2;
        switch (args.Action)
        {
          //case NotifyCollectionChangedAction.Reset:
          //  _NotifiedItemsCount = 0;
          //  base.NotifyCollectionChanged(this, args);
          //  break;
          ////  if (previousAction != NotifyCollectionChangedAction.Reset)
          ////  {
          ////    Debug.WriteLine($"{args.Action} NewStartingIndex={args.NewStartingIndex} NewItemsCount={args.NewItems?.Count}");
          ////    HandleCollectionChangedEvent(this, CollectionChanged, args);
          ////  }
          ////  break;
          //case NotifyCollectionChangedAction.Add:
          //          //  base.NotifyCollectionChanged(this, args);
          //  break;
          default:
            base.NotifyCollectionChanged(this, args);
            InNotifyCallback = false;
            break;
        }
        previousAction = args.Action;
      }
    }
    #endregion

    /// <summary>
    /// If NotificationDelay is greater than zero, then notification is registered
    /// to be handled by timer. Otherwise it is handled by base method.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public override void NotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      InNotifyCallback = true;

      //if (NotificationDelay > 0)
      //  RegisterNotification(sender, args);
      //else
      base.NotifyCollectionChanged(sender, args);
      InNotifyCallback = false;
    }
  }


}

