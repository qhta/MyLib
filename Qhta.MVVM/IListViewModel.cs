using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Qhta.MVVM
{
  /// <summary>
  /// Interface for a <see cref="ListViewModel{ItemType}"/>.
  /// </summary>
  public interface IListViewModel
  {
    /// <summary>
    /// Gets a type of the item.
    /// </summary>
    /// <returns></returns>
    Type GetItemType();

    /// <summary>
    /// Enumerates through all item items.
    /// </summary>
    /// <returns></returns>
    IEnumerable<object> GetItems();

    /// <summary>
    /// Enumerable of all items.
    /// </summary>
    IEnumerable<object> Items { get; }

    /// <summary>
    /// Current item.
    /// </summary>
    object? CurrentItem { get; set; }

    /// <summary>
    /// Enumerable of selected items.
    /// </summary>
    IEnumerable<object> SelectedItems { get; set; }

    /// <summary>
    /// A method of selecting or deselecting all items.
    /// </summary>
    /// <param name="select"></param>
    /// 
    void SelectAll(bool select);
    /// <summary>
    /// Parent of the <see cref="ListViewModel"/>
    /// </summary>
    IViewModel? ParentViewModel { get; }

    /// <summary>
    /// A string specified how <see cref="ListViewModel"/> items are sorted.
    /// </summary>
    string? SortedBy { get; set; }

    /// <summary>
    /// A method to switch to the first item with a specific pattern and selected propNames.
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="propNames"></param>
    void FindFirstItem(object pattern, IEnumerable<string> propNames);

    /// <summary>
    /// A method to switch to the first item that fullfills the expression.
    /// </summary>
    /// <param name="expression"></param>
    void FindFirstItem(Expression<Func<object, bool>> expression);

    /// <summary>
    /// A method to switch to the next item.
    /// </summary>
    void FindNextItem();

    /// <summary>
    /// A method to switch to the first invalid item.
    /// </summary>
    void FindFirstInvalidItem();

    /// <summary>
    /// A method to switch to the next invalid item.
    /// </summary>
    void FindNextInvalidItem();
  }
}