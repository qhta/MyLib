using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Qhta.WPF.Utils
{
  public class ActionHolder
  {
    public Action Action { get; set; }

    public void Execute()
    {
      Action.Invoke();
    }
  }
}
