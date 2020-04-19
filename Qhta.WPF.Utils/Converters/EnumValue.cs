using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qhta.WPF.Utils
{
  /// <summary>
  /// Obiekt zastępujący wartość wyliczeniową do wyświetlania narodowych nazw w listach wyboru
  /// </summary>
  public class EnumValue
  {
    /// <summary>
    /// Wartość (przeliczona na int)
    /// </summary>
    public object Value { get; set; }
    /// <summary>
    /// Nazwa wyświetlana
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Konwersja na łańcuch bierze nazwę
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return Name;
    }
    /*
    public static implicit operator EnumType (EnumValue<EnumType> v)
    {
      return v.Value;
    }
    */ 
  }
}
