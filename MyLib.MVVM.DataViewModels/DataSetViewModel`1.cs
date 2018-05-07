using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLib.MultiThreadingObjects;

namespace MyLib.MVVM
{
  public abstract class DataSetViewModel<EntityType>: DataSetViewModel
  {
    public DispatchedCollection<EntityType> Items => _Items;
    DispatchedCollection<EntityType> _Items = new DispatchedCollection<EntityType>();

  }
}
