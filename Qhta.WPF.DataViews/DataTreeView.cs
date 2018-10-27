using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Qhta.WPF.DataViews
{

  public partial class DataTreeView : TreeView
  {
    public DataTreeView()
    {
      //InitializeComponent();
    }

    //public static DependencyProperty ItemsSourceProperty = DependencyProperty.Register
    //("ItemsSource", typeof(IListViewModel), typeof(DataTreeView),
    //  new PropertyMetadata(null,
    //    (DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
    //    {
    //      (sender as DataTreeView).MainTreeView.ItemsSource = (args.NewValue as IListViewModel).GetItems();
    //    })
    //);

    //public IListViewModel ItemsSource
    //{
    //  get => (IListViewModel)GetValue(ItemsSourceProperty);
    //  set => SetValue(ItemsSourceProperty, value);
    //}

  }
}
