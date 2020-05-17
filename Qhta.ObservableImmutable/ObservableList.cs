using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Qhta.ObservableImmutable
{
  public class ObservableList<T> : ObservableCollectionObject,
    IEnumerable,
    ICollection,
    IList,
    IList<T>,
    ICollection<T>,
    IEnumerable<T>,
    IReadOnlyList<T>,
    IReadOnlyCollection<T>,
    INotifyCollectionChanged, INotifyPropertyChanged
  {

    protected List<T> _items = new List<T>();



    #region constructors
    public ObservableList() : this(new T[0], LockTypeEnum.SpinWait)
    {
    }

    public ObservableList(IEnumerable<T> items) : this(items, LockTypeEnum.SpinWait)
    {
    }

    public ObservableList(LockTypeEnum lockType) : this(new T[0], lockType)
    {
    }

    public ObservableList(IEnumerable<T> items, LockTypeEnum lockType) : base(lockType)
    {
      _items = new List<T>(items);
    }

    #endregion

    #region List<T> wrappers
    //
    // Summary:
    //     Gets or sets the element at the specified index.
    //
    // Parameters:
    //   index:
    //     The zero-based index of the element to get or set.
    //
    // Returns:
    //     The element at the specified index.
    //
    // Exceptions:
    //   T:System.ArgumentOutOfRangeException:
    //     index is less than 0. -or- index is equal to or greater than System.Collections.Generic.List`1.Count.
    public T this[int index]
    {
      get
      {
        return _items[index];
      }
      set
      {
        SetItem(index, value);
      }
    }

    //
    // Summary:
    //     Gets the number of elements contained in the System.Collections.Generic.List`1.
    //
    // Returns:
    //     The number of elements contained in the System.Collections.Generic.List`1.
    public int Count
    {
      get
      {
        return _items.Count;
      }
    }


    public void SetItem(int index, T value)
    {
      var oldItem = _items[index];
      _items[index] = value;
      NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldItem, value, index));
    }

    //
    // Summary:
    //     Adds an object to the end of the System.Collections.Generic.List`1.
    //
    // Parameters:
    //   item:
    //     The object to be added to the end of the System.Collections.Generic.List`1. The
    //     value can be null for reference types.
    public void Add(T item)
    {
      var index = _items.Count;
      _items.Add(item);
      NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
    }
    //
    // Summary:
    //     Adds the elements of the specified collection to the end of the System.Collections.Generic.List`1.
    //
    // Parameters:
    //   collection:
    //     The collection whose elements should be added to the end of the System.Collections.Generic.List`1.
    //     The collection itself cannot be null, but it can contain elements that are null,
    //     if type T is a reference type.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     collection is null.
    public void AddRange(IEnumerable<T> collection)
    {
      var index = _items.Count;
      _items.AddRange(collection);
      NotifyCollectionChanged(NotifyCollectionChangedAction.Add, collection, index);
    }
    //
    // Summary:
    //     Returns a read-only System.Collections.ObjectModel.ReadOnlyCollection`1 wrapper
    //     for the current collection.
    //
    // Returns:
    //     An object that acts as a read-only wrapper around the current System.Collections.Generic.List`1.
    public ReadOnlyCollection<T> AsReadOnly()
    {
      return _items.AsReadOnly();
    }

    //
    // Summary:
    //     Searches the entire sorted System.Collections.Generic.List`1 for an element using
    //     the default comparer and returns the zero-based index of the element.
    //
    // Parameters:
    //   item:
    //     The object to locate. The value can be null for reference types.
    //
    // Returns:
    //     The zero-based index of item in the sorted System.Collections.Generic.List`1,
    //     if item is found; otherwise, a negative number that is the bitwise complement
    //     of the index of the next element that is larger than item or, if there is no
    //     larger element, the bitwise complement of System.Collections.Generic.List`1.Count.
    //
    // Exceptions:
    //   T:System.InvalidOperationException:
    //     The default comparer System.Collections.Generic.Comparer`1.Default cannot find
    //     an implementation of the System.IComparable`1 generic interface or the System.IComparable
    //     interface for type T.
    public int BinarySearch(T item)
    {
      return _items.BinarySearch(item);
    }
    //
    // Summary:
    //     Searches the entire sorted System.Collections.Generic.List`1 for an element using
    //     the specified comparer and returns the zero-based index of the element.
    //
    // Parameters:
    //   item:
    //     The object to locate. The value can be null for reference types.
    //
    //   comparer:
    //     The System.Collections.Generic.IComparer`1 implementation to use when comparing
    //     elements. -or- null to use the default comparer System.Collections.Generic.Comparer`1.Default.
    //
    // Returns:
    //     The zero-based index of item in the sorted System.Collections.Generic.List`1,
    //     if item is found; otherwise, a negative number that is the bitwise complement
    //     of the index of the next element that is larger than item or, if there is no
    //     larger element, the bitwise complement of System.Collections.Generic.List`1.Count.
    //
    // Exceptions:
    //   T:System.InvalidOperationException:
    //     comparer is null, and the default comparer System.Collections.Generic.Comparer`1.Default
    //     cannot find an implementation of the System.IComparable`1 generic interface or
    //     the System.IComparable interface for type T.
    public int BinarySearch(T item, IComparer<T> comparer)
    {
      return _items.BinarySearch(item, comparer);
    }
    //
    // Summary:
    //     Searches a range of elements in the sorted System.Collections.Generic.List`1
    //     for an element using the specified comparer and returns the zero-based index
    //     of the element.
    //
    // Parameters:
    //   index:
    //     The zero-based starting index of the range to search.
    //
    //   count:
    //     The length of the range to search.
    //
    //   item:
    //     The object to locate. The value can be null for reference types.
    //
    //   comparer:
    //     The System.Collections.Generic.IComparer`1 implementation to use when comparing
    //     elements, or null to use the default comparer System.Collections.Generic.Comparer`1.Default.
    //
    // Returns:
    //     The zero-based index of item in the sorted System.Collections.Generic.List`1,
    //     if item is found; otherwise, a negative number that is the bitwise complement
    //     of the index of the next element that is larger than item or, if there is no
    //     larger element, the bitwise complement of System.Collections.Generic.List`1.Count.
    //
    // Exceptions:
    //   T:System.ArgumentOutOfRangeException:
    //     index is less than 0. -or- count is less than 0.
    //
    //   T:System.ArgumentException:
    //     index and count do not denote a valid range in the System.Collections.Generic.List`1.
    //
    //   T:System.InvalidOperationException:
    //     comparer is null, and the default comparer System.Collections.Generic.Comparer`1.Default
    //     cannot find an implementation of the System.IComparable`1 generic interface or
    //     the System.IComparable interface for type T.
    public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
    {
      return _items.BinarySearch(index, count, item, comparer);
    }

    //
    // Summary:
    //     Removes all elements from the System.Collections.Generic.List`1.
    public void Clear()
    {
      _items.Clear();
      NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
    }

    //
    // Summary:
    //     Determines whether an element is in the System.Collections.Generic.List`1.
    //
    // Parameters:
    //   item:
    //     The object to locate in the System.Collections.Generic.List`1. The value can
    //     be null for reference types.
    //
    // Returns:
    //     true if item is found in the System.Collections.Generic.List`1; otherwise, false.
    public bool Contains(T item)
    {
      return _items.Contains(item);
    }
    //
    // Summary:
    //     Converts the elements in the current System.Collections.Generic.List`1 to another
    //     type, and returns a list containing the converted elements.
    //
    // Parameters:
    //   converter:
    //     A System.Converter`2 delegate that converts each element from one type to another
    //     type.
    //
    // Type parameters:
    //   TOutput:
    //     The type of the elements of the target array.
    //
    // Returns:
    //     A System.Collections.Generic.List`1 of the target type containing the converted
    //     elements from the current System.Collections.Generic.List`1.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     converter is null.
    public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
    {
      return _items.ConvertAll(converter);
    }
    //
    // Summary:
    //     Copies a range of elements from the System.Collections.Generic.List`1 to a compatible
    //     one-dimensional array, starting at the specified index of the target array.
    //
    // Parameters:
    //   index:
    //     The zero-based index in the source System.Collections.Generic.List`1 at which
    //     copying begins.
    //
    //   array:
    //     The one-dimensional System.Array that is the destination of the elements copied
    //     from System.Collections.Generic.List`1. The System.Array must have zero-based
    //     indexing.
    //
    //   arrayIndex:
    //     The zero-based index in array at which copying begins.
    //
    //   count:
    //     The number of elements to copy.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     array is null.
    //
    //   T:System.ArgumentOutOfRangeException:
    //     index is less than 0. -or- arrayIndex is less than 0. -or- count is less than
    //     0.
    //
    //   T:System.ArgumentException:
    //     index is equal to or greater than the System.Collections.Generic.List`1.Count
    //     of the source System.Collections.Generic.List`1. -or- The number of elements
    //     from index to the end of the source System.Collections.Generic.List`1 is greater
    //     than the available space from arrayIndex to the end of the destination array.
    public void CopyTo(int index, T[] array, int arrayIndex, int count)
    {
      _items.CopyTo(index, array, arrayIndex, count);
    }
    //
    // Summary:
    //     Copies the entire System.Collections.Generic.List`1 to a compatible one-dimensional
    //     array, starting at the specified index of the target array.
    //
    // Parameters:
    //   array:
    //     The one-dimensional System.Array that is the destination of the elements copied
    //     from System.Collections.Generic.List`1. The System.Array must have zero-based
    //     indexing.
    //
    //   arrayIndex:
    //     The zero-based index in array at which copying begins.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     array is null.
    //
    //   T:System.ArgumentOutOfRangeException:
    //     arrayIndex is less than 0.
    //
    //   T:System.ArgumentException:
    //     The number of elements in the source System.Collections.Generic.List`1 is greater
    //     than the available space from arrayIndex to the end of the destination array.
    public void CopyTo(T[] array, int arrayIndex)
    {
      _items.CopyTo(array, arrayIndex);
    }
    //
    // Summary:
    //     Copies the entire System.Collections.Generic.List`1 to a compatible one-dimensional
    //     array, starting at the beginning of the target array.
    //
    // Parameters:
    //   array:
    //     The one-dimensional System.Array that is the destination of the elements copied
    //     from System.Collections.Generic.List`1. The System.Array must have zero-based
    //     indexing.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     array is null.
    //
    //   T:System.ArgumentException:
    //     The number of elements in the source System.Collections.Generic.List`1 is greater
    //     than the number of elements that the destination array can contain.
    public void CopyTo(T[] array)
    {
      _items.CopyTo(array);
    }
    //
    // Summary:
    //     Determines whether the System.Collections.Generic.List`1 contains elements that
    //     match the conditions defined by the specified predicate.
    //
    // Parameters:
    //   match:
    //     The System.Predicate`1 delegate that defines the conditions of the elements to
    //     search for.
    //
    // Returns:
    //     true if the System.Collections.Generic.List`1 contains one or more elements that
    //     match the conditions defined by the specified predicate; otherwise, false.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     match is null.
    public bool Exists(Predicate<T> match)
    {
      return _items.Exists(match);
    }
    //
    // Summary:
    //     Searches for an element that matches the conditions defined by the specified
    //     predicate, and returns the first occurrence within the entire System.Collections.Generic.List`1.
    //
    // Parameters:
    //   match:
    //     The System.Predicate`1 delegate that defines the conditions of the element to
    //     search for.
    //
    // Returns:
    //     The first element that matches the conditions defined by the specified predicate,
    //     if found; otherwise, the default value for type T.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     match is null.
    public T Find(Predicate<T> match)
    {
      return _items.Find(match);
    }
    //
    // Summary:
    //     Retrieves all the elements that match the conditions defined by the specified
    //     predicate.
    //
    // Parameters:
    //   match:
    //     The System.Predicate`1 delegate that defines the conditions of the elements to
    //     search for.
    //
    // Returns:
    //     A System.Collections.Generic.List`1 containing all the elements that match the
    //     conditions defined by the specified predicate, if found; otherwise, an empty
    //     System.Collections.Generic.List`1.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     match is null.
    public List<T> FindAll(Predicate<T> match)
    {
      return _items.FindAll(match);
    }
    //
    // Summary:
    //     Searches for an element that matches the conditions defined by the specified
    //     predicate, and returns the zero-based index of the first occurrence within the
    //     range of elements in the System.Collections.Generic.List`1 that starts at the
    //     specified index and contains the specified number of elements.
    //
    // Parameters:
    //   startIndex:
    //     The zero-based starting index of the search.
    //
    //   count:
    //     The number of elements in the section to search.
    //
    //   match:
    //     The System.Predicate`1 delegate that defines the conditions of the element to
    //     search for.
    //
    // Returns:
    //     The zero-based index of the first occurrence of an element that matches the conditions
    //     defined by match, if found; otherwise, –1.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     match is null.
    //
    //   T:System.ArgumentOutOfRangeException:
    //     startIndex is outside the range of valid indexes for the System.Collections.Generic.List`1.
    //     -or- count is less than 0. -or- startIndex and count do not specify a valid section
    //     in the System.Collections.Generic.List`1.
    public int FindIndex(int startIndex, int count, Predicate<T> match)
    {
      return _items.FindIndex(startIndex, count, match);
    }

    //
    // Summary:
    //     Searches for an element that matches the conditions defined by the specified
    //     predicate, and returns the zero-based index of the first occurrence within the
    //     range of elements in the System.Collections.Generic.List`1 that extends from
    //     the specified index to the last element.
    //
    // Parameters:
    //   startIndex:
    //     The zero-based starting index of the search.
    //
    //   match:
    //     The System.Predicate`1 delegate that defines the conditions of the element to
    //     search for.
    //
    // Returns:
    //     The zero-based index of the first occurrence of an element that matches the conditions
    //     defined by match, if found; otherwise, –1.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     match is null.
    //
    //   T:System.ArgumentOutOfRangeException:
    //     startIndex is outside the range of valid indexes for the System.Collections.Generic.List`1.
    public int FindIndex(int startIndex, Predicate<T> match)
    {
      return _items.FindIndex(startIndex, match);
    }
    //
    // Summary:
    //     Searches for an element that matches the conditions defined by the specified
    //     predicate, and returns the zero-based index of the first occurrence within the
    //     entire System.Collections.Generic.List`1.
    //
    // Parameters:
    //   match:
    //     The System.Predicate`1 delegate that defines the conditions of the element to
    //     search for.
    //
    // Returns:
    //     The zero-based index of the first occurrence of an element that matches the conditions
    //     defined by match, if found; otherwise, –1.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     match is null.
    public int FindIndex(Predicate<T> match)
    {
      return _items.FindIndex(match);
    }
    //
    // Summary:
    //     Searches for an element that matches the conditions defined by the specified
    //     predicate, and returns the last occurrence within the entire System.Collections.Generic.List`1.
    //
    // Parameters:
    //   match:
    //     The System.Predicate`1 delegate that defines the conditions of the element to
    //     search for.
    //
    // Returns:
    //     The last element that matches the conditions defined by the specified predicate,
    //     if found; otherwise, the default value for type T.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     match is null.
    public T FindLast(Predicate<T> match)
    {
      return _items.FindLast(match);
    }
    //
    // Summary:
    //     Searches for an element that matches the conditions defined by the specified
    //     predicate, and returns the zero-based index of the last occurrence within the
    //     range of elements in the System.Collections.Generic.List`1 that contains the
    //     specified number of elements and ends at the specified index.
    //
    // Parameters:
    //   startIndex:
    //     The zero-based starting index of the backward search.
    //
    //   count:
    //     The number of elements in the section to search.
    //
    //   match:
    //     The System.Predicate`1 delegate that defines the conditions of the element to
    //     search for.
    //
    // Returns:
    //     The zero-based index of the last occurrence of an element that matches the conditions
    //     defined by match, if found; otherwise, –1.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     match is null.
    //
    //   T:System.ArgumentOutOfRangeException:
    //     startIndex is outside the range of valid indexes for the System.Collections.Generic.List`1.
    //     -or- count is less than 0. -or- startIndex and count do not specify a valid section
    //     in the System.Collections.Generic.List`1.
    public int FindLastIndex(int startIndex, int count, Predicate<T> match)
    {
      return _items.FindLastIndex(startIndex, count, match);
    }
    //
    // Summary:
    //     Searches for an element that matches the conditions defined by the specified
    //     predicate, and returns the zero-based index of the last occurrence within the
    //     range of elements in the System.Collections.Generic.List`1 that extends from
    //     the first element to the specified index.
    //
    // Parameters:
    //   startIndex:
    //     The zero-based starting index of the backward search.
    //
    //   match:
    //     The System.Predicate`1 delegate that defines the conditions of the element to
    //     search for.
    //
    // Returns:
    //     The zero-based index of the last occurrence of an element that matches the conditions
    //     defined by match, if found; otherwise, –1.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     match is null.
    //
    //   T:System.ArgumentOutOfRangeException:
    //     startIndex is outside the range of valid indexes for the System.Collections.Generic.List`1.
    public int FindLastIndex(int startIndex, Predicate<T> match)
    {
      return _items.FindLastIndex(startIndex, match);
    }
    //
    // Summary:
    //     Searches for an element that matches the conditions defined by the specified
    //     predicate, and returns the zero-based index of the last occurrence within the
    //     entire System.Collections.Generic.List`1.
    //
    // Parameters:
    //   match:
    //     The System.Predicate`1 delegate that defines the conditions of the element to
    //     search for.
    //
    // Returns:
    //     The zero-based index of the last occurrence of an element that matches the conditions
    //     defined by match, if found; otherwise, –1.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     match is null.
    public int FindLastIndex(Predicate<T> match)
    {
      return _items.FindLastIndex(match);
    }
    //
    // Summary:
    //     Performs the specified action on each element of the System.Collections.Generic.List`1.
    //
    // Parameters:
    //   action:
    //     The System.Action`1 delegate to perform on each element of the System.Collections.Generic.List`1.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     action is null.
    //
    //   T:System.InvalidOperationException:
    //     An element in the collection has been modified.
    public void ForEach(Action<T> action)
    {
      _items.ForEach(action);
      NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
    }
    //
    // Summary:
    //     Returns an enumerator that iterates through the System.Collections.Generic.List`1.
    //
    // Returns:
    //     A System.Collections.Generic.List`1.Enumerator for the System.Collections.Generic.List`1.
    public IEnumerator<T> GetEnumerator()
    {
      return _items.GetEnumerator();
    }

    //
    // Summary:
    //     Creates a shallow copy of a range of elements in the source System.Collections.Generic.List`1.
    //
    // Parameters:
    //   index:
    //     The zero-based System.Collections.Generic.List`1 index at which the range starts.
    //
    //   count:
    //     The number of elements in the range.
    //
    // Returns:
    //     A shallow copy of a range of elements in the source System.Collections.Generic.List`1.
    //
    // Exceptions:
    //   T:System.ArgumentOutOfRangeException:
    //     index is less than 0. -or- count is less than 0.
    //
    //   T:System.ArgumentException:
    //     index and count do not denote a valid range of elements in the System.Collections.Generic.List`1.
    public List<T> GetRange(int index, int count)
    {
      return _items.GetRange(index, count);
    }
    //
    // Summary:
    //     Searches for the specified object and returns the zero-based index of the first
    //     occurrence within the range of elements in the System.Collections.Generic.List`1
    //     that starts at the specified index and contains the specified number of elements.
    //
    // Parameters:
    //   item:
    //     The object to locate in the System.Collections.Generic.List`1. The value can
    //     be null for reference types.
    //
    //   index:
    //     The zero-based starting index of the search. 0 (zero) is valid in an empty list.
    //
    //   count:
    //     The number of elements in the section to search.
    //
    // Returns:
    //     The zero-based index of the first occurrence of item within the range of elements
    //     in the System.Collections.Generic.List`1 that starts at index and contains count
    //     number of elements, if found; otherwise, –1.
    //
    // Exceptions:
    //   T:System.ArgumentOutOfRangeException:
    //     index is outside the range of valid indexes for the System.Collections.Generic.List`1.
    //     -or- count is less than 0. -or- index and count do not specify a valid section
    //     in the System.Collections.Generic.List`1.
    public int IndexOf(T item, int index, int count)
    {
      return _items.IndexOf(item, index, count);
    }
    //
    // Summary:
    //     Searches for the specified object and returns the zero-based index of the first
    //     occurrence within the range of elements in the System.Collections.Generic.List`1
    //     that extends from the specified index to the last element.
    //
    // Parameters:
    //   item:
    //     The object to locate in the System.Collections.Generic.List`1. The value can
    //     be null for reference types.
    //
    //   index:
    //     The zero-based starting index of the search. 0 (zero) is valid in an empty list.
    //
    // Returns:
    //     The zero-based index of the first occurrence of item within the range of elements
    //     in the System.Collections.Generic.List`1 that extends from index to the last
    //     element, if found; otherwise, –1.
    //
    // Exceptions:
    //   T:System.ArgumentOutOfRangeException:
    //     index is outside the range of valid indexes for the System.Collections.Generic.List`1.
    public int IndexOf(T item, int index)
    {
      return _items.IndexOf(item, index);
    }
    //
    // Summary:
    //     Searches for the specified object and returns the zero-based index of the first
    //     occurrence within the entire System.Collections.Generic.List`1.
    //
    // Parameters:
    //   item:
    //     The object to locate in the System.Collections.Generic.List`1. The value can
    //     be null for reference types.
    //
    // Returns:
    //     The zero-based index of the first occurrence of item within the entire System.Collections.Generic.List`1,
    //     if found; otherwise, –1.
    public int IndexOf(T item)
    {
      return _items.IndexOf(item);
    }
    //
    // Summary:
    //     Inserts an element into the System.Collections.Generic.List`1 at the specified
    //     index.
    //
    // Parameters:
    //   index:
    //     The zero-based index at which item should be inserted.
    //
    //   item:
    //     The object to insert. The value can be null for reference types.
    //
    // Exceptions:
    //   T:System.ArgumentOutOfRangeException:
    //     index is less than 0. -or- index is greater than System.Collections.Generic.List`1.Count.
    public void Insert(int index, T item)
    {
      _items.Insert(index, item);
      NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
    }

    //
    // Summary:
    //     Inserts the elements of a collection into the System.Collections.Generic.List`1
    //     at the specified index.
    //
    // Parameters:
    //   index:
    //     The zero-based index at which the new elements should be inserted.
    //
    //   collection:
    //     The collection whose elements should be inserted into the System.Collections.Generic.List`1.
    //     The collection itself cannot be null, but it can contain elements that are null,
    //     if type T is a reference type.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     collection is null.
    //
    //   T:System.ArgumentOutOfRangeException:
    //     index is less than 0. -or- index is greater than System.Collections.Generic.List`1.Count.
    public void InsertRange(int index, IEnumerable<T> collection)
    {
      _items.InsertRange(index, collection);
      NotifyCollectionChanged(NotifyCollectionChangedAction.Add, collection, index);
    }
    //
    // Summary:
    //     Searches for the specified object and returns the zero-based index of the last
    //     occurrence within the entire System.Collections.Generic.List`1.
    //
    // Parameters:
    //   item:
    //     The object to locate in the System.Collections.Generic.List`1. The value can
    //     be null for reference types.
    //
    // Returns:
    //     The zero-based index of the last occurrence of item within the entire the System.Collections.Generic.List`1,
    //     if found; otherwise, –1.
    public int LastIndexOf(T item)
    {
      return _items.LastIndexOf(item);
    }
    //
    // Summary:
    //     Searches for the specified object and returns the zero-based index of the last
    //     occurrence within the range of elements in the System.Collections.Generic.List`1
    //     that extends from the first element to the specified index.
    //
    // Parameters:
    //   item:
    //     The object to locate in the System.Collections.Generic.List`1. The value can
    //     be null for reference types.
    //
    //   index:
    //     The zero-based starting index of the backward search.
    //
    // Returns:
    //     The zero-based index of the last occurrence of item within the range of elements
    //     in the System.Collections.Generic.List`1 that extends from the first element
    //     to index, if found; otherwise, –1.
    //
    // Exceptions:
    //   T:System.ArgumentOutOfRangeException:
    //     index is outside the range of valid indexes for the System.Collections.Generic.List`1.
    public int LastIndexOf(T item, int index)
    {
      return _items.LastIndexOf(item, index);
    }
    //
    // Summary:
    //     Searches for the specified object and returns the zero-based index of the last
    //     occurrence within the range of elements in the System.Collections.Generic.List`1
    //     that contains the specified number of elements and ends at the specified index.
    //
    // Parameters:
    //   item:
    //     The object to locate in the System.Collections.Generic.List`1. The value can
    //     be null for reference types.
    //
    //   index:
    //     The zero-based starting index of the backward search.
    //
    //   count:
    //     The number of elements in the section to search.
    //
    // Returns:
    //     The zero-based index of the last occurrence of item within the range of elements
    //     in the System.Collections.Generic.List`1 that contains count number of elements
    //     and ends at index, if found; otherwise, –1.
    //
    // Exceptions:
    //   T:System.ArgumentOutOfRangeException:
    //     index is outside the range of valid indexes for the System.Collections.Generic.List`1.
    //     -or- count is less than 0. -or- index and count do not specify a valid section
    //     in the System.Collections.Generic.List`1.
    public int LastIndexOf(T item, int index, int count)
    {
      return _items.LastIndexOf(item, index, count);
    }

    //
    // Summary:
    //     Removes the first occurrence of a specific object from the System.Collections.Generic.List`1.
    //
    // Parameters:
    //   item:
    //     The object to remove from the System.Collections.Generic.List`1. The value can
    //     be null for reference types.
    //
    // Returns:
    //     true if item is successfully removed; otherwise, false. This method also returns
    //     false if item was not found in the System.Collections.Generic.List`1.
    public bool Remove(T item)
    {
      int index = _items.IndexOf(item);
      if (index < 0)
        return false;

      _items.RemoveAt(index);
      NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
      return true;
    }
    //
    // Summary:
    //     Removes all the elements that match the conditions defined by the specified predicate.
    //
    // Parameters:
    //   match:
    //     The System.Predicate`1 delegate that defines the conditions of the elements to
    //     remove.
    //
    // Returns:
    //     The number of elements removed from the System.Collections.Generic.List`1 .
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     match is null.
    public int RemoveAll(Predicate<T> match)
    {
      return _items.RemoveAll(match);
    }
    //
    // Summary:
    //     Removes the element at the specified index of the System.Collections.Generic.List`1.
    //
    // Parameters:
    //   index:
    //     The zero-based index of the element to remove.
    //
    // Exceptions:
    //   T:System.ArgumentOutOfRangeException:
    //     index is less than 0. -or- index is equal to or greater than System.Collections.Generic.List`1.Count.
    public void RemoveAt(int index)
    {
      var value = _items[index];
      _items.RemoveAt(index);
      NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, value, index);
    }
    //
    // Summary:
    //     Removes a range of elements from the System.Collections.Generic.List`1.
    //
    // Parameters:
    //   index:
    //     The zero-based starting index of the range of elements to remove.
    //
    //   count:
    //     The number of elements to remove.
    //
    // Exceptions:
    //   T:System.ArgumentOutOfRangeException:
    //     index is less than 0. -or- count is less than 0.
    //
    //   T:System.ArgumentException:
    //     index and count do not denote a valid range of elements in the System.Collections.Generic.List`1.
    public void RemoveRange(int index, int count)
    {
      var count1 = _items.Count - index;
      if (count1 < count)
        count = count1;
      var items = new T[count];
      _items.CopyTo(index, items, 0, count);
      _items.RemoveRange(index, count);
      NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, items, index);
    }
    //
    // Summary:
    //     Reverses the order of the elements in the specified range.
    //
    // Parameters:
    //   index:
    //     The zero-based starting index of the range to reverse.
    //
    //   count:
    //     The number of elements in the range to reverse.
    //
    // Exceptions:
    //   T:System.ArgumentOutOfRangeException:
    //     index is less than 0. -or- count is less than 0.
    //
    //   T:System.ArgumentException:
    //     index and count do not denote a valid range of elements in the System.Collections.Generic.List`1.
    public void Reverse(int index, int count)
    {
      _items.Reverse(index, count);
      NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
    }
    //
    // Summary:
    //     Reverses the order of the elements in the entire System.Collections.Generic.List`1.
    public void Reverse()
    {
      _items.Reverse();
      NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
    }
    //
    // Summary:
    //     Sorts the elements in the entire System.Collections.Generic.List`1 using the
    //     specified System.Comparison`1.
    //
    // Parameters:
    //   comparison:
    //     The System.Comparison`1 to use when comparing elements.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     comparison is null.
    //
    //   T:System.ArgumentException:
    //     The implementation of comparison caused an error during the sort. For example,
    //     comparison might not return 0 when comparing an item with itself.
    public void Sort(Comparison<T> comparison)
    {
      _items.Sort(comparison);
      NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
    }
    //
    // Summary:
    //     Sorts the elements in a range of elements in System.Collections.Generic.List`1
    //     using the specified comparer.
    //
    // Parameters:
    //   index:
    //     The zero-based starting index of the range to sort.
    //
    //   count:
    //     The length of the range to sort.
    //
    //   comparer:
    //     The System.Collections.Generic.IComparer`1 implementation to use when comparing
    //     elements, or null to use the default comparer System.Collections.Generic.Comparer`1.Default.
    //
    // Exceptions:
    //   T:System.ArgumentOutOfRangeException:
    //     index is less than 0. -or- count is less than 0.
    //
    //   T:System.ArgumentException:
    //     index and count do not specify a valid range in the System.Collections.Generic.List`1.
    //     -or- The implementation of comparer caused an error during the sort. For example,
    //     comparer might not return 0 when comparing an item with itself.
    //
    //   T:System.InvalidOperationException:
    //     comparer is null, and the default comparer System.Collections.Generic.Comparer`1.Default
    //     cannot find implementation of the System.IComparable`1 generic interface or the
    //     System.IComparable interface for type T.
    public void Sort(int index, int count, IComparer<T> comparer)
    {
      _items.Sort(index, count, comparer);
      NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
    }    //
         // Summary:
         //     Sorts the elements in the entire System.Collections.Generic.List`1 using the
         //     default comparer.
         //
         // Exceptions:
         //   T:System.InvalidOperationException:
         //     The default comparer System.Collections.Generic.Comparer`1.Default cannot find
         //     an implementation of the System.IComparable`1 generic interface or the System.IComparable
         //     interface for type T.
    public void Sort()
    {
      _items.Sort();
      NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
    }
    //
    // Summary:
    //     Sorts the elements in the entire System.Collections.Generic.List`1 using the
    //     specified comparer.
    //
    // Parameters:
    //   comparer:
    //     The System.Collections.Generic.IComparer`1 implementation to use when comparing
    //     elements, or null to use the default comparer System.Collections.Generic.Comparer`1.Default.
    //
    // Exceptions:
    //   T:System.InvalidOperationException:
    //     comparer is null, and the default comparer System.Collections.Generic.Comparer`1.Default
    //     cannot find implementation of the System.IComparable`1 generic interface or the
    //     System.IComparable interface for type T.
    //
    //   T:System.ArgumentException:
    //     The implementation of comparer caused an error during the sort. For example,
    //     comparer might not return 0 when comparing an item with itself.
    public void Sort(IComparer<T> comparer)
    {
      _items.Sort(comparer);
      NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
    }
    //
    // Summary:
    //     Copies the elements of the System.Collections.Generic.List`1 to a new array.
    //
    // Returns:
    //     An array containing copies of the elements of the System.Collections.Generic.List`1.
    public T[] ToArray()
    {
      return _items.ToArray();
    }
    //
    // Summary:
    //     Sets the capacity to the actual number of elements in the System.Collections.Generic.List`1,
    //     if that number is less than a threshold value.
    public void TrimExcess()
    {
      _items.TrimExcess();
    }
    //
    // Summary:
    //     Determines whether every element in the System.Collections.Generic.List`1 matches
    //     the conditions defined by the specified predicate.
    //
    // Parameters:
    //   match:
    //     The System.Predicate`1 delegate that defines the conditions to check against
    //     the elements.
    //
    // Returns:
    //     true if every element in the System.Collections.Generic.List`1 matches the conditions
    //     defined by the specified predicate; otherwise, false. If the list has no elements,
    //     the return value is true.
    //
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     match is null.
    public bool TrueForAll(Predicate<T> match)
    {
      return _items.TrueForAll(match);
    }

    #endregion

    #region ICollection<T> missing implementation
    bool ICollection<T>.IsReadOnly => false;
    #endregion

    #region IEnumerable, ICollection, IList explicit implementation
    object IList.this[int index]
    {
      get => this[index];
      set
      {
        if (value is T item)
          this[index] = item;
      }
    }

    bool IList.IsFixedSize => false;

    bool IList.IsReadOnly => false;

    int ICollection.Count => this.Count;

    bool ICollection.IsSynchronized => (_items as ICollection).IsSynchronized;

    object ICollection.SyncRoot => (_items as ICollection).SyncRoot;

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

    void IList.Clear() => this.Clear();

    bool IList.Contains(object value)
    {
      if (value is T item)
        return this.Contains(item);
      return false;
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

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

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

    void IList.RemoveAt(int index) => this.RemoveAt(index);
    #endregion

  }
}
