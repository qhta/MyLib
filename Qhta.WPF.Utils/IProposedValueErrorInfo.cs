using System.Globalization;

namespace Qhta.WPF.Utils
{
  public interface IProposedValueErrorInfo
  {
    object GetError(string propertyName, object value, CultureInfo cultureInfo);
  }
}
