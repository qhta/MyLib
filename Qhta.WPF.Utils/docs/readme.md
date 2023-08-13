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

* BackgroundDehavior - Enables a control to observe target object "Waiting" boolean property and display a waiting cursor.
 Target object must be implement INotifyPropertyChanged interface. It defines two properties:
* CollectionViewBehavior - Establishes synchronized binding
 between CollectionView and Collection which has thread-safe operations. The only property is:
* ComboBoxBehavior - ComboBox behavior class that defines its IsNullable property and Cleared event.
* DataGridBehavior - DataGrid behavior class that defines properties and events for find and filter functionality.
* GridViewBehavior - Defines behavior for GridView - a special component of ListView.
* InputBindingAttachment - Behavior class to define an InputBinding property that can be attached to an UI element.
* ListboxBehavior - Behavior class that defines several properties and events for ListBox.
* ListViewBehavior - Behavior class that defines a few ListView properties.

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
