using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.EFTools
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple =false, Inherited =false)]
  public class DerivedAttribute: Attribute
  {
    public string Property { get; set; }

    public string GetMethod { get; set; }

    public Type TypeConverter { get; set; }
  }
}
