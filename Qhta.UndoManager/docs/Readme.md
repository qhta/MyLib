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

Action grouping is currently used in Qhta.SF.WPF.Tools in clipboard operations, delete and fill commands.