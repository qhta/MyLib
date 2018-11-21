
using System.Windows.Media;

namespace Qhta.WPF.Utils
{
  public class KnownColor
  {
    public string Name { get; set; }
    public Color Color { get; set; }

    public override string ToString()
    {
      return $"{Name} ({Color})";
    }

    public static implicit operator Color(KnownColor value)
    {
      return value.Color;
    }
  }
}
