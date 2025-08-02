using System.Collections;

namespace Qhta.SF.Tools;

/// <summary>
/// A collection interface that supports removing items.
/// </summary>
public interface IRemovableCollection: ICollection
{
  /// <summary>
  /// Returns a value indicating whether the specified item can be removed from the collection.
  /// </summary>
  /// <param name="item"></param>
  /// <returns></returns>
  bool CanRemove(object item);

  /// <summary>
  /// Removes the specified item from the collection.
  /// </summary>
  /// <param name="item">The item to remove.</param>
  void Remove(object item);
  
  /// <summary>
  /// Indicates whether the collection is read-only.
  /// </summary>
  bool IsReadOnly { get; }
}