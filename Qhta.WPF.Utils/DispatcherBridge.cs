﻿namespace Qhta.WPF.Utils;

/// <summary>
/// A class that implements <see cref="IDispatcherBridge"/> in WPF applications
/// </summary>
public class DispatcherBridge: IDispatcherBridge
{
  private Dispatcher Dispatcher { get; }

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="dispatcher"></param>
  public DispatcherBridge(Dispatcher dispatcher)
  {
    Dispatcher = dispatcher;
  }

  /// <summary>
  ///  Executes the specified delegate asynchronously 
  ///  on the thread that the System.Windows.Threading.Dispatcher was created on.
  /// </summary>
  /// <param name="method">The delegate to a method that takes parameters specified in args, which is pushed
  ///     onto the Dispatcher event queue.
  /// </param>
  public void BeginInvoke(Delegate method) => Dispatcher.BeginInvoke(method);

  /// <summary>
  ///  Executes the specified delegate asynchronously 
  ///  on the thread that the Dispatcher was created on.
  /// </summary>
  /// <param name="method">The delegate to a method that takes parameters specified in args, which is pushed
  /// onto the Dispatcher event queue.</param>
  /// <param name="args">An array of objects to pass as arguments to the given method.</param>
  public void BeginInvoke(Delegate method, object args) => Dispatcher.BeginInvoke(method, args);

  /// <summary>
  ///  Executes the specified delegate asynchronously 
  ///  on the thread that the Dispatcher was created on.
  /// </summary>
  /// <param name="method">The delegate to a method that takes parameters specified in args, which is pushed
  /// onto the Dispatcher event queue.</param>
  /// <param name="sender">Sender object to pass as the first argument to the given method</param>
  /// <param name="args">An array of objects to pass as arguments to the given method.</param>
  public void BeginInvoke(Delegate method, object sender, object args) => Dispatcher.BeginInvoke(DispatcherPriority.Background, method, sender, args);

  /// <summary>
  /// Executes the specified delegate synchronously on the thread the Dispatcher is associated with.
  /// </summary>
  /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed
  ///  onto the Dispatcher event queue.</param>
  /// <returns>The return value from the delegate being invoked or null if the delegate has no return value</returns>
  public object Invoke(Delegate method) => Dispatcher.Invoke(method);

  /// <summary>
  /// Executes the specified delegate with the specified arguments synchronously on the thread the Dispatcher is associated with.
  /// </summary>
  /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed
  ///  onto the Dispatcher event queue.</param>
  /// <param name="args">An array of objects to pass as arguments to the given method.</param>
  /// <returns>The return value from the delegate being invoked or null if the delegate has no return value</returns>
  public object Invoke(Delegate method, object args) => Dispatcher.Invoke(method, args);

  /// <summary>
  /// Executes the specified delegate with the specified arguments synchronously on the thread the Dispatcher is associated with.
  /// </summary>
  /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed
  ///  onto the Dispatcher event queue.</param>
  /// <param name="sender">Sender object to pass as the first argument to the given method</param>
  /// <param name="args">An array of objects to pass as arguments to the given method.</param>
  /// <returns>The return value from the delegate being invoked or null if the delegate has no return value</returns>
  public object Invoke(Delegate method, object sender, object args) => Dispatcher.Invoke(DispatcherPriority.Send, method, sender, args);

  /// <summary>
  ///  Executes the specified System.Action synchronously on the thread the Dispatcher is associated with.
  /// </summary>
  /// <param name="callback">A delegate to invoke through the dispatcher.</param>
  public void Invoke(Action callback) => Dispatcher.Invoke(callback);

  /// <summary>
  ///  Executes the specified System.Func synchronously on the thread the Dispatcher is associated with.
  /// </summary>
  /// <param name="callback">A delegate to invoke through the dispatcher.</param>
  /// <returns>The value returned by callback.</returns>
  public TResult Invoke<TResult>(Func<TResult> callback) => Dispatcher.Invoke<TResult>(callback);
}


