namespace Qhta.MVVM
{
  public class DataListViewModel<ItemType>: ListViewModel<ItemType> where ItemType: class, IValidated, ISelectable
  {

  }
}
