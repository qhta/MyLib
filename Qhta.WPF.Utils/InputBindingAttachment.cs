namespace Qhta.WPF.Utils;

/// <summary>
/// Behavior class to define an InputBinding property that can be attached to an UI element.
/// </summary>
public class InputBindingAttachment
{
  /// <summary>
  /// Getter of InputBinding property.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static InputBindingCollection GetInputBindings(UIElement element)
  {
    return (InputBindingCollection)element.GetValue(InputBindingsProperty);
  }

  /// <summary>
  /// Setter of InputBinding property
  /// </summary>
  /// <param name="element"></param>
  /// <param name="inputBindings"></param>
  public static void SetInputBindings(UIElement element, InputBindingCollection inputBindings)
  {
    element.SetValue(InputBindingsProperty, inputBindings);
  }

  /// <summary>
  /// Dependency property to store InputBinding property.
  /// </summary>
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
}
