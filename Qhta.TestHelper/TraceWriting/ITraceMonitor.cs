using System.Runtime.CompilerServices;

namespace Qhta.TestHelper
{
  /// <summary>
  /// The monitor does not write messages. It is used to collect messages and process them in aggregation.
  /// </summary>
  public interface ITraceMonitor
  {
    /// <summary>
    /// Register a message send by a caller.
    /// </summary>
    /// <param name="message">Text message</param>
    /// <param name="callerName">Caller name (by default set as name of caller method)</param>
    void Notify(string message, [CallerMemberName] string? callerName = null);

    /// <summary>
    /// Check if a message was already registered.
    /// Enables omitting duplicate message processing.
    /// </summary>
    /// <param name="message">Text message</param>
    /// <param name="callerName">Caller name (by default set as name of caller method)</param>
    /// <returns>true if the message was already registered</returns>
    bool WasNotified(string message, [CallerMemberName] string? callerName = null);

    /// <summary>
    /// Flush processed data to output
    /// </summary>
    void Flush();

    /// <summary>
    /// Clear registry
    /// </summary>
    void Clear();
  }
}
