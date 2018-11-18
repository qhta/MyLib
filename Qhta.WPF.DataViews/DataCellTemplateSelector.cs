using System.Windows;
using System.Windows.Controls;

namespace Qhta.WPF.DataViews
{
  public class DataCellTemplateSelector : DataTemplateSelector
  {
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      FrameworkElement element = container as FrameworkElement;

      if (element != null && item is PropertyViewModel viewModel)
      {
        //Debug.WriteLine($"SelectDataCellTemplate({item}, {element})");
        if (viewModel.Type.IsEnum)
          return  element.FindResource("EnumDataTemplate") as DataTemplate;
        else
          return  element.FindResource("TextDataTemplate") as DataTemplate;
      }
      return null;
    }
  }

  public class DataEditTemplateSelector : DataTemplateSelector
  {
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      FrameworkElement element = container as FrameworkElement;

      if (element != null && item is PropertyViewModel viewModel)
      {
        //Debug.WriteLine($"SelectDataEditTemplate({item}, {element})");
        if (viewModel.Type.IsEnum)
          return element.FindResource("EnumDataTemplate") as DataTemplate;
        else
          return element.FindResource("TextDataEditingTemplate") as DataTemplate;
      }
      return null;
    }
  }
}
