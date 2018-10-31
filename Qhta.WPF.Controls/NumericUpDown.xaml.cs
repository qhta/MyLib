using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Qhta.WPF.Controls
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
    /// Właściwość zależna <see cref="Increment"/>
    /// </summary>
    public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register
      ("Increment", typeof(decimal), typeof(NumericUpDown),
        //      new PropertyMetadata (0.1));
        new FrameworkPropertyMetadata((decimal)1,
          (DependencyObject sender, DependencyPropertyChangedEventArgs args)=> 
          {
            (sender as NumericUpDown).SmallChange=(decimal)args.NewValue;
            (sender as NumericUpDown).LargeChange=(decimal)args.NewValue;
          }));

    /// <summary>
    /// Właściwość zależna <see cref="SmallChange"/>
    /// </summary>
    public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register
      ("SmallChange", typeof (decimal), typeof (NumericUpDown), 
//      new PropertyMetadata (0.1));
        new FrameworkPropertyMetadata ((decimal)1),
        new ValidateValueCallback (IsValidChange));

    /// <summary>
    /// Wartość małej zmiany
    /// </summary>
    public decimal SmallChange
    {
      get { return (decimal)GetValue (SmallChangeProperty); }
      set { SetValue (SmallChangeProperty, value); }
    }

    /// <summary>
    /// Właściwość zależna <see cref="LargeChange"/>
    /// </summary>
    public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register
      ("LargeChange", typeof (decimal), typeof (NumericUpDown),
      //      new PropertyMetadata (0.1));
        new FrameworkPropertyMetadata ((decimal)1),
        new ValidateValueCallback (IsValidChange));

    /// <summary>
    /// Wartość małej zmiany
    /// </summary>
    public decimal LargeChange
    {
      get { return (decimal)GetValue (LargeChangeProperty); }
      set { SetValue (LargeChangeProperty, value); }
    }

    /// <summary> 
    /// Validate input value in NumericUpDown (SmallChange and LargeChange).
    /// </summary> 
    /// <param name="value"></param> 
    /// <returns>Returns False if value is NaN or NegativeInfinity or PositiveInfinity or negative. Otherwise, returns True.</returns>
    private static bool IsValidChange (object value)
    {
      decimal d = (decimal)value;

      return IsValidDecimalValue (value) && d >= 0;
    }

    /// <summary>
    /// Właściwość zależna wartości maksymalnej
    /// </summary>
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register
      ("Maximum", typeof (decimal), typeof (NumericUpDown), 
        new FrameworkPropertyMetadata (decimal.MaxValue,
                new PropertyChangedCallback (OnMaximumChanged),
                new CoerceValueCallback (CoerceMaximum)),
        new ValidateValueCallback (IsValidDecimalValue));


    private static object CoerceMaximum (DependencyObject d, object value)
    {
      NumericUpDown ctrl = (NumericUpDown)d;
      decimal min = ctrl.Minimum;
      if ((decimal)value < min)
      {
        return min;
      }
      return value;
    }

    /// <summary>
    /// Wartość maksymalna
    /// </summary>
    public decimal Maximum
    {
      get { return (decimal)GetValue (MaximumProperty); }
      set { SetValue (MaximumProperty, value); }
    }

    /// <summary>
    ///     Called when MaximumProperty is changed on "d." 
    /// </summary>
    private static void OnMaximumChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      NumericUpDown ctrl = (NumericUpDown)d;

      //NumericUpDownAutomationPeer peer = UIElementAutomationPeer.FromElement (ctrl) as NumericUpDownAutomationPeer;
      //if (peer != null)
      //{
      //  peer.RaiseMaximumPropertyChangedEvent ((decimal)e.OldValue, (decimal)e.NewValue);
      //}

      ctrl.CoerceValue (ValueProperty);
      ctrl.OnMaximumChanged ((decimal)e.OldValue, (decimal)e.NewValue);
    }

    /// <summary> 
    ///     This method is invoked when the Maximum property changes.
    /// </summary> 
    /// <param name="oldMaximum">The old value of the Maximum property.</param>
    /// <param name="newMaximum">The new value of the Maximum property.</param>
    protected virtual void OnMaximumChanged (decimal oldMaximum, decimal newMaximum)
    {
    }

    /// <summary>
    /// Właściwość zależna wartości minimalnej
    /// </summary>
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register
      ("Minimum", typeof (decimal), typeof (NumericUpDown), 
//      new PropertyMetadata (0));
                        new FrameworkPropertyMetadata ((decimal)0,
                                new PropertyChangedCallback (OnMinimumChanged)),
                        new ValidateValueCallback (IsValidDecimalValue)); 

    /// <summary>
    /// Wartość minimalna
    /// </summary>
    [Bindable (true), Category ("Behavior")]
    public decimal Minimum
    {
      get { return (decimal)GetValue (MinimumProperty); }
      set { SetValue (MinimumProperty, value); }
    }

    /// <summary>
    /// Właściwość zależna wartości
    /// </summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register
      ("Value", typeof (decimal), typeof (NumericUpDown), 
      //new PropertyMetadata (0.0d));
      new FrameworkPropertyMetadata ((decimal)0,
              FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
              new PropertyChangedCallback (OnValueChanged),
              new CoerceValueCallback (ConstrainToRange)),
              new ValidateValueCallback (IsValidDecimalValue));
      
    
    internal static object ConstrainToRange (DependencyObject d, object value)
    {
      NumericUpDown ctrl = (NumericUpDown)d;
      decimal min = ctrl.Minimum;
      decimal v = (decimal)value;
      if (v < min)
      {
        return min;
      }

      decimal max = ctrl.Maximum;
      if (v > max)
      {
        return max;
      }

      return value;
    } 
 


    /// <summary>
    /// Wartość
    /// </summary>
    public decimal Value
    {
      get { return(decimal)GetValue (ValueProperty); }
      set { SetValue (ValueProperty, value); }
    }

    /// <summary> 
    ///     Called when ValueID is changed on "d."
    /// </summary>
    private static void OnValueChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      NumericUpDown ctrl = (NumericUpDown)d;

      //NumericUpDownAutomationPeer peer = UIElementAutomationPeer.FromElement (ctrl) as NumericUpDownAutomationPeer;
      //if (peer != null)
      //{
      //  peer.RaiseValuePropertyChangedEvent ((decimal)e.OldValue, (decimal)e.NewValue);
      //}

      ctrl.OnValueChanged ((decimal)e.OldValue, (decimal)e.NewValue);
    }

    /// <summary> 
    ///     This method is invoked when the Value property changes.
    /// </summary> 
    /// <param name="oldValue">The old value of the Value property.</param>
    /// <param name="newValue">The new value of the Value property.</param>
    protected virtual void OnValueChanged (decimal oldValue, decimal newValue)
    {
      RoutedPropertyChangedEventArgs<decimal> args = new RoutedPropertyChangedEventArgs<decimal> (oldValue, newValue);
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
      ("ValueChanged", RoutingStrategy.Bubble, typeof (RoutedPropertyChangedEventHandler<decimal>), typeof (NumericUpDown));

    /// <summary>
    /// Add / Remove ValueChangedEvent handler
    /// </summary>
    [Category ("Behavior")]
    public event RoutedPropertyChangedEventHandler<decimal> ValueChanged 
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

      //NumericUpDownAutomationPeer peer = UIElementAutomationPeer.FromElement (ctrl) as NumericUpDownAutomationPeer;
      //if (peer != null)
      //{
      //  peer.RaiseMinimumPropertyChangedEvent ((decimal)e.OldValue, (decimal)e.NewValue);
      //}

      ctrl.CoerceValue (MaximumProperty);
      ctrl.CoerceValue (ValueProperty);
      ctrl.OnMinimumChanged ((decimal)e.OldValue, (decimal)e.NewValue);
    }

    /// <summary> 
    ///     This method is invoked when the Minimum property changes.
    /// </summary> 
    /// <param name="oldMinimum">The old value of the Minimum property.</param>
    /// <param name="newMinimum">The new value of the Minimum property.</param>
    protected virtual void OnMinimumChanged (decimal oldMinimum, decimal newMinimum)
    {
    }

    /// <summary>
    /// Validate input value in NumericUpDown (Minimum, Maximum, and Value).
    /// </summary>
    /// <param name="value"></param> 
    /// <returns>Returns False if value is NaN or NegativeInfinity or PositiveInfinity. Otherwise, returns True.</returns>
    private static bool IsValidDecimalValue (object value)
    {
      decimal d = (decimal)value;

      return true;// !(decimal.IsNaN (d) || decimal.IsInfinity (d));
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
        Value = Math.Min((decimal)newValue, Maximum);
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
        Value = Math.Max((decimal)newValue, Minimum);
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
