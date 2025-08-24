Package Qhta.UndoManager is a library implementing Undo/Redo mechanism by registering actions.


The package contains:
- IAction interface as the basic definition for Undo/Redo functionality,
- UndoRedoEntry as item type of Undo/Redo stack,
- UndoRedoManager class that records actions and executes Undo/Redo functionality,
- ChangePropertyAction as the basic implementation of IAction that changes the property,
- ActionGroup as the specific action used for action grouping.

# IAction interface

IAction interface defines two public methods:
- Execute(object? args) - to redo the action,
- Undo(object? args) - to undo the action.

Both methods have very generic arguments.

# UndoRedoEntry record

UndoRedo record is used as an item type for storing actions and their arguments. It has two properties:
- Action of IAction type,
- Args as object? type.

# UndoRedoManager class

UndoRedoManager is a static class that records actions and enables their Undo/Redo operations.
It declares two stacks of UndoRedoEntry items:
- UndoStack and
- RedoStack.

UndoStack is used to register actions. RedoStack registers actions that were undo.
UndoRedoManager declares three main methods:
- Register - to store actions in UndoStack,
- Undo - to perform undo operations on actions stored in UndoStack and move them to RedoStack,
- Redo - to perform redo operations on actions stored in RedoStack and move them to UndoStack.

There are cases when multiple actions should be undone as one. For these cases UndoRedoManager declares two methods:
- StartGrouping,
- StopGrouping.

All actions registered between StartGrouping and StopGrouping methods are stored in an ActionGroup special action.

The functionality of UndoRedoManager is controlled by several boolean properties:
- IsEnabled - that determines whether the Record/Undo/Redo functionality is enabled,
- IsUndoAvailable - flag to indicate that the Undo functionality is available,
- IsUndoAvailable - flag to indicate that the Redo functionality is available,
- IsRecordingAvailable - flag to indicate that recording is available,
- IsGroupingAvailable - flag to indicate that grouping is available,
- IsUndoing - flag to indicate that manager is currently undoing an action,
- IsRedoing - flag to indicate that manager is currently redoing an action,
- IsRecording - flag to indicate that manager is currently recording an action,
- IsGrouping - flag to indicate that manager is currently grouping actions.

# ChangePropertyAction

ChangePropertyAction is a class implementing IAction interface. It Execute and Undo methods require argument of ChangePropertyArgs type.
This argument record provides four properties:
- Target that specifies and object which property is changed,
- PropertyName that specifies the name of changed property,
- OldValue that holds the previous value of the property,
- NewValue that specifies the new value of the property.

Execute and Undo methods use type reflection to change the property value.

# ActionGroup

Action group is a class implementing IAction that is used for grouping actions. 
Is declares an internal Group field that stored a collection of recorded actions.
In Undo method the actions in the Group are undone in the reverse order.
In Execute method the actions in the Group are executed in the originally recorded order.

# Example

Action grouping is currently used in Qhta.SF.WPF.Tools in clipboard operations, delete and fill commands.

## DataOp sample

Below is a fragment of ExecuteDataOp method that implements delete operation in SfDataGrid.
First, UndoRedoManager StartGrouping method is invoked, as in case of undo we must undo all changes.
Next, we choose DeleterRecords or DeleteCells method.
Finally, UndoRedoManager StopGrouping method is invoked,

```csharp
      if (op == DataOp.Delete)
      {
        UndoRedoManager.StartGrouping();
        if (allColumnsSelected)
          DeleteRecords(dataGrid, selectedRows);
        else
          DeleteCells(selectedColumns, columnInfos, selectedRows);
        UndoRedoManager.StopGrouping();
      }
```

### DeleteCells method

Here is the code of DeleteCells method. We just set cell data to null.

```csharp
  private static void DeleteCells(GridColumn[] selectedColumns, GridColumnInfo?[]? columnInfos, object[] selectedRows)
  {
    foreach (var row in selectedRows)
    {
      foreach (var column in selectedColumns)
      {
        var cellInfo = new GridCellInfo(column, row, null, -1, false);
        var columnInfo = columnInfos?.FirstOrDefault(info => info?.MappingName == column.MappingName);
        if (columnInfo != null)
        {
          SetCellData(cellInfo, columnInfo, null);
        }
      }
    }
  }
```
### SetCellData method

SetCellData method is rather complicated as we must consider various data types. We don't need to analyze it, 
as we don't use UndoRedManager here, but we move action registering to set method of the cleared property (further below).

```csharp
  private static void SetCellData(GridCellInfo cellInfo, GridColumnInfo columnInfo, string? str)
  {
    var rowData = cellInfo.RowData;
    if (rowData == null)
    {
      Debug.WriteLine("Row data is null.");
      return;
    }
    var propertyInfo = columnInfo.ValuePropertyInfo;
    if (!propertyInfo.CanWrite)
      return;
    object? value = str;
    if (columnInfo.ItemsSource!=null && !String.IsNullOrEmpty(str))
    {
      if (columnInfo.DisplayPropertyInfo != null)
        value = columnInfo.ItemsSource.Cast<object>()
                  .FirstOrDefault(item => columnInfo.DisplayPropertyInfo.GetValue(item)?.ToString() == str);
      else
        // If no display property, use the ToString() method of the item
        value = columnInfo.ItemsSource.Cast<object>()
          .FirstOrDefault(item => item.ToString() == str);
      if (value==null)
      {
        Debug.WriteLine($"Value '{str}' not found in items source for column '{columnInfo.MappingName}'.");
        return;
      }
    }
    if (value==null && propertyInfo.PropertyType.IsValueType)
    {
      // If the property is a value type, we cannot set it to null
      // Set it to the default value of the type
      value = Activator.CreateInstance(propertyInfo.PropertyType);
    }
    else if (value == null && propertyInfo.PropertyType != typeof(string) && !propertyInfo.PropertyType.IsNullable() && !propertyInfo.PropertyType.IsClass)
    {
      // If the property is not a string, and we have a null value, we cannot set it
      Debug.WriteLine($"Cannot set null value for non-string property '{columnInfo.MappingName}'.");
      return;
    }
    if (value != null && !propertyInfo.PropertyType.IsInstanceOfType(value))
    {
      // If the value is not of the correct type, we need to convert it
      try
      {
        value = Convert.ChangeType(value, propertyInfo.PropertyType);
      }
      catch (InvalidCastException ex)
      {
        Debug.WriteLine($"Cannot convert value '{value}' to type '{propertyInfo.PropertyType.Name}': {ex.Message}");
        return;
      }
    }
    propertyInfo.SetValue(rowData, value);
  }
```

### ModelView property

In an application, we used MVVM framework and EntityFramework to access to data records.
A ViewModel class has a Model property, which expressed data record entity. 
A sample accessords of ViewModel Type property gets the value from model's Type property,
but is uses ChangeEntityProperty method to set the Type value.

```csharp
  public WritingSystemType? Type
  {
    [DebuggerStepThrough]
    get => Model.Type;
    set => ChangeEntityProperty(nameof(Type), nameof(Model.Type), value);
  }
```

### ChangeEntityProperty method

ChangeEntityProperty methods requires three parameters:
- thisPropertyName - that expresses the name of the ViewModel's property,
- modelPropertyName - that expresses the name of the Model's property (they can be the same),
- newValue - the value of the ViewModel's property that will be set.

Getting and setting of the property is performed using type reflection.
First we find the property info in the ViewModel's type and get its old value.
We check if the change is really needed, and return if not.
If UndoRedoManager is not actually undoing, we register a new ChangePropertyAction with old and new value.
As the ViewModel does not store its own copy of the value, but gets the value directly from the Model
(see above), we just change the Model's property using ChangeModelProperty method.
At the end, we notify all the observers that a property was changed to update the view.

```csharp
  public bool ChangeEntityProperty(string thisPropertyName, string modelPropertyName, object? newValue)
  {
    var type = this.GetType();
    var property = type.GetProperty(thisPropertyName);
    if (property == null)
      throw new ArgumentException($"Property '{thisPropertyName}' not found on entity type {type.Name}", nameof(thisPropertyName));
    
    var oldValue = property.GetValue(this);
    if (Equals(oldValue, newValue)) return false; // No change needed
    if (!UndoRedoManager.IsUndoing)
      UndoRedoManager.Record(new ChangePropertyAction(), new ChangePropertyArgs(this, thisPropertyName, oldValue, newValue));
    ChangeModelProperty(modelPropertyName, newValue);
    NotifyPropertyChanged(thisPropertyName);
    return true;
  }
```

### ChangeModelProperty method

ChangeModelProperty method changes the value of the Model's property. It requires Model's property name and its new value.

We use type reflection once more.
Similary we find the property info in the Model's type and get its old value.
Similary we check if the change is really needed, and return if not.
This time we don't need to register ChangePropertyAction, because undo is done by wrapping property of ViewModel.

```csharp
  private bool ChangeModelProperty(string propertyName, object? newValue)
  {
    var type = Model.GetType();
    var property = type.GetProperty(propertyName);
    if (property == null)
      throw new ArgumentException($"Property '{propertyName}' not found on entity type {type.Name}", nameof(propertyName));

    var oldValue = property.GetValue(Model);
    if (Equals(oldValue, newValue)) return false; // No change needed
    property.SetValue(Model, newValue);
    return true;
  }
```

### DeleteRecords method

DeleteRecords methods removes selected rows from the dataGrid's dataSource. 
As there can errors occur during deleting records in the database, all the code is enclosed in try-catch-finally instruction.
Note that we moved UndoManager.StopGrouping call to the finally clause. Catch clause transfers exception message to errorMessageProvider.
In the main loop, before row removing, we record new DelRecordAction (defined separately).


```csharp
  private static void DeleteRecords(SfDataGrid dataGrid, object[] selectedRows)
  {
    if (dataGrid.ItemsSource is IList dataSource)
    {
      IErrorMessageProvider? errorMessageProvider = null;
      try
      {
        UndoRedoManager.StartGrouping();
        foreach (var row in selectedRows)
        {
          errorMessageProvider = row as IErrorMessageProvider;
          var delRecordArgs = new DelRecordArgs(dataSource, dataSource.IndexOf(row), row);
          UndoRedoManager.Record(new DelRecordAction(), delRecordArgs);
          dataSource.Remove(row);
        }
      }
      catch (Exception e)
      {
        if (errorMessageProvider != null)
          errorMessageProvider.ErrorMessage = e.Message;
      }
      finally
      {
        if (UndoRedoManager.IsGrouping)
          UndoRedoManager.StopGrouping();
      }
    }
  }
```

### DelRecordAction class

DelRecordAction is a specific IAction class to undo/redo database record deletion.
To undo deletion, we insert a deleted dataObject at the stored index to the dataList.
All three values are stored as DelRecordArgs.

```csharp
public record DelRecordArgs(IList DataList, int? Index, object DataObject);

public class DelRecordAction: IAction
{
  public void Execute(object? args)
  {
    if (args is not DelRecordArgs delRecordArgs)
      throw new ArgumentNullException(nameof(args));
    delRecordArgs.DataList.Remove(delRecordArgs.DataObject);
  }

  public void Undo(object? args)
  {
    if (args is not DelRecordArgs delRecordArgs)
      throw new ArgumentNullException(nameof(args));
    if (delRecordArgs.Index.HasValue && delRecordArgs.Index.Value >= 0 && delRecordArgs.Index.Value < delRecordArgs.DataList.Count)
      delRecordArgs.DataList.Insert((int)delRecordArgs.Index, delRecordArgs.DataObject);
    else
      delRecordArgs.DataList.Add(delRecordArgs.DataObject);
  }
}
```
