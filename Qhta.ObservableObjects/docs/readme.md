A package of classes that use Dispatcher to invoke back-threaded methods in main thread when implementing interfaces
INotifyPropertyChanged and INotifyCollectionChanged.

Classes are:
* ObservableObject - a class that implements INotifyPropertyChanged interface. It invokes Dispatcher on PropertyChanged event.
 It defines a static CommonDispatcher property which is initially set to Dispatcher.CurrentDispatcher.
 To use it properly, an instance of this class or any derived class must be created in main application thread (GUI thread).
 It can also be set-up programatically in GUI thread or overriden in any instance by set-up its Dispatcher property.

 * ObservableCollectionObject - an abstract base class for derived collection classes.
 It itself derives from ObservableObject class.
 It implements INotifyCollectionChanged interface by defining CollectionChanged event and a protected HandleCollectionChangedEvent method.
 To bind it to WPF collection view properly, 
         BindingOperator.EnableCollectionSynchronization(itemsCollection, itemsCollection.SyncRoot)
 must be invoked.
 Instead of this explicit invoke in code, proper binding can be assured in XAML using CollectionViewBehavior class from Qhta.WPF.Behaviors assembly.
 First you must define a namespace prefix, like this:
         xmlns:bhv="clr-namespace:Qhta.WPF.Behaviors;assembly=Qhta.WPF.Behaviors"
 Then set-up an attribute in the collection view declaration in XAML:
         bhv:CollectionViewBehavior.EnableCollectionSynchronization="True"

* ObservableList{TValue} - a dispatched version ObservableCollection{TValue}. 
 It derives from ObservableCollectionObject class to notify collection changes.
 It uses ImmutableList class to implement items.
 It also implements List{TValue} operations, like AddRange and Sort.

* ObservableDictionary{TKey, TValue} - an observable version of Dictionary{TKey, TValue}. 
 It derives from ObservableCollectionObject class to notify collection changes.
 It uses ImmutableDictionary class to implement items.
 Its default enumerator moves on Values.
