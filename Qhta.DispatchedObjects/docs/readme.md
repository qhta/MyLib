A package of classes that use Dispatcher to invoke back-threaded methods in main thread when implementing interfaces
INotifyPropertyChanged and INotifyCollectionChanged.

Classes are:
* DispatchedObject - a class that invokes <see cref="Dispatcher"/> on <see cref="PropertyChanged"/>> event or on other action.
 It defines a static <see cref="ApplicationDispatcher"/> property which enables a developer to setup a <see cref="Dispatcher"/> from any application
 (e.g. it can be <see cref="Application.Current.Dispatcher"/> in WPF applications).


* DispatchedCollection{TValue} - a dispatched version of <see cref="ObservableCollection{TValue}"/>.
 Is is based on <see cref="DispatchedObject"/> to notify on changes and to invoke actions.

* DispatchedDictionary{TKey, TValue} - a dispatched version of <see cref="Dictionary{TKey, TValue}"/>. 
 It implements <see cref="INotifyCollectionChanged"/> event (as <see cref="DispatchedCollection{TValue}"/> does).
 Is is based on <see cref="DispatchedObject"/> to notify on changes and to invoke actions.