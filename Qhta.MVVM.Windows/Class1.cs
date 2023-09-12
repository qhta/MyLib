namespace Qhta.MVVM;

public class EditableListViewModel<T>: ListViewModel<T>/*, IEditableCollectionView*/
  where T : class, IValidated, ISelectable
{

}
