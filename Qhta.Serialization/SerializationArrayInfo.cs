using System;
using System.Reflection;
using System.Collections.Generic;

namespace Qhta.Serialization
{
  public class SerializationArrayInfo: SerializationPropertyInfo
  {
    public SerializationArrayInfo(string name, PropertyInfo info, int order): base(name, info, order) { }

    public void Add(string itemName, Type itemType)
    {
      var item = new SerializationItemInfo(itemName, this.PropInfo, itemType);
      Items.Add(itemName, item);
    }

    public KnownTypesDictionary Items { get; private set; } = new ();
  }

}
