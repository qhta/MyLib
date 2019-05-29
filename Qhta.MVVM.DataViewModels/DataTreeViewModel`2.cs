using Qhta.DispatchedObjects;

namespace Qhta.MVVM
{
  public abstract class DataTreeViewModel<EntityType, ChildType>: DataTreeViewModel<ChildType>
  {
    public EntityType Model { get; set; }

  }
}
