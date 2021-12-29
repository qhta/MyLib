using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.Erratum
{
  public abstract class ErrataEntry
  {
    /// <summary>
    /// Short description for a human.
    /// </summary>
    public string Comment { get; set; }

    public CorrOp Op {get; set; }

  }
}
