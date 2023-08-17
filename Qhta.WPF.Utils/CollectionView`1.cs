namespace Qhta.WPF.Utils;

/// <summary>
/// Generic version of CollectionView class.
/// </summary>
/// <typeparam name="T"></typeparam>
public class CollectionView<T>: CollectionView
{
  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="collection"></param>
  public CollectionView(IEnumerable collection): base(collection){}

}
