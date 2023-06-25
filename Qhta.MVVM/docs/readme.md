This package implements Model-View-View-Model paradigm with several classes and interfaces 
that use System.Windows and System.Windows.Input namespaces components from WindowsBase library 

Interfaces:
* IExpandable - interface that defines IsExpanded property.
* ILazyLoad - interface that defines properties and a method for loading object in background thread.
* IListViewModel - interface for ListViewModel{ItemType}
* INotifySelectionChanged - interface that declares SelectionChanged event.
* INumbered - interface for an object that has a <see cref="Number"/> property.
* IOrientable - interface for ListViewModel to be oriented horizontally or vertically.
* IRelayCommand - an interface expanding ICommand (from System.Windows.Input) with NotifyCanExecuteChanged method.
* ISameAs - interface for an object which can be compared to another object.
* ISelectable - interface that defines IsSelected property.
* ISelector - interface for the object that can have a selection.
* ISequenceable{ItemType} - Interface for a collection that can go to next or previous object.
* IValidated - an interface that defines IsValid method.
* IViewModel - an interface expanding INotifyPropertyChanged (from System.ComponentModel) with NotifyPropertyChanged method.
* IVisible - interface that defines IsVisible property.

ViewModels:
* ViewModel - base class of view model.
* ViewModel{ModelType} - ViewModel with a specific model object.
* VisibleViewModel - ViewModel which implements IVisible, ISelectable, IDetailedRow and IExpandable interfaces.
* VisibleViewModel{ModelType} - VisibleViewModel which has a model of a specific type.
* LazyLoadViewModel{ModelType} -abstract LazyLoadViewModel{ModelType} that implements ILazyLoad and IExpandable interfaces.
* ListViewModel - abstract VisibleViewModel for a list.
* ListViewModel{ItemType} with specified item type.

Commands:
* Command - a class that implements interface ICommand (from System.Windows.Input) using DependencyObject (from System.Windows).
* Commands - observable collection of Command instances.
* DispatchedCommand - a command which invokes Dispatcher when NotifyCanExecuteChanged is called.
* RelayCommand - a command which relays its functionality to other objects by invoking delegates.
* RelayDispatchedCommand - a dispatched command which relays its functionality to other objects by invoking delegates.

Events:
* CurrentItemChanged - event raised when a current item of ListViewModel has been changed.
* SelectionChanged - event raised when a selection of ListViewModel has been changed.