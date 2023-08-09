using System;
using System.Collections.Generic;
using System.Text;

namespace Qhta.MVVM;

/// <summary>
/// Interface for class which will listen to Command.CanExecuteChanged event.
/// </summary>
public interface ICanExecuteChangedListener
{
  /// <summary>
  /// Listener for CanExecuteChanged event.
  /// </summary>
  public event EventHandler CanExecuteChanged;
}
