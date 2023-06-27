using System;
using System.Windows.Threading;

namespace Qhta.DispatchedObjects
{
  /// <summary>
  /// Interface declaring methods to execute action in application main thread.
  /// </summary>
  public interface IDispatcherBridge
  {
    /// <summary>
    /// Gets access to Windows Application Dispatcher.
    /// </summary>
    /// <returns></returns>
    void Invoke (Action action);
  }
}
