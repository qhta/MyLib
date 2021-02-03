using Qhta.RegularExpressions;
using Qhta.RegularExpressions.Descriptions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace RegExTaggerTest
{
  [ContentProperty(nameof(Items))]
  public class TestItem
  {
    public string Pattern { get; set; }

    public RegExStatus? Result { get; set; }

    public RegExItems Items { get; set; } = new RegExItems();

    public PatternItems PatternItems { get; set; }

  }
}
