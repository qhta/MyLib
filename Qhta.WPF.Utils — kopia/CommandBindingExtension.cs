namespace Qhta.WPF.Utils;

/// <summary>
/// Markup extension class that defines a commandName for a Command XAML definition.
/// </summary>
[MarkupExtensionReturnType(typeof(ICommand))]
public class CommandBindingExtension : MarkupExtension
{

  /// <summary>
  /// Default constructor
  /// </summary>
  public CommandBindingExtension()
  {
  }

  /// <summary>
  /// Initializing constructor
  /// </summary>
  /// <param name="commandName"></param>
  public CommandBindingExtension(string commandName)
  {
    this.CommandName = commandName;
  }

  /// <summary>
  /// Constructor argument.
  /// </summary>
  [ConstructorArgument("commandName")]
  public string? CommandName { get; set; }

  /// <summary>
  /// Defines the target object to which the command is assigned.
  /// </summary>
  private object? targetObject;

  /// <summary>
  /// Defines the target property to which the command is assigned.
  /// </summary>
  private object? targetProperty;

  /// <summary>
  /// Provides a Command using the service provider.
  /// If there is some problem, it provides a DummyCommand.
  /// </summary>
  /// <param name="serviceProvider"></param>
  /// <returns></returns>
  public override object ProvideValue(IServiceProvider serviceProvider)
  {
    if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget provideValueTarget)
    {
      targetObject = provideValueTarget.TargetObject;
      targetProperty = provideValueTarget.TargetProperty;
    }

    if (!string.IsNullOrEmpty(CommandName))
    {
      // The serviceProvider is actually a ProvideValueServiceProvider, which has a private field "_context"of type ParserContext
      var parserContext = GetPrivateFieldValue<ParserContext>(serviceProvider, "_context");
      if (parserContext != null)
      {
        // A ParserContext has a private field "_rootElement", which returns the root element of the XAML file
        FrameworkElement? rootElement = GetPrivateFieldValue<FrameworkElement>(parserContext, "_rootElement");
        if (rootElement != null)
        {
          // Now we can retrieve the DataContext
          object dataContext = rootElement.DataContext;

          // The DataContext may not be set yet when the FrameworkElement is first created, and it may change afterwards,
          // so we handle the DataContextChanged event to update the Command when needed
          if (!dataContextChangeHandlerSet)
          {
            rootElement.DataContextChanged += new DependencyPropertyChangedEventHandler(RootElement_DataContextChanged);
            dataContextChangeHandlerSet = true;
          }

          if (dataContext != null)
          {
            ICommand? command = GetCommand(dataContext, CommandName);
            if (command != null)
              return command;
          }
        }
      }
    }

    // The Command property of an InputBinding cannot be null, so we return a dummy extension instead
    return DummyCommand.Instance;
  }

  /// <summary>
  /// Gets a Command for a command name. Can return null.
  /// </summary>
  /// <param name="dataContext"></param>
  /// <param name="commandName"></param>
  /// <returns></returns>
  private ICommand? GetCommand(object dataContext, string commandName)
  {
    PropertyInfo? prop = dataContext.GetType().GetProperty(commandName);
    if (prop != null)
    {
      ICommand? command = prop.GetValue(dataContext, null) as ICommand;
      if (command != null)
        return command;
    }
    return null;
  }

  /// <summary>
  /// Assignes a command to the target property of the target object.
  /// </summary>
  /// <param name="command"></param>
  private void AssignCommand(ICommand command)
  {
    if (targetObject != null && targetProperty != null)
    {
      if (targetProperty is DependencyProperty)
      {
        var depObj = targetObject as DependencyObject;
        var depProp = targetProperty as DependencyProperty;
        if (depObj != null && depProp != null)
          depObj.SetValue(depProp, command);
      }
      else
      {
        PropertyInfo? prop = targetProperty as PropertyInfo;
        if (prop != null)
          prop.SetValue(targetObject, command, null);
      }
    }
  }

  private bool dataContextChangeHandlerSet;

  /// <summary>
  /// Handler for DataContextChanged event of the rpot element.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  private void RootElement_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
  {
    FrameworkElement? rootElement = sender as FrameworkElement;
    if (rootElement != null)
    {
      object dataContext = rootElement.DataContext;
      if (dataContext != null && !String.IsNullOrEmpty(CommandName))
      {
        ICommand? command = GetCommand(dataContext, CommandName);
        if (command != null)
        {
          AssignCommand(command);
        }
      }
    }
  }

  /// <summary>
  /// Gets a private field value of the target object. Can return null.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="target"></param>
  /// <param name="fieldName"></param>
  /// <returns></returns>
  private T? GetPrivateFieldValue<T>(object target, string fieldName)
  {
    FieldInfo? field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
    if (field != null)
    {
      return (T?)field.GetValue(target);
    }
    return default(T);
  }

  /// <summary>
  /// A dummy command that does nothing
  /// </summary>
  private class DummyCommand : Command
  {

    #region Singleton pattern

    private DummyCommand()
    {
    }

    private static DummyCommand? _instance = null;
    public static DummyCommand Instance
    {
      get
      {
        if (_instance == null)
        {
          _instance = new DummyCommand();
        }
        return _instance;
      }
    }

    public override void Execute(object? parameter)
    {
      // does nothing.
    }

    #endregion

  }
}