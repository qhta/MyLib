using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyLib.DbUtils
{
  /// <summary>
  /// Klasa tworząca komendę SQL do tworzenia tabeli w bazie danych
  /// MS ACCESS na podstawie tablicy <c>DataTable</c>
  /// </summary>
  public class AccessTableBuilder: SqlTableBuilder
  {
    /// <summary>
    /// Ograniczona długość łańcucha
    /// </summary>
    protected override int MaxStringLength
    {
      get
      {
        return 255;
      }
    }
  }
}
