using System;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace Qhta.WPF.Controls
{
  public class NumericUpDownAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
    {
      ///
      public NumericUpDownAutomationPeer (NumericUpDown owner)
        : base (owner)
      {
      }

      /// 
      override public object GetPattern (PatternInterface patternInterface)
      {
        if (patternInterface == PatternInterface.RangeValue)
          return this;

        return null;
      }

      // 
      [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      internal void RaiseMinimumPropertyChangedEvent (decimal oldValue, decimal newValue)
      {
        RaisePropertyChangedEvent (RangeValuePatternIdentifiers.MinimumProperty, oldValue, newValue);
      }

      // 
      [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      internal void RaiseMaximumPropertyChangedEvent (decimal oldValue, decimal newValue)
      {
        RaisePropertyChangedEvent (RangeValuePatternIdentifiers.MaximumProperty, oldValue, newValue);
      }

      //
      [System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      internal void RaiseValuePropertyChangedEvent (decimal oldValue, decimal newValue)
      {
        RaisePropertyChangedEvent (RangeValuePatternIdentifiers.ValueProperty, oldValue, newValue);
      }

      /// <summary> 
      /// Request to set the value that this UI element is representing
      /// </summary>
      /// <param name="val">Value to set the UI to, as an object</param>
      /// <returns>true if the UI element was successfully set to the specified value</returns> 
      //[CodeAnalysis("AptcaMethodsShouldOnlyCallAptcaMethods")] //Tracking Bug: 29647
      void IRangeValueProvider.SetValue (double value)
      {
        if (!IsEnabled ())
          throw new ElementNotEnabledException ();
      decimal val = (decimal)value;
        NumericUpDown owner = (NumericUpDown)Owner;
        if (val < owner.Minimum || val > owner.Maximum)
        {
          throw new ArgumentOutOfRangeException ("val");
        }

        owner.Value = (decimal)val;
      }


      /// <summary>Value of a value control, as an object</summary>
      double IRangeValueProvider.Value
      {
        get
        {
          return (double)((NumericUpDown)Owner).Value;
        }
      }

      ///<summary>Indicates that the value can only be read, not modified.
      ///returns True if the control is read-only</summary> 
      bool IRangeValueProvider.IsReadOnly
      {
        get
        {
          return !IsEnabled ();
        }
      }

      ///<summary>maximum value </summary> 
      double IRangeValueProvider.Maximum
      {
        get
        {
          return (double)((NumericUpDown)Owner).Maximum;
        }
      }

      ///<summary>minimum value</summary> 
      double IRangeValueProvider.Minimum
      {
        get
        {
          return (double)((NumericUpDown)Owner).Minimum;
        }
      }

      ///<summary>Value of a Large Change</summary> 
      double IRangeValueProvider.LargeChange
      {
        get
        {
          return (double)((NumericUpDown)Owner).SmallChange;
        }
      }

      ///<summary>Value of a Small Change</summary> 
      double IRangeValueProvider.SmallChange
      {
        get
        {
          return (double)((NumericUpDown)Owner).SmallChange;
        }
      }
    }
  }

