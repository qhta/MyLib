using System.Collections.Generic;

namespace Qhta.WPF.Utils
{
  public interface ICompoundItem
  {
    IEnumerable<object> Items { get; }
  }
}
