namespace Qhta.WPF.Utils;

/// <summary>
/// A class that reports progress in programmable time periods. 
/// It updates a dependency property in an dependency object using target object dispatcher.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ProgressMonitor<T> : IProgress<T> where T: IComparable<T>
{
  /// <summary>
  /// Create for value only update.
  /// </summary>
  /// <param name="target">DependencyObject to be updated on Report</param>
  /// <param name="valueProperty">DependencyProperty of DependencyObject to be updated</param>
  /// <param name="updatePeriod">updatePeriod in milliseconds</param>
  public ProgressMonitor(DependencyObject target, DependencyProperty valueProperty, int updatePeriod = 0)
  {
    Target = target;
    ValueProperty = valueProperty;
    lastValue = default(T);
    updateValue = default(T);
    if (updatePeriod != 0)
    {
      Timer = new System.Threading.Timer(TimerCallback, null, 0, updatePeriod);
    }
  }

  /// <summary>
  /// Create for maximum and current value update.
  /// </summary>
  /// <param name="target">DependencyObject to be updated</param>
  /// <param name="valueProperty">DependencyProperty of DependencyObject to be updated on Report</param>
  /// <param name="maxProperty">DependencyProperty of DependencyObject to be updated on SetMaximum</param>
  /// <param name="updatePeriod">updatePeriod in milliseconds (apply to valueProperty only)</param>
  public ProgressMonitor(DependencyObject target, DependencyProperty valueProperty, DependencyProperty maxProperty, int updatePeriod = 0)
  {
    Target = target;
    ValueProperty = valueProperty;
    MaximumProperty = maxProperty;
    lastValue = default(T);
    updateValue = default(T);
    if (updatePeriod != 0)
    {
      Timer = new System.Threading.Timer(TimerCallback, null, 0, updatePeriod);
    }
  }

  /// <summary>
  /// DependencyObject that is updated.
  /// </summary>
  public readonly DependencyObject? Target;

  /// <summary>
  /// Dependency property that is updated.
  /// </summary>
  public readonly DependencyProperty? ValueProperty;

  /// <summary>
  /// A property with maximum value.
  /// </summary>
  public readonly DependencyProperty? MaximumProperty;

  /// <summary>
  ///  Time period to report progress.
  /// </summary>
  public readonly TimeSpan? UpdatePeriod;
  private T? lastValue;
  private T? updateValue;
  private System.Threading.Timer? Timer = null;

  /// <summary>
  /// Set maximum value using Target Dispatcher.
  /// </summary>
  /// <param name="value"></param>
  /// <exception cref="ArgumentNullException"></exception>
  public void SetMaximum(T value)
  {
    if (MaximumProperty == null)
      throw new ArgumentNullException("MaximumProperty not set in ProgressMonitor");
    Target?.Dispatcher.Invoke(
      () =>
      {
        var binding = BindingOperations.GetBindingExpression(Target, MaximumProperty);
        binding.UpdateTarget();
      }
    );
  }

  /// <summary>
  /// Method to report a valu using Target Dispatcher.
  /// </summary>
  /// <param name="value"></param>
  public void Report(T value)
  {
    if (value.CompareTo(lastValue)!=0)
    {
      lastValue = value;
      if (Timer == null)
      {
        updateValue = value;
        Target?.Dispatcher.Invoke(
          () =>
          {
            var binding = BindingOperations.GetBindingExpression(Target, ValueProperty);
            binding.UpdateTarget();
          }
        );
      }
    }
  }

  private void TimerCallback(object? state)
  {
    if (updateValue!=null && updateValue.CompareTo(lastValue) != 0)
    {
      updateValue = lastValue;
      //Debug.WriteLine($"UpdateValue=={updateValue}");
      Target?.Dispatcher.Invoke(
        () =>
        {
          var binding = BindingOperations.GetBindingExpression(Target, ValueProperty);
          binding.UpdateTarget();
        }
      );
    }
  }
}
