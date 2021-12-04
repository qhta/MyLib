using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.Erratum
{
  public struct ErrEntry
  {
    public string Exp;
    public string Found;
    public string Repl;
    public int? Length;
    public int? Skip;
  }

  public struct ErrKey
  {
    public string Exp;
    public string Found;
  }

  public struct ErrVal
  {
    public int? Length;
    public string Repl;
  }

  public class ErrList: List<ErrEntry>
  {
  }
}
