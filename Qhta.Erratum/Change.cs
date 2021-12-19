using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.Erratum
{
  public class Change
  {
    public int Pos { get; init; }
    
    public string? Text {get; init; }

    public string? Replacement { get; init; }

    public Change(int pos, string? text, string? replacement)
    {
      Pos = pos;
      Text = text;
      Replacement = replacement;
    }

  }
}
