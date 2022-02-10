using System;
using System.Reflection;
using System.Collections.Generic;

namespace Qhta.Serialization
{
  public class SerializationArrayInfo: SerializationPropertyInfo
  {
    public SerializationArrayInfo(string name, PropertyInfo propInfo, int order): 
      base(name, propInfo, order) { }

    //public SerializationItemInfo Add(string itemName, Type itemType)
    //{
    //  var item = new SerializationItemInfo(itemName, this.PropInfo, itemType);
    //  var itemTypeInfo =
    //  KnownItemTypes.Add(itemName, item);
    //  return item;
    //}


    public KnownTypesDictionary KnownItemTypes { get; private set; } = new ();
  }

}
