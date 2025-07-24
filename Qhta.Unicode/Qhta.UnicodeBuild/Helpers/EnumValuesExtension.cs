using System.Diagnostics;
using System.Windows.Markup;

namespace Qhta.UnicodeBuild.Helpers
{
  /// <summary>
  /// Xaml markup extension that provides the values of an enum type.
  /// </summary>
  public class EnumValuesExtension : MarkupExtension
  {
    /// <summary>
    /// Enumeration type for which the values are provided.
    /// </summary>
    public Type EnumType { [DebuggerStepThrough] get; set; } = null!;

    /// <summary>
    /// Method to provide all the values of the specified enumeration type.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return Enum.GetValues(EnumType);
    }
  }
}