This package contains containers class which extends Qhta.MVVM package for work on DataViewModel items:
* DataViewModel - it is an extension of ViewModel with data loading state
* DataRowViewModel it is an extension of ViewModel which implements IExpandable, ISelectable
* DataListViewModel<T> - it is an extension of ListViewModel<ItemType> where Item type is IValidated, ISelectable
* DataSetViewModel - it is an extension of DataViewModel with VisibleColumns property
* DataSetViewModel<T> - it is an extension of ListViewModel<ItemType> where ItemType: DataRowViewModel
* DataTreeViewModel - it is an extension of DataViewModel with CanExpandItems property
* DataTreeViewModel<T> - it is an extension of DataTreeViewModel with Items property
* DataTreeViewModel<EntityType, ChildType> - DataTreeViewModel<T>