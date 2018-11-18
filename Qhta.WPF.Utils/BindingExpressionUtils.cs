using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  public static class BindingExpressionUtils
  {
    public static string GetString(this BindingExpressionBase expression)
    {
      if (expression is BindingExpression expr)
      {
        var str = expr.DataItem?.ToString();
        return str;
      }
      return expression.GetType().Name;
    }
  }
}
