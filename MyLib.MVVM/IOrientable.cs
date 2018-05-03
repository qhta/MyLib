using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.MVVM
{
  public enum OrientationType
  {
    Horizontal = 0,
    Vertical = 1
  }

  public interface IOrientable
  {
    OrientationType Orientation { get; }
  }
}
