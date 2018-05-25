using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MyLib.WpfUtils
{
  public class InputBindingAttachment
  {
    public static readonly DependencyProperty InputBindingsProperty =
        DependencyProperty.RegisterAttached("InputBindings", typeof(InputBindingCollection), typeof(InputBindingAttachment),
        new FrameworkPropertyMetadata(new InputBindingCollection(),
        (sender, e) =>
        {
          var element = sender as UIElement;
          if (element == null) return;
          element.InputBindings.Clear();
          foreach (var inputBinding in ((InputBindingCollection)e.NewValue).Cast<InputBinding>())
          {
            var newInputBinding = (InputBinding)inputBinding.Clone();
            element.InputBindings.Add(newInputBinding);
            newInputBinding.CommandParameter=element;
          }
        }));

    public static InputBindingCollection GetInputBindings(UIElement element)
    {
      return (InputBindingCollection)element.GetValue(InputBindingsProperty);
    }

    public static void SetInputBindings(UIElement element, InputBindingCollection inputBindings)
    {
      element.SetValue(InputBindingsProperty, inputBindings);
    }

  }
}
