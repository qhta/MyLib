using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.WPF.PropertyGrid
{
  public interface IPropertiesHostViewModel
  {
    PropertiesCollection Properties { get; }
  }
}
