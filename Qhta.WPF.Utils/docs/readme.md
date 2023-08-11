This package contains behavior helper classes which can be used in WPF XAML. 

The classes are as follows:
* CollectionViewBehavior - defines EnableCollectionSynchronization property to help establish synchronized binding
 between CollectionView and Collection which has thread-safe operations. To use it,
 first you must define a namespace prefix, like this:
         xmlns:bhv="clr-namespace:Qhta.WPF.Utilss;assembly=Qhta.WPF.Utilss"
 Then set-up an attribute in the collection view declaration in XAML:
         bhv:CollectionViewBehavior.EnableCollectionSynchronization="True"

* DispatcherBridge - implements IDispatcherBridge interface from ObservableObjects assembly.

