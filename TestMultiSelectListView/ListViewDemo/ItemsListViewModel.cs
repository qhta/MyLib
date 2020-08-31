using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Qhta.WPF.Utils;

namespace TestMultiSelectListView
{
  public class ItemsListViewModel : ObservableCollection<ListItemViewModel>, INotifyPropertyChanged, IListSelector
  {
    public ItemsListViewModel()
    {
      base.CollectionChanged += ListViewModel_CollectionChanged;
    }

    private void ListViewModel_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      if (e.NewItems != null)
        foreach (var item in e.NewItems.Cast<ListItemViewModel>())
          item.PropertyChanged += Item_PropertyChanged;
    }

    private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsSelected")
      {
        NotifySelectionChanged();
      }
    }

    public void SelectItem(object item, bool select)
    {
      if (item is TreeViewItem)
        item = (item as TreeViewItem).DataContext;
      if (item is ListItemViewModel)
      {
        (item as ListItemViewModel).IsSelected = select;
        NotifySelectionChanged();
      }
    }
    public void SelectAll(bool select)
    {
      foreach (var item in Items)
      {
        item.IsSelected = select;
      }
      NotifySelectionChanged();
    }

    public void NotifySelectionChanged()
    {
      OnPropertyChanged(new PropertyChangedEventArgs("SelectedItemsCount"));
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
