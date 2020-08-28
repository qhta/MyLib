using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Qhta.WPF.Utils;

namespace TestMultiSelectTreeView
{
  public class ItemsListViewModel : ObservableCollection<TreeItemViewModel>, INotifyPropertyChanged, IListSelector
  {
    public ItemsListViewModel()
    {
      base.CollectionChanged += ListViewModel_CollectionChanged;
    }

    private void ListViewModel_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      if (e.NewItems != null)
        foreach (var item in e.NewItems.Cast<TreeItemViewModel>())
          item.PropertyChanged += Item_PropertyChanged;
    }

    private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsSelected")
      {
        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        OnPropertyChanged(new PropertyChangedEventArgs("SelectedItems"));
      }
    }

    public void SelectItem(object item, bool select)
    {
      if (item is TreeViewItem)
        item = (item as TreeViewItem).DataContext;
      if (item is TreeItemViewModel)
      {
        (item as TreeItemViewModel).IsSelected = select;
        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        OnPropertyChanged(new PropertyChangedEventArgs("SelectedItems"));
      }
    }

    public void SelectAll(bool select)
    {
      foreach (var item in Items)
      {
        item.IsSelected = select;
        item.Children?.SelectAll(select);
      }
      OnPropertyChanged(new PropertyChangedEventArgs("Count"));
      OnPropertyChanged(new PropertyChangedEventArgs("SelectedItems"));
    }

    public new event PropertyChangedEventHandler PropertyChanged
    {
      add => base.PropertyChanged += value;
      remove => base.PropertyChanged -= value;
    }

    public int SelectedItemsCount => this.Count(item => item.IsSelected) + this.Sum(item=>item.Children?.SelectedItemsCount ?? 0);

    public IEnumerable<object> SelectedItems => this.Where(item => item.IsSelected);
  }

}
