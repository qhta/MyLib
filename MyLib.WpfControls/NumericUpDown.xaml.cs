using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Automation.Peers;

namespace MyLib.WPFControls
{
  /// <summary>
  /// Kontrolka zmieniająca wartość liczbową w górę i w dół
  /// </summary>
  public partial class NumericUpDown : UserControl
  {

    static NumericUpDown()
    {
      CommandManager.RegisterClassCommandBinding(typeof(NumericUpDown),
        new CommandBinding(NumericUpDown.IncrementCommand,
          new ExecutedRoutedEventHandler(OnIncrementCommand), new CanExecuteRoutedEventHandler(OnQueryIncrementCommand)));
      CommandManager.RegisterClassCommandBinding(typeof(NumericUpDown),
        new CommandBinding(NumericUpDown.DecrementCommand,
          new ExecutedRoutedEventHandler(OnDecrementCommand), new CanExecuteRoutedEventHandler(OnQueryDecrementCommand)));
    }


    public NumericUpDown ()
    {
      InitializeComponent ();
    }

    public override void OnApplyTemplate ()
    {
      base.OnApplyTemplate ();
      UpButton.Command = IncrementCommand;
      DownButton.Command = DecrementCommand;
    }

    /// <summary>
    /// Właściwość zależna <see cref="SmallChange"/>
    /// </summary>
    public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register
      ("SmallChange", typeof (double), typeof (NumericUpDown), 
//      new PropertyMetadata (0.1));
        new FrameworkPropertyMetadata (0.1),
        new ValidateValueCallback (IsValidChange));

    /// <summary>
    /// Wartość małej zmiany
    /// </summary>
    public double SmallChange
    {
      get { return (double)GetValue (SmallChangeProperty); }
      set { SetValue (SmallChangeProperty, value); }
    }

    /// <summary>
    /// Właściwość zależna <see cref="LargeChange"/>
    /// </summary>
    public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register
      ("LargeChange", typeof (double), typeof (NumericUpDown),
      //      new PropertyMetadata (0.1));
        new FrameworkPropertyMetadata (0.1),
        new ValidateValueCallback (IsValidChange));

    /// <summary>
    /// Wartość małej zmiany
    /// </summary>
    public double LargeChange
    {
      get { return (double)GetValue (LargeChangeProperty); }
      set { SetValue (LargeChangeProperty, value); }
    }

    /// <summary> 
    /// Validate input value in NumericUpDown (SmallChange and LargeChange).
    /// </summary> 
    /// <param name="value"></param> 
    /// <returns>Returns False if value is NaN or NegativeInfinity or PositiveInfinity or negative. Otherwise, returns True.</returns>
    private static bool IsValidChange (object value)
    {
      double d = (double)value;

      return IsValidDoubleValue (value) && d >= 0.0;
    }

    /// <summary>
    /// Właściwość zależna wartości maksymalnej
    /// </summary>
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register
      ("Maximum", typeof (double), typeof (NumericUpDown), 
//      new PropertyMetadata (1));
        new FrameworkPropertyMetadata (
                Double.MaxValue,
                new PropertyChangedCallback (OnMaximumChanged),
                new CoerceValueCallback (CoerceMaximum)),
        new ValidateValueCallback (IsValidDoubleValue));


    private static object CoerceMaximum (DependencyObject d, object value)
    {
      NumericUpDown ctrl = (NumericUpDown)d;
      double min = ctrl.Minimum;
      if ((double)value < min)
      {
        return min;
      }
      return value;
    }

    /// <summary>
    /// Wartość maksymalna
    /// </summary>
    public double Maximum
    {
      get { return (double)GetValue (MaximumProperty); }
      set { SetValue (MaximumProperty, value); }
    }

    /// <summary>
    ///     Called when MaximumProperty is changed on "d." 
    /// </summary>
    private static void OnMaximumChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      NumericUpDown ctrl = (NumericUpDown)d;

      NumericUpDownAutomationPeer peer = UIElementAutomationPeer.FromElement (ctrl) as NumericUpDownAutomationPeer;
      if (peer != null)
      {
        peer.RaiseMaximumPropertyChangedEvent ((double)e.OldValue, (double)e.NewValue);
      }

      ctrl.CoerceValue (ValueProperty);
      ctrl.OnMaximumChanged ((double)e.OldValue, (double)e.NewValue);
    }

    /// <summary> 
    ///     This method is invoked when the Maximum property changes.
    /// </summary> 
    /// <param name="oldMaximum">The old value of the Maximum property.</param>
    /// <param name="newMaximum">The new value of the Maximum property.</param>
    protected virtual void OnMaximumChanged (double oldMaximum, double newMaximum)
    {
    }

    /// <summary>
    /// Właściwość zależna wartości minimalnej
    /// </summary>
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register
      ("Minimum", typeof (double), typeof (NumericUpDown), 
//      new PropertyMetadata (0));
                        new FrameworkPropertyMetadata (
                                0.0d,
                                new PropertyChangedCallback (OnMinimumChanged)),
                        new ValidateValueCallback (IsValidDoubleValue)); 

    /// <summary>
    /// Wartość minimalna
    /// </summary>
    [Bindable (true), Category ("Behavior")]
    public double Minimum
    {
      get { return (double)GetValue (MinimumProperty); }
      set { SetValue (MinimumProperty, value); }
    }

    /// <summary>
    /// Właściwość zależna wartości
    /// </summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register
      ("Value", typeof (double), typeof (NumericUpDown), 
      //new PropertyMetadata (0.0d));
      new FrameworkPropertyMetadata (
              0.0d,
              FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
              new PropertyChangedCallback (OnValueChanged),
              new CoerceValueCallback (ConstrainToRange)),
              new ValidateValueCallback (IsValidDoubleValue));
      
    
    internal static object ConstrainToRange (DependencyObject d, object value)
    {
      NumericUpDown ctrl = (NumericUpDown)d;
      double min = ctrl.Minimum;
      double v = (double)value;
      if (v < min)
      {
        return min;
      }

      double max = ctrl.Maximum;
      if (v > max)
      {
        return max;
      }

      return value;
    } 
 


    /// <summary>
    /// Wartość
    /// </summary>
    public double Value
    {
      get { return(double)GetValue (ValueProperty); }
      set { SetValue (ValueProperty, value); }
    }

    /// <summary> 
    ///     Called when ValueID is changed on "d."
    /// </summary>
    private static void OnValueChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      NumericUpDown ctrl = (NumericUpDown)d;

      NumericUpDownAutomationPeer peer = UIElementAutomationPeer.FromElement (ctrl) as NumericUpDownAutomationPeer;
      if (peer != null)
      {
        peer.RaiseValuePropertyChangedEvent ((double)e.OldValue, (double)e.NewValue);
      }

      ctrl.OnValueChanged ((double)e.OldValue, (double)e.NewValue);
    }

    /// <summary> 
    ///     This method is invoked when the Value property changes.
    /// </summary> 
    /// <param name="oldValue">The old value of the Value property.</param>
    /// <param name="newValue">The new value of the Value property.</param>
    protected virtual void OnValueChanged (double oldValue, double newValue)
    {
      RoutedPropertyChangedEventArgs<double> args = new RoutedPropertyChangedEventArgs<double> (oldValue, newValue);
      args.RoutedEvent = NumericUpDown.ValueChangedEvent;
      RaiseEvent (args);

      DependencyPropertyChangedEventArgs args2 = new DependencyPropertyChangedEventArgs (ValueProperty, oldValue, newValue);
      OnPropertyChanged (args2);
    }

    #region Events
    /// <summary>
    /// Event correspond to Value changed event 
    /// </summary> 
    public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent 
      ("ValueChanged", RoutingStrategy.Bubble, typeof (RoutedPropertyChangedEventHandler<double>), typeof (NumericUpDown));

    /// <summary>
    /// Add / Remove ValueChangedEvent handler
    /// </summary>
    [Category ("Behavior")]
    public event RoutedPropertyChangedEventHandler<double> ValueChanged 
    { 
      add { AddHandler (ValueChangedEvent, value); } 
      remove { RemoveHandler (ValueChangedEvent, value); } 
    }
    #endregion Events 


    /// <summary> 
    ///     Called when MinimumProperty is changed on "d."
    /// </summary> 
    private static void OnMinimumChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      NumericUpDown ctrl = (NumericUpDown)d;

      NumericUpDownAutomationPeer peer = UIElementAutomationPeer.FromElement (ctrl) as NumericUpDownAutomationPeer;
      if (peer != null)
      {
        peer.RaiseMinimumPropertyChangedEvent ((double)e.OldValue, (double)e.NewValue);
      }

      ctrl.CoerceValue (MaximumProperty);
      ctrl.CoerceValue (ValueProperty);
      ctrl.OnMinimumChanged ((double)e.OldValue, (double)e.NewValue);
    }

    /// <summary> 
    ///     This method is invoked when the Minimum property changes.
    /// </summary> 
    /// <param name="oldMinimum">The old value of the Minimum property.</param>
    /// <param name="newMinimum">The new value of the Minimum property.</param>
    protected virtual void OnMinimumChanged (double oldMinimum, double newMinimum)
    {
    }

    /// <summary>
    /// Validate input value in NumericUpDown (Minimum, Maximum, and Value).
    /// </summary>
    /// <param name="value"></param> 
    /// <returns>Returns False if value is NaN or NegativeInfinity or PositiveInfinity. Otherwise, returns True.</returns>
    private static bool IsValidDoubleValue (object value)
    {
      double d = (double)value;

      return !(double.IsNaN (d) || double.IsInfinity (d));
    }

    /// <summary> 
    /// Increment Value
    /// </summary> 
    public static readonly RoutedCommand IncrementCommand = new RoutedCommand ("Increment", typeof (NumericUpDown));
    /// <summary>
    /// Decrement Value
    /// </summary>
    public static readonly RoutedCommand DecrementCommand = new RoutedCommand ("Decrement", typeof (NumericUpDown));

    private bool IsBindedControlEnabled
    {
      get
      {
        BindingExpression aBinding = GetBindingExpression (ValueProperty);
        if (aBinding == null)
          return true;
        if (aBinding.DataItem is Control)
          return (aBinding.DataItem as Control).IsEnabled;
        return true;
      }
    }

    private static void OnQueryIncrementCommand (object target, CanExecuteRoutedEventArgs args)
    {
      args.CanExecute = (target as NumericUpDown).IsBindedControlEnabled;
    } 

    private static void OnIncrementCommand (object target, ExecutedRoutedEventArgs args)
    {
      (target as NumericUpDown).Increment ();
    }

    public void Increment (bool large=false)
    {
      decimal newValue = (decimal)Value + (large ? (decimal)LargeChange : (decimal)SmallChange);
      if ((decimal)Value != newValue)
      {
        Value = Math.Min((double)newValue, Maximum);
      } 
    }

    private static void OnQueryDecrementCommand (object target, CanExecuteRoutedEventArgs args)
    {
      args.CanExecute = (target as NumericUpDown).IsBindedControlEnabled;
    }

    private static void OnDecrementCommand (object target, ExecutedRoutedEventArgs args)
    {
      (target as NumericUpDown).Decrement ();
    }

    public void Decrement (bool large=false)
    {
      decimal newValue = (decimal)Value - (large ? (decimal)LargeChange : (decimal)SmallChange);
      if ((decimal)Value != newValue)
      {
        Value = Math.Max((double)newValue, Minimum);
      } 
    }

    private void UpButton_Click (object sender, RoutedEventArgs e)
    {
      Increment (Keyboard.IsKeyDown (Key.LeftShift) || Keyboard.IsKeyDown (Key.LeftShift));
      e.Handled = true;
    }

    private void DownButton_Click (object sender, RoutedEventArgs e)
    {
      Decrement (Keyboard.IsKeyDown (Key.LeftShift) || Keyboard.IsKeyDown (Key.LeftShift));
      e.Handled = true;
    }

  }
}
