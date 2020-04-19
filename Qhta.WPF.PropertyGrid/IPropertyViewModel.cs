using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.WPF.PropertyGrid
{
  public interface IPropertyViewModel
  {
    string Name { get; }
    string DisplayName { get; }

    object Value { get; }

    bool Readonly { get; }

    string TypeName { get; }

    string[] Enums { get; }
  }
}
