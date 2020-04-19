using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Qhta.TextUtils;

namespace Qhta.WPF.PropertyGrid
{

  public class PropertyTemplateSelector : DataTemplateSelector
  {

    public PropertyTemplateSelector(FrameworkElement host)
    {
      Host = host;
    }
    FrameworkElement Host;

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      if (item != null)
      {
        var propertyInfo = item as IPropertyViewModel;

        DataTemplate template = null;
        if (propertyInfo.Readonly)
          template = (DataTemplate)Host.TryFindResource("DataTemplate.Property.ReadonlyValueTemplate");
        if (template != null)
          return template;
        var typeName = propertyInfo.TypeName.TitleCase();
        if (String.IsNullOrEmpty(typeName))
          template = (DataTemplate)Host.TryFindResource("DataTemplate.Property.ReadonlyValueTemplate");
        if (template != null)
          return template;
        typeName =  typeName.TitleCase();
        template = (DataTemplate)Host.TryFindResource($"DataTemplate.Property.{typeName}ValueTemplate");
        if (template != null)
          return template;
        template = (DataTemplate)Host.TryFindResource("DataTemplate.Property.ReadonlyValueTemplate");
        if (template != null)
          return template;
      }
      return base.SelectTemplate(item, container);
    }
  }
}
