Package Qhta.SF.WPF.Tools is a library for extending Syncfusion WPF controls functionality.

*Note: Trying to name it “Qhta.Syncfusion.WPF.Tools” result compiling errors due to Visual Studio C# namespace resolving policy!*

The package contains:
- Default SfDataGrid styles and templates,
- LongTextColumn template and functionality,
- Clipboard operations on SfDataGrid,
- SfDataGridFiltering tools,
- SfDataGridFinder class and commanding,
- FillColumn command,
- ColumnManagementCommand command,
- Behavior extensions for SfDataGrid and GridColumn classes.

# Default SfDataGrid styles and templates

The SfDataGridTools resource dictionary contains styles, converters and data templates extending functionality of the original SfDataGrid. 
Some of the style resources only contain specific default property setters, 
some are extracted from the original Syncfusion.SfGrid.WPF resources and extended with new templates. 
The resource dictionary code-behind functionality is also provided in the SfDataGridTools.cs file. 

## The whole SfDataGrid column selection

Default styles for GridHeaderCellControl, GridCell are defined to let the application to select the grid columns, 
and mark them by gray background.

When nothing is selected, the column header row and row header column are displayed with silver background with graphite foreground 
and the grid cell background is transparent.

When the column is selected, its header cell background turns to dark gray, and its cells background turns to silver. 
The appropriate styles are named as "SelectedColumnHeaderStyle", "SelectedGridCellStyle" and "UnselectedGridCellStyle".

The GridHeaderCellControl style handles MouseLeftButtonDown event to let the user to select the whole column with mouse click on the column's header cell. 
The code-behind method is rather sophisticated as we must enable user to click filter button and the sorting marker at the right of the header cell.
So if the column allows filtering, we set the right margin limit to 20 pcx, and if the column allows sorting, we add more 20 pcx to the right margin limit.
We must also enable user to resize columns by dragging their left edges, so we set the small (5 pcx) left margin limit. 
We continue to handle the mouse click only if its position is between left and right margin limits.

We use SfDataGridColumnBehavior to mark the column as selected. 
We do not use the SfDataGrid.SelectionController to select all the cells in the column separately because of the two reasons. 
First, because of the performance of selection. When the data grid contains a large number of rows, selection of all the cells in the column takes much time.
Second reason is to let the application to load the data of the grid in the background task. 
If we would select the cells of the column separately, the cells in rows which data have not been loaded yet, would stay unmarked.
Instead we use the data grid SelectionController to ClearSelections if some cells they were selected separately.

Next we handle keyboard modifier as Shift and Control. 
If none of them is pressed, only the currently clicked column is selected, and other columns are unselected.
If only the Control modifier key is pressed, no other columns are unselected.
If the Shift modifier key is pressed (and Control modifier is not pressed), 
then all the column between the currently clicked column the previously selected columns are selected.
As the columns are represented by the indexed collection, we find the last prior (left) selected column index and the first next (right) selected column index.
If the previously selected columns where both on the left and on the right of the currently clicked column, 
all the columns between the current and the nearest one are marked as selected.

SfDataGridColumnConverter class is used to provide a mechanism to get a GridColumn object "IsSelected" property for data binding in SfDataGrid. 
We can use its static LogIt property to debug the selection.

## The whole SfDataGrid row selection

The whole row selection is handled by the original SfDataGrid, so we do not need to handle it in SfDataGridTools. 
If the SfDataGrid SelectionUnit property is set to Any, then when the user click the row header, all the cells in the row are selected.

## Individual SfDataGrid row resizing

We extracted and redefined the original GridRowHeaderCell style for other purpose - to let the user to resize the individual rows.
SfDataGrid has AllowResizingColumns property which lets the user to resize columns by dragging their edges, but there is no AllowsResizingRows property.
Original SfDataGrid allows the application to evaluate the individual rows heights by the QueryRowHeight event, 
but it lacks user functionality to manipulate the heights.
To fill this gap, we defined a control named RowResizer (as a descender of Thumb control). The dependency properties of this control are: 
- MinRowHeight - set by default to 24,
- MaxRowHeight - set by default to 120.

RowResizer control handles LeftMouseDown, MouseMove, and LeftMouseUp to let user to drag this control. 
To resize the row, its dataContext must implement IRowHeightProvider interface. 
This interface declares a single RowHeight property to store the current row height.

The application can switch the row resizing functionality on by using SfDataGridBehavior AllowRowResizing attached property. 
RowResizer control uses SfDataGridBehavior IsRowResizing attached property to mark the state between LeftMouseDown and LeftMouseUp 
and let the user to drag the boundary between rows.

The new GridRowHeaderCell style is named as "ResizedGridRowHeaderCellStyle". 
It differs from the original GridRowHeaderCellStyle in the content of "PART_RowHeaderIndicatorGrid", where we included RowResizer.
If the PART_RowHeaderIndicatorGrid is visible, the appropriate row's height can be increased or decreased 
by dragging the bottom edge of the row header cell down and up.

As the current row height is stored in the row's data context RowHeight property, this functionality requires that the data grid handles 
QueryRowHeight event. The event handler is implemented in code-behind in the SfDataGridTools.cs file.

## The whole data selection

The user can select the whole data in the data grid by clicking the "cell" at the crossing of the column headers row and row headers column.
This cell control type is GridRowIndencCell, 
and its style declared in the SfDataGridTools resource dictionary sets the event handler of the MouseLeftButtonDown event.
Its implementation in code-behing simply selects or unselects all columns of the data grid. 
It uses the extension methods AreAllColumnSelected and SelectAllColumns implemented in SfDataGridSelector static class.

## Default SfDataGrid style settings

The default style for SfDataGrid sets the following properties:
- AllowDraggingColumns to True,
- AllowEditing to True,
- AllowFiltering to True,
- AllowResizingColumns to True,
- AllowSorting to True,
- AllowTriStateSorting to True,
- AllowDeleting to True,
- ColumnSizer to Auto,
- HeaderRowHeight to 26,
- AutoGenerateColumns to False,
- GridValidationMode to InEdit,
- EditTrigger to OnTap,
- EnableDataVirtualization to True,
- GridCopyOption to CopyData,IncludeFormat,
- SelectionUnit to Any,
- SelectionMode to Extended,
- ShowRowHeader to True.

It also sets event handlers:
- Loaded event to DataGrid_OnLoaded,
- KeyDown event to DataGrid_OnKeyDown.

Both methods are implemented in the code-behind in the SfDataGridTools.xaml.cs file.

### SfDataGrid OnLoaded event handler

SfDataGrid Loaded event is needed to initialize these event handlers, which are not RoutedEvent handlers
and can not be initialized in xaml. These are:
- QueryRowHeight,
- FilterItemsPopulating,
- FilterItemsChanged,
- GridCopyContent,
- GridPasteContent.

In contrast to the above events, KeyDown event is a RoutedEvent and is initialized in SfDataGrid default style.

### SfDataGrid OnQueryRowHeight event handler

QueryRowHeight simply invokes OnQueryRowHeight method of the RowHeightProvider static class. 
This method checks if the appropriate record of the SfDataGrid.View.Records collection implements IRowHeightProvider interface
and sets event args Height property to its RowHeight property value. If this value is not NaN, the event is just handled.

If the QueryRowHeight event is not handled by RowHeightProvider then SfDataGrid invokes OnQueryRowHeight method 
of the LongTextColumn class. 
This method if the appropriate record of the SfDataGrid.View.Records collection implements ILongTextViewModel interface
and evaluates the row height basing on the cell text content.

### SfDataGrid OnFilterItemsPopulating and OnFilterChanged event handlers

They simply redirect to SfDataGridFiltering static class appropriate methods.

### SfDataGrid OnGridCopyContent and OnGridPasteContent event handlers

These handlers use SfDataGridCommander static class and its CanCopyData, CopyData, CanPasteData, and PasteData appropriately.
These handlers are needed because in specific (and not very clear) conditions the internal logic of SfDataGrid tries 
to copy or paste data ommiting Copy and Paste commands.

Note that there are no similar CutData nor DeleteData events declared in the original SfDataGrid.

### SfDataGrid OnKeyDown event handler

This method handles the user pressed:
- Ctrl-C key combination to invoke SfDataGridCommander.CopyData method,
- Ctrl-X key combination to invoke SfDataGridCommander.CutData method,
- Ctrl-V key combination to invoke SfDataGridCommander.PasteData method,
- Delete key to invoke SfDataGridCommander.DeleteData method,
- Ctrl-F key combination to invoke FindCommand.Execute method,
- F3 key to invoke FindCommand.ExecuteNext method,

# LongTextColumn template and functionality

Original Syncfusion WPF library defines several types of column. GridTextColumn is one of them, and it operates on String data.
However its cell template is limited to simple TextBox functionality, which is not enough if the cell content string is long.

LongTextColumn class was thought as an extension for the original GridTextColumn. 
However as we need to define new template, it is declared as an extension for GridTemplateColumn class.

LongTextColumn provides sophisticated functionality for those SfDataGrid colums which are mapped to string-typed properties with no length limit. 
When the column is wide enough to display the whole text content, its layout does not differ from the GridTextColumn.
When the text does not fit in a single line, a triangle-down button is displayed on the right of the cell.
The user can click the button to show a popup text box with the whole text. 
The popup stays open until the user clicks a triangle-up button displayed at the rigth-top corner of the popup, or selects another cell.
The popup is displayed also when the user starts editing. 
Its height grows when the user add more and more text.

Two data templates are defined for a LongTextColumn control:
- LongTextCellTemplate for displaying the text,
- LongTextEditTemplate for editing.

Both templates are contained in the SfDataGridTools.xaml. They use two specially-defined value converters:
- GridColumnMappingConverter and
- LongTextColumnExpanderVisibilityConverter.

These converters take the bound property name which is stored in a MappingName property of the column. 
First converter returns the name of the bound property. 
It implements IMultiValueConverter interface, but not IValueConverter because of two-way binding purposes (needed for editing).

The second converter also implements IMultiValueConverter despite it returns a Visibility-typed value used to hide and show ShowPopupButton.
It could be an implementation of IValueConverter interface, but copies much code from the GridColumnMappingConverter. 
To determine whether the ShowPopupButton should be visible or collapsed, it uses an EvaluateTextHeight method of the LongTextColumn class.

# Clipboard operations on SfDataGrid

Three clipboard commands: Copy, Cut, and Paste are implemented in SfDataGridCommander class. 
Because a Delete command implementation is similar to the Cut command, it was added to the same class.

So, SfDataGridCommander class is partially encoded in six files:
- SfDataGridCommander!.cs
- SfDataGridCommander.CopyData.cs
- SfDataGridCommander.CutData.cs
- SfDataGridCommander.DeleteData.cs
- SfDataGridCommander.PasteData.cs
- SfDataGridCommander~.DataOp.cs

*Note that "!" and "~" suffixes are used solely to achieve logical file-name sorting.*

First file contains basic methods used in core methods implementation. Next four files contain methods directly invoked in commands implementation:
- CanCopyData, CopyData - for Copy command,
- CanCutData, CutData - for Cut command,
- CanDeleteData, DeleteData - for Delete command,
- CanPasteData, PasteData - for Paste command.

Each method has a single parameter of SfDataGrid type.

Methods invoked for Copy, Cut, and Delete commands redirect invocations to the last file (SfDataGridCommander~.DataOp.cs), 
which contains general implementation of one of three data operations 
(defined as enum type DataOp): Copy, Cut, and Delete. There are two general methods: CanExecuteDataOp and ExecuteDataOp, which have two parameters:
one of SfDataGrid type and one of DataOp type. 
These methods take advantage of the fact that the three operations Copy, Cut, and Delete share common elements of functionality. 

They operate on selected columns, rows, or cells of the data grid. 
They use a GetSelectedRowsAndColumns method, which returns an array of GridCellInfo of selected cells of the data grid, 
but also have four output parameters:
- allColumnsSelected - set to true if and only if all the columns of the grid are selected,
- selectedColumn (as an array of GridColumn) - filled with those columns, which are currently selected,
- allRowsSelected - set to true if and only if all the rows of the grid are selected,
- selectedRows (as an array of object) - filled with those data rows, which are currently selected.

They also use GetRowDataType method which returns a type of elements of the grid.View.SourceCollection, 
and GetGridColumnInfos which returns additional array of GridColumnInfo (exposing ValuePropertyInfo and DisplayPropertyInfo).

Copy and Cut operations build clipboard content as a text lines. 
The first line in the content is a header line, which consists of data grid headers 
(HeaderText or MappingName of the columns declarations) separated with Tab ('\t') characters.
Header line is omitted if only one cell is copied to the clipboard.
Each data line consists of cell data (get using ValuePropertyInfo or DisplayPropertyInfo) also separated with Tab characters.

Cut and Delete operations set null values to each selected cell -- in case of deleting separate cells. 
If the whole rows are selected, data records are removed from the data grid's items source, which must implement IList interface.
However, in the implementation of the CanExecuteDataOp method, if the items source implements IRemovableCollection, 
we can check whether we can remove individual records.

CanPasteData and PasteData method are implemented directly in SfDataGridCommander.PasteData.cs file. 

To provide undo operations, an UndoMgr class (defined in Qhta.UndoManager library) is used in PasteData, CutData and DeleteData operations.


# SfDataGridFiltering tools

SfDataGrid class has just implemented filtering and sorting operations. 
When AllowSorting is set to true, the user can switch sorting on by clicking column headers. 
If also AllowTriStateSorting is set to true, the user can switch sorting on and off. 
If AllowTriStateSorting is set to to, the user can only switch sorting direction.

If AllowFiltering is set to true, a funnel-shape button is displayed at the right of each column header. 
When the user click this button, SfDataGrid displays FilterControl popup. FilterControl enables user to sort and filter data.
The use can have two modes of filtering:
- selectable filtering,
- advanced fitering.

Selectable filtering is used when the column maps to enumerable data. A check list is displayed with selectable element 
and, additionaly, with empty and non-empty elements. However, enumerable elements are identified by enum names.
If an combobox popup list displays elements in other way, both lists are inconsistent. 

A static class SfDataGridFiltering contains two methods to address this problem:
- OnFilterItemsPopulating,
- OnFilterChanging.

Both methods are event handlers used by SfDataGrid. First method is invoked when filtering is applied to a column.
If it is a GridComboBoxColumn, a local SetSelectableItemsFilter method is invoked. Otherwise SetAdvancedFilter is invoked.

SetSelectableItemsFilter method parameter an enumeration of elements that implement ISelectableItem. 
If the items source is not IEnumerable of ISelectableItem, a temporary list of SelectableItem is created.
ISelectableItem interface provides properties used to build and to apply selectable filtering:
- DisplayName - to display an item in the checked list,
- ToolTip - to display a tooltip for selectabe items,
- ActualValue - to provide a value to evaluate filtering predicates,
- IsEmpty - a read-only property with default method checking whether the ActualValue is null,
- IsNotEmpty - a read-only property with default method checking whether the ActualValue is NonEmptyValue,
- IsSelected - determines whether an item is selected in UI.

IsEmpty and IsNotEmpty properties are used in evaluating filtering predicates for special values. 
NonEmptyValue is a special singleton type to express all elements that are not null.
These two properties have default implementation declared in the ISelectableItem interface
and do not need to be implemented separately in the class that implements this interface.

The second method, OnFilterChanging, is used to fit predicate FilterType and FilterValue according to empty and non-empty values.
If the predicate FilterValue is a ISelectableItem implementation, then if it is an empty item (IsEmpty property gives True), 
then filter value is set to null, and filter type is set to Equals. 
It the item it is non-empty (IsNotEmpty property gives True), then filter value is set to null, but filter type is set to NotEquals. 

ISelectableItem interface can be implemented directly in a ViewModel class, or a SelectableItem class may be used.
The SelectableItem class is usefull especially when the selectable items list must display enum-typed values.
As the enum-typde values are simple static fields of the enum type, it is not possible to make them to implement the ISelectableItem interface.
In this case, we can wrap enum type values with SelectableItem class.

The SelectableItem class define (in addition to ISelectableItem properties) also two properties having IValueConverter types:
- ValueConverter and
- ToolTipConverter.

These converters are used in get-methods of DisplayName and ToolTip values.
ValueConverter can be used to transform enum-typed value programmatic names (which are singular words) 
to more friendly display names (including localized resource strings in non-English applications).
ToolTipConverter can be used to transform enum-typed value programmatic names to longer explanations (also using localized resource strings).

This way we get a fully-localizable user interface for setting column filters.

# SfDataGridFinder class and commanding

Original Syncfusion framework lacks of finding and replacing values or text functionality. SfDataGridFinder class 
(along whith some other classes) fill this gap by providing a UI and API for finding and selecting data in the SfDataGrid.

First, we provide a FindAndReplaceCommand class which enables user to start finding (and optionally replacing) 
values or text in SfDataGrid columns. The command can be executed when one and only one column of the grid is selected.

The FindAndReplaceCommand has four public methods:
- CanExecute,
- Execute,
- CanExecuteFindNext,
- ExecuteFindNext.

First two methods are used to handle full interactive Find/Replace command, which opens a SpecificValueWindow in one of two modes:
- Find mode,
- FindAndReplace mode.

Last two methods are used to handle Find/Replace next item command, which does not open a dialog window, but is based on previous Find/Replace settings.

SpecificValueWindow open mode can be determined by a property FindOrReplaceMode of FindAndReplaceCommand instance. It can be one of:
- Auto (default),
- Find,
- Replace.

In Auto mode, the window open mode is determined by the data grid and column read/write capabilities.
In Find mode, the window open mode is set to Find.
In Replace mode, if the the data grid or column does not allow writing, CanExecute method returns false and Execute method throws an exception.

Find mode enables user to select a value and an option how to find a value in sequence:
- FindFirst,
- FindNext,
- FindAll.

In FindFirst mode, the selected value is searched from the beginning of the column (from the top row). When found, the cell is selected.
If not found, the appropriate message is displayed for the user.
In FindNext mode, the selected value is searched beginning from the next cell (below the current cell) in the column.
In FindAll mode, all the cells in the column are iterated and all cells, that fulfills the search criteria, are selected.

FindAndReplace mode is similar to Find mode, but enables an option to select a replacement value. 
When the user clicks to select a replacement value, the secondary field for it is shown. 
In command execution, the cells are not only selected, but also replaced with a replacement value.

The SpecificValueWindow can work in one of SpecificViewModes:
- Edit mode,
- Selector mode,
- Both mode.

In Edit mode, a SpecificEditView is visible in the SpecificValueWindow. 
The SpecificEditView enables user to edit a specific value in a TextBox. 
After checking Replace option, the user can also edit a replacement value. 
In Selector mode, a SpecificValueSelector view is visible. 
The SpecifiedEditSelector enables user to select a specific value in a ListBox, 
and after checking Replace option, also to select a replacement value. 
In Both mode, both views are visible and selectable in a TabControl.

To execute the FindAndReplace command, a special SfDataGridFinder class is defined. 
The instance of this class is created and stored for a column in first command execution.

SfDataGridFinder stores the SpecifiedValue, the ReplacementValue, boolean Replace option, and boolean Found result. 
It also stored a Predicate property using FilterPredicate class. 
The stored Predicate is created in FindFirst and FindAll modes, and used in FindNext modes. 

We do not use FilterValue property of FilterPredicate. 
Instead, in EvaluatePredicate method of SfDataGridFinder, a separate specifiedValue parameter is applied.
In EvaluateReplacement, both separate specifiedValue and replacementValue properties are applied.

We use other properties of FilterPredicate:
- FilterBehavior property to select if the SpecifiedValue is StringTyped or StronglyType,
- FilterType property to select a comparison operation,
- IsCaseSensitive property to determine if SpecifiedValue in StringTyped mode is searched with case sensitivity.

# Fill Column Command

A FillColumnCommand uses SpecificValueWindow in Fill mode. In this mode, specifying a replacement value is not possible. 
Also FindInSequence option is not visible. 
Instead an OverwriteNonEmptyCells options is visible to let the execution method to fill also non-empty cells.

# Column Management Command

Original SfDataGrid library contains a class named GridColumnChooserControl. 
It is a class of a popup window, which enables user to hide/show individual grid columns.
However the basic control implementation requires, that the user drags column header to the popup window to hide it. 
We developed more traditional mechanism of column hiding or showing.

ColumnManagementCommand opens the ColumnManagementWindow which shows all the columns in the grid. The column headers are listed along with checkboxes.
The user can decide to hide or show each column by clicking the checkboz.

Moreover, the ColumnManagementWindow allows user to change the order of column appearance by using MoveUp/MoveDown buttons.

ColumnManagementCommand can be executed if SfDataGrid AllowColumnManagement attached property is set to true.

ColumnManagementCommand can be activated by right mouse button click on any column header cell 
(but only if there is no ContextMenu assigned to the column).
To do so, both styles for GridHeaderCellControl defined in SfDataGridTools.Xml file (default style ant SelectedColumnHeaderStyle)
assign GridHeaderCellControl_MouseRightButtonDown handler to PreviewMouseDown event.

# Behavior extensions for SfDataGrid and GridColumn classes

There are two classes defined that implement Behavior mechanism. These are:
- SfDataGridBehavior and
- SfDataGridColumnBehavior.

The Behavior is a Microsoft extension mechanism for PresentationFramefork controls. 
It enables the XAML developer to attach new properties to existing controls without redefining their classes.

The SfDataGridBehavior defines the following properties for SfDataGrid:
- AllowRowResizing - to enable resizing rows by dragging low edge or row header cells.
- IsRowResizing - to determine the current state of row resizing,
- StartOfOffset - to store the pixel distance between the starting position of mouse click and the row intial height.
- AllowColumnManagement - to let user to execute ColumnManagementCommand on right mouse button click on any column header cell.

The SfDataGridColumnBehavior defines one attachable property:
- IsSelected.

It can work for GridColumn and for GridHeaderCellControl.
 When it is get or set to GridHeaderCellControl, its boolean value is stored to the appropriate CellColumn. 
 
There is als SfDataGridColumnBehaviorExtensions static class. 
It defines static methods to store SfDataGridFinder instance in a GridColumn:
- SetFinder and
- GetFinder.
 
The accessor SetFinder/GetFinder methods can not be declared in SfDataGridColumnBehavior class because this class is not static.