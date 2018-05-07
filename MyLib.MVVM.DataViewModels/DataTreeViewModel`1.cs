using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLib.MultiThreadingObjects;

namespace MyLib.MVVM
{
  public abstract class DataTreeViewModel<EntityType>: DataTreeViewModel
  {
    public DispatchedCollection<EntityType> Items => _Items;
    DispatchedCollection<EntityType> _Items = new DispatchedCollection<EntityType>();
  }
}
