namespace Qhta.UnicodeBuild.Helpers;

using PropertyTools.Wpf;

public class PropertyModelOperator : PropertyGridOperator
{
  public override IEnumerable<Tab> CreateModel(object instance, bool isEnumerable, IPropertyGridOptions options)
  {
    return base.CreateModel(instance, isEnumerable, options);
  }
}