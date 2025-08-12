using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

using Qhta.SF.Tools;
using Qhta.UndoManager;
using Qhta.UnicodeBuild.ViewModels;
using Qhta.WPF.Utils;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.Commands;

/// <summary>
/// Provides a set of routed commands for managing user interactions in a data grid.
/// </summary>
/// <remarks>This class contains static members that define routed commands, which can be used to handle specific
/// actions in a data grid, such as filling a column with a selected value. Routed commands are designed to integrate
/// with WPF's command system, enabling separation of UI and logic.</remarks>
public static partial class _Commander
{
  private static SfDataGrid GetDataGrid(object? sender, ICommand command)
  {
    if (sender is SfDataGrid dataGrid)
      return dataGrid;
    if (sender is MainWindow mainWindow)
      sender = mainWindow.TabControl.SelectedContent;
    SfDataGrid? dataGrid1 = null;
    if (sender is UserControl userControl)
      dataGrid1 = VisualTreeHelperExt.FindDescendant<SfDataGrid>(userControl);
    if (dataGrid1 == null)
      throw new ArgumentException($"{command}({sender}): Sender is not a SfDataGrid.", nameof(sender));
    return dataGrid1;
  }

  /// <summary>
  /// Handles the CanExecute event for routed commands, determining whether the command can be executed based on the current state of the application.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  public static void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
  {
    //Debug.WriteLine($"CommandBinding_OnCanExecute({e.Command})");
    var command = e.Command;
    if (command == ApplicationCommands.Save)
      e.CanExecute = _ViewModels.Instance.DbContext?.ThereAreUnsavedChanges ?? false;
    else if (command == ApplicationCommands.Copy)
      e.CanExecute = SfDataGridCommander.CanCopyData(GetDataGrid(sender, command));
    else if (command == ApplicationCommands.Cut)
      e.CanExecute = SfDataGridCommander.CanCutData(GetDataGrid(sender, command));
    else if (command == ApplicationCommands.Paste)
      e.CanExecute = SfDataGridCommander.CanPasteData(GetDataGrid(sender, command));
    else if (command == ApplicationCommands.Delete)
      e.CanExecute = SfDataGridCommander.CanDeleteData(GetDataGrid(sender, command));
    else if (command == ApplicationCommands.Undo)
      e.CanExecute = UndoMgr.IsUndoAvailable;
    else if (command == ApplicationCommands.Redo)
      e.CanExecute = UndoMgr.IsRedoAvailable;
    else if (command == ApplicationCommands.Find)
      e.CanExecute = FindCommand.CanExecute(e.Parameter ?? GetDataGrid(sender, command));
    else if (command == FindNextCommand)
      e.CanExecute = FindCommand.CanExecuteFindNext(e.Parameter ?? GetDataGrid(sender, command));
    else
      e.CanExecute = true; // Default to true for other commands
    //Debug.WriteLine($"_Commander.CommandBinding_OnCanExecute({sender}, {GetName(e.Command)}({e.Parameter}))={e.CanExecute}");
  }

  /// <summary>
  /// Debug helper method to get the name of the command for logging purposes.
  /// </summary>
  /// <param name="command"></param>
  /// <returns></returns>
  private static string? GetName(ICommand command)
  {
    if (command is RoutedCommand routedCommand)
      return routedCommand.Name;
    return command.ToString();
  }

  /// <summary>
  /// Handles the Executed event for routed commands, executing the command based on the current state of the application and user interactions.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  public static void OnExecute(object sender, ExecutedRoutedEventArgs e)
  {
    //Debug.WriteLine($"_Commander.CommandBinding_OnExecute({sender}, {GetName(e.Command)}({e.Parameter}))");
    var command = e.Command;
    if (command == ApplicationCommands.Save)
    {
      Debug.WriteLine("Data changes save started");
      _ViewModels.Instance.DbContext?.SaveChangesAsync();
    }
    else if (command == ApplicationCommands.Copy)
    {
      SfDataGridCommander.CopyData(GetDataGrid(sender, command));
    }
    else if (command == ApplicationCommands.Cut)
    {
      SfDataGridCommander.CutData(GetDataGrid(sender, command));
    }
    else if (command == ApplicationCommands.Paste)
    {
      SfDataGridCommander.PasteData(GetDataGrid(sender, command));
    }
    else if (command == ApplicationCommands.Delete)
    {
      SfDataGridCommander.DeleteData(GetDataGrid(sender, command));
    }
    else if (command == ApplicationCommands.Undo)
    {
      UndoMgr.Undo();
      GetDataGrid(sender, command).UpdateLayout();
    }
    else if (command == ApplicationCommands.Redo)
    {
      UndoMgr.Redo();
      GetDataGrid(sender, command).UpdateLayout();
    }
    else if (command == ApplicationCommands.Find)
    {
      FindCommand.Execute(e.Parameter ?? GetDataGrid(sender, command));
    }
    else if (command == FindNextCommand)
    {
      FindCommand.ExecuteFindNext(e.Parameter ?? GetDataGrid(sender, command));
    }
    else
    {
      Debug.WriteLine($"Command {command} not executed");
    }
  }

  /// <summary>
  /// Updates the execution state of all commands defined as properties in the <see cref="_Commander"/> class.
  /// </summary>
  /// <remarks>This method iterates through all properties of the <see cref="_Commander"/> class that are
  /// derived from  <see cref="Qhta.MVVM.Command"/> and invokes their <see
  /// cref="Qhta.MVVM.Command.NotifyCanExecuteChanged"/>  method to refresh their execution state. This is typically
  /// used to ensure that the commands'  <see cref="Qhta.MVVM.Command.CanExecute"/> logic is re-evaluated.</remarks>
  public static void NotifyCanExecuteChanged()
  {
    foreach (var property in typeof(_Commander).GetProperties())
    {
      if (property.PropertyType.IsSubclassOf(typeof(Qhta.MVVM.Command)))
      {
        //Debug.WriteLine($"Updating CanExecute for command: {property.Name}");
        var command = (Qhta.MVVM.Command)property.GetValue(null)!;
        command.NotifyCanExecuteChanged();
      }
    }
  }

  /// <summary>
  /// Command to fill the current column in the data grid with a selected value.
  /// </summary>
  public static FillColumnCommand FillColumnCommand { get; } = new();

  /// <summary>
  /// Command to find a value selected by the user in the current column in the data grid.
  /// </summary>
  public static FindCommand FindCommand { get; } = new();

  /// <summary>
  /// Command to find a value selected by the user in the current column in the data grid.
  /// </summary>
  public static RoutedCommand FindNextCommand { get; } = new RoutedCommand("FindNext", typeof(_Commander), [new KeyGesture(Key.F3)]);

  /// <summary>
  /// Command to apply writing system mappings from a file.
  /// </summary>
  public static ApplyBlockMappingCommand ApplyBlockMappingCommand { get; } = new();

  /// <summary>
  /// Command to apply writing system mappings from a file.
  /// </summary>
  public static ApplyWritingSystemMappingCommand ApplyWritingSystemMappingCommand { get; } = new();

  /// <summary>
  /// Command to apply writing systems recognition in a set of Unicode code points. 
  /// </summary>
  public static ApplyWritingSystemRecognitionCommand ApplyWritingSystemRecognitionCommand { get; } = new();

  /// <summary>
  /// Command to apply character names generation to selected Unicode code points.
  /// </summary>
  public static ApplyCharNamesGenerationCommand ApplyCharNamesGenerationCommand { get; } = new();

  /// <summary>
  /// Command to mark unused writing systems.
  /// </summary>
  public static MarkUnusedWritingSystemsCommand MarkUnusedWritingSystemsCommand { get; } = new();
}