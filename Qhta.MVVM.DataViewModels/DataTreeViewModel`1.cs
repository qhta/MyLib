using Qhta.DispatchedObjects;

namespace Qhta.MVVM
{
  public abstract class DataTreeViewModel<EntityType>: DataTreeViewModel
  {
    public DispatchedCollection<EntityType> Items => _Items;
    DispatchedCollection<EntityType> _Items = new DispatchedCollection<EntityType>();
  }
}
