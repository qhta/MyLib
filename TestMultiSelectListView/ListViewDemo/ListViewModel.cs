using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Qhta.WPF.Utils;

namespace TestMultiSelectListView
{
  public class ListViewModel : ObservableCollection<ListItemViewModel>, INotifyPropertyChanged, IListSelector
  {
    public ListViewModel()
    {
      base.CollectionChanged += ListViewModel_CollectionChanged;
    }

    private void ListViewModel_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      if (e.NewItems!=null)
        foreach (var item in e.NewItems.Cast<ListItemViewModel>())
          item.PropertyChanged += Item_PropertyChanged;
    }

    private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName=="IsSelected")
      {
        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        OnPropertyChanged(new PropertyChangedEventArgs("SelectedItems"));
      }
    }

    public void SelectItem(object item, bool select)
    {
      if (item is ListViewItem)
        item = (item as ListViewItem).DataContext;
      if (item is ListItemViewModel)
      {
        (item as ListItemViewModel).IsSelected = select;
        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        OnPropertyChanged(new PropertyChangedEventArgs("SelectedItems"));
      }
    }

    public void SelectAll(bool select)
    {
      foreach (var item in Items)
        item.IsSelected = select;
      OnPropertyChanged(new PropertyChangedEventArgs("Count"));
      OnPropertyChanged(new PropertyChangedEventArgs("SelectedItems"));
    }

    public new event PropertyChangedEventHandler PropertyChanged
    {
      add => base.PropertyChanged += value;
      remove => base.PropertyChanged -= value;
    }

    public int SelectedItemsCount => this.Count(item => item.IsSelected);

    public IEnumerable<object> SelectedItems => this.Where(item => item.IsSelected);
  }
}
