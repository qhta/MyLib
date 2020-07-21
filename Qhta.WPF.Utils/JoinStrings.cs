using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Qhta.WPF.Utils
{
  [ContentProperty(nameof(Items))]
  public class JoinStrings : MarkupExtension
  {
    public string Separator { get; set; }

    public List<string> Items { get; private set; } = new List<string>();

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return ToString();
    }

    public override string ToString()
    {
      return String.Join(Separator, Items);
    }
  }
}
