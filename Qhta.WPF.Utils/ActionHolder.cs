namespace Qhta.WPF.Utils
{
  /// <summary>
  /// A class that holds an action.
  /// </summary>
  public class ActionHolder
  {
    /// <summary>
    /// Action to hold and execute.
    /// </summary>
    public Action? Action { get; set; }

    /// <summary>
    /// Executes the action.
    /// </summary>
    public void Execute()
    {
      Action?.Invoke();
    }
  }
}
