This package contains utility classes which can be used in WPF XAML. 
Behavior classes are designed to define additional behavior properties of XAML elements directly in XAML.
Utility classes are designed to provide commonly used methods for WPF elements.
Markup extension classes are used in XAML and provide values.
There are also some classes that serve as bridges from ObservableObjects and MVVM ViewModels to WPF.
These classes are defined outside ObservableObject and MVVM modules to let these modules to be defined in .NET Standard.

To use behavior classes in XAML you must first define a namespace prefix in XAML, e.g.:

        xmlns:utl="clr-namespace:Qhta.WPF.Utils;assembly=Qhta.WPF.Utils"

 Then you can set-up an attribute in the collection view declaration in XAML, e.g.:

        utl:CollectionViewBehavior.EnableCollectionSynchronization="True"

Behavior classes are as follows:

* BackgroundBehavior - enables a control to observe target object "Waiting" boolean property and display a waiting cursor.
* CollectionViewBehavior - establishes synchronized binding between CollectionView and Collection which has thread-safe operations.
 Defines many attached properties for Column and ItemsControls (like DataGrid) 
 that help to format DataGridColumns.
* ComboBoxBehavior - ComboBox behavior class that defines its IsNullable property and Cleared event.
* GridViewBehavior - Defines behavior for GridView - a special component of ListView.
* InputBindingAttachment - Behavior class to define an InputBinding property that can be attached to an UI element.
* ListboxBehavior - Behavior class that defines several properties and events for ListBox.
* ListViewBehavior - Behavior class that defines a few ListView properties.

Properties and events defined in CollectionViewBehavior are as follows:
* EnableCollectionSynchronizationProperty - when is set to true, then establishes synchronized binding 
 between CollectionView and Collection which has thread-safe operations.
* SortingEventHandlerProperty - enables setting an external method to handle DataGridSortingEvent of DataGrid.
 If this method is valid DataGridSortingEventHandler, it is assigned to the data grid.
 Otherwise a default handler is assigned which redirects event to DataContext implementing IListViewModel interface.
* IsSelectableProperty - specifies whether rows of the collection can be separately selected.
* ScrollIntoViewProperty - helper property to store added row which is passed to DataGrid ScrollIntoView.
* HiddenHeader - specifies a hidden header string for a column. This header is not displayed, but may be used e.g. in filtering dialog.
* ShowFilterButtonProperty - specifies whether a "Filter" button should be displayed in the column header.
* FilterButtonClickEventHandler - routed event to store FilterButtonClick event handler. It the DataGridColumnHeader
 has not handled this event, then OnShowFilterButton_Clicked is invoked.
* FilterButtonShapeProperty - helper property which defines visual shape of the filter button. 
The type of this property is FilterButtonShape. It can be "Empty" or "Filled".
* ColumnFilterProperty - helper property to store ColumnFilter property. It can be any object.
* CollectionFilterProperty - helper property to store CollectionFilter property. It can be any object.

Public virtual OnShowFilterButton_Clicked method checks the type of property to which the DataGridColumn
is bound and invokes DisplayFilterDialog method to let the user to choose filter function and value. 
 The dialog is defined in the MVVM package Views resource folder as ColumnFilterDialog.
 This dialog is opened with a specific view of the filter according to the type of the property.
 String, Boolean, Enum, Number and Object).
 After the user sets filter parameters and closes this dialog, 
 a CollectionViewFilter is built and applied to the items source of the items control. 
 Filtered collection can be a CollectionView (defined in PresentationFramework.dll)
 or can be a view model implementing IFiltered interface.

Contents of Views folder:
* ColumnFilterDialog - main dialog window for specific filter views.
* TextFilterView - user control for editing TextFilterViewModel properties.
* BoolFilterView - user control for editing BoolFilterViewModel properties.
* EnumFilterView - user control for editing EnumFilterViewModel properties,
* NumFilterView - user control for editing EnumFilterViewModel properties.
* ObjFilterView - user control for editing ObjFilterViewModel properties.

Contents of ViewModels folder:
* ColumnFilterOperation - specifies what to do with a column filter (Add, Edit or Clear).
* ColumnFilterViewModel - abstract view model of the filter stored and edited in ColumnFilterDialog.
 Has four main properties: 
 >- Operation (ColumnFilterOperation type) - which specifies what to do with a column filter after dialog is closed.
 >- EditOpEnabled (Boolean type) - which specifies whether Edit operation is enabled. If not, then Add operation is enabled.
 >- PropPath (PropertyInfo[] type) - which holds info of column binding properties.
   As binding path may complex, it is an array of property info items, which values must be evaluated in cascade.
 >- PropName - displayed name of column binding property.

* TextFilterViewModel - specific ColumnFilterViewModel of string property filter edited in TextFilterView.
* TextPredicateFunction - enumeration of functions used to define text predicate: 
  IsEqual, NotEqual, IsEmpty, NotEmpty, StartsWith, EndsWith, Contains, RegExpr. 
  (IgnoreCase property is defined in TextFilterViewModel).
* BoolFilterViewModel - specific ColumnFilterViewModel of boolean property filter edited in BoolFilterView.
* BoolPredicateFunction - enumeration of functions used to define boolean predicate: 
  IsTrue, IsFalse, IsEmpty, NotEmpty.
* EnumFilterViewModel - specific ColumnFilterViewModel of enum property filter edited in EnumFilterView.
* EnumPredicateFunction - enumeration of functions used to define enum value predicate: 
  IsEqual, NotEqual, IsEmpty, NotEmpty.
* NumFilterViewModel - abstract base ColumnFilterViewModel of numeric property filter edited in NumFilterView.
* NumFilterViewModel\<T\> - generic NumFilterViewModel for numeric values filter with a type parameter. 
 Valid types are all comparable values, however internal TryParseFilterText method recognizes 
 only integers, floats, DateTime and TimeSpan.
* NumPredicateFunction - enumeration of functions used to define numeric predicate: 
  IsEqual, NotEqual, IsEmpty, NotEmpty, IsGreater, NotGreater, IsLess, NotLess.
* ObjFilterViewModel - specific ColumnFilterViewModel of object property filter edited in ObjFilterView.
* ObjPredicateFunction - enumeration of functions used to define object predicate: 
  IsEmpty, NotEmpty.

Utility classes:

* BindingEvaluator - A class to evaluate this Binding value. Defines GetValue methods.
* BindingExpressionUtils - Utility class for BindingExpressions. It defines GetString and RefreshBinding methods.
* ClipboardUtils - Utility class to implement copy DataGrid and ListView content to clipboard.
* DataGridColumnCreator - Utility class that helps to autogenerate DataGridContentBoundColumn. It uses DataGridColumn attributes assigned to properties.
* FocusUtils -  Utility class that checks whether the window has a control with focus.
* FrameworkElementUtils - Utility class that gets a parent window of the framework element.
* GeometryUtils - Utility class that contains some geometry calculation methods.

Bridges classes:
* CommandManagerBridge - Listener for CanExecuteChanged event implementation based on WPF CommandManager.
* DispatcherBridge - implements IDispatcherBridge interface from ObservableObjects assembly.

Markup extensions:
* CommandBindingExtension - Markup extension class that defines a commandName for a Command XAML definition.
* DynamicResource - Markup extension class that defines a dynamic resource.
* DynamicResourceBinding - DynamicResourceExtension class that implements dynamic resource binding with optional converter (as normal Binding).
* DynamicResourceProvider - Markup extension class that provides a type instance.
* JoinStrings - Markup extension that joins input strings.


Helper classes:
* ActionHolder - A class that can hold and execute an action.
* ArrayGraphics - A class to handle a bitmap (get/set array of pixels).
* BorderLine - A class that draws a border line adorner for an UI element.
* BrushUtils - A class that checks if the brush is null or empty. "Empty" brush is a solid color brush with Alpha component set to 0.
* ColorAdjustEffect - ShaderEffect class to adjust colors of the brush.
* ColumnViewInfo - Information on a single column to define a column view.
* ColumnsViewInfo - Observable collection of ColumnViewInfo.
* DataGridColumnCreator - Helper class that handles DataGridAutoGeneratingColumnEvent.
* DataGridColumnDef - Helper class that contains information to create or format DataGridColumn.
* DataGridContentBoundColumn - Specific implementation of DataGridBoundColumn with ContentTemplate.
* DragData - Helper class to drag data. Contains source DependencyObject and dragged data.
* DynamicTemplateSelector - Provides means to specify DataTemplates to be selected from within WPF code.
* EnumValuesProvider - DataSourceProvider that provides enum type values.

Helper interfaces:
* ICompoundItem - Interface that defines Items property.
* IListSelector - Interface that defines selecting methods.
* IProposedValueErrorInfo - Interface that defines GetError method.
