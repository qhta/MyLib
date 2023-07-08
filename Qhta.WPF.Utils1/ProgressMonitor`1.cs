using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  public class ProgressMonitor<T> : IProgress<T> where T: IComparable<T>
  {
    /// <summary>
    /// Create for value only update.
    /// </summary>
    /// <param name="object">DependencyObject to be updated on Report</param>
    /// <param name="valueProperty">DependencyProperty of DependencyObject to be updated</param>
    /// <param name="updatePeriod">updatePeriod in milliseconds</param>
    public ProgressMonitor(DependencyObject @object, DependencyProperty valueProperty, int updatePeriod = 0)
    {
      Object = @object;
      ValueProperty = valueProperty;
      lastValue = default(T);
      updateValue = default(T);
      if (updatePeriod != 0)
      {
        Timer = new Timer(TimerCallback, null, 0, updatePeriod);
      }
    }

    /// <summary>
    /// Create for maximum and current value update.
    /// </summary>
    /// <param name="object">DependencyObject to be updated</param>
    /// <param name="valueProperty">DependencyProperty of DependencyObject to be updated on Report</param>
    /// <param name="maxProperty">DependencyProperty of DependencyObject to be updated on SetMaximum</param>
    /// <param name="updatePeriod">updatePeriod in milliseconds (apply to valueProperty only)</param>
    public ProgressMonitor(DependencyObject @object, DependencyProperty valueProperty, DependencyProperty maxProperty, int updatePeriod = 0)
    {
      Object = @object;
      ValueProperty = valueProperty;
      MaximumProperty = maxProperty;
      lastValue = default(T);
      updateValue = default(T);
      if (updatePeriod != 0)
      {
        Timer = new Timer(TimerCallback, null, 0, updatePeriod);
      }
    }

    public readonly DependencyObject Object;
    public readonly DependencyProperty ValueProperty;
    public readonly DependencyProperty MaximumProperty;
    public readonly TimeSpan UpdatePeriod;
    private T lastValue;
    private T updateValue;
    private Timer Timer = null;

    public void SetMaximum(T value)
    {
      if (MaximumProperty == null)
        throw new ArgumentNullException("MaximumProperty not set in ProgressMonitor");
      Object.Dispatcher.Invoke(
        () =>
        {
          var binding = BindingOperations.GetBindingExpression(Object, MaximumProperty);
          binding.UpdateTarget();
        }
      );
    }

    public void Report(T value)
    {
      if (value.CompareTo(lastValue)!=0)
      {
        lastValue = value;
        if (Timer == null)
        {
          updateValue = value;
          Object.Dispatcher.Invoke(
            () =>
            {
              var binding = BindingOperations.GetBindingExpression(Object, ValueProperty);
              binding.UpdateTarget();
            }
          );
        }
      }
    }

    private void TimerCallback(object state)
    {
      if (lastValue.CompareTo(updateValue) != 0)
      {
        updateValue = lastValue;
        //Debug.WriteLine($"UpdateValue=={updateValue}");
        Object.Dispatcher.Invoke(
          () =>
          {
            var binding = BindingOperations.GetBindingExpression(Object, ValueProperty);
            binding.UpdateTarget();
          }
        );
      }
    }
  }
}
