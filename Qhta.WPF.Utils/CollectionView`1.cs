namespace Qhta.WPF.Utils;

public class CollectionView<T>: CollectionView, IFiltered
{
  public CollectionView(IEnumerable collection): base(collection){ IsFiltered = true;}

  public bool IsFiltered { get; set; }
}
