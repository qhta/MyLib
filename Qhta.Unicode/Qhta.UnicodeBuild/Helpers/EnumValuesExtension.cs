using System;
using System.Windows.Markup;

namespace Qhta.UnicodeBuild.Helpers
{
  public class EnumValuesExtension : MarkupExtension
  {
    public Type EnumType { get; set; } = null!;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return Enum.GetValues(EnumType);
    }
  }
}