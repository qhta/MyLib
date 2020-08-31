using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace TestMultiSelectTreeView
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      SelectedNodes = new ObservableCollection<TreeItemViewModel>();
      SelectedNodes.CollectionChanged += RootNodes_CollectionChanged;

      RootNodes = BuildTreeModel();
      RootNodes.CollectionChanged += RootNodes_CollectionChanged;
      RootNodes.PropertyChanged += RootNodes_PropertyChanged;

      InitializeComponent();
    }

    private static ItemsListViewModel BuildTreeModel()
    {
      var model = new ItemsListViewModel();
      for (int i = 0; i <= 9; i++)
      {
        model.Add(BuildTreeModel(i));
      }
      return model;
    }

    private static TreeItemViewModel BuildTreeModel(int n)
    {
      var model = new TreeItemViewModel($"Node {n}");
      model.IsExpanded=true;
      model.Children = new ItemsListViewModel();
      for (int i = 1; i <= 9; i++)
      {
        model.Children.Add(BuildTreeModel(n, i));
      }
      return model;
    }

    private static TreeItemViewModel BuildTreeModel(int n, int m)
    {
      var model = new TreeItemViewModel($"Node {n}.{m}");
      model.Children = new ItemsListViewModel();
      for (int i = 0; i <= 9; i++)
      {
        model.Children.Add(new TreeItemViewModel($"Node {n}.{m}.{i}"));
      }
      return model;
    }

    public ItemsListViewModel RootNodes { get; set; }

    public ObservableCollection<TreeItemViewModel> SelectedNodes { get; set; }

    private void RootNodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      this.NumberOfSelectedNodes.Text = RootNodes.SelectedItemsCount.ToString();
    }

    private void RootNodes_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      this.NumberOfSelectedNodes.Text = RootNodes.SelectedItemsCount.ToString();
    }

    private void SelectAllButton_Click(object sender, RoutedEventArgs e)
    {
      RootNodes.SelectAll(true);
      SelectAllButton.Visibility = Visibility.Collapsed;
      UnselectAllButton.Visibility = Visibility.Visible;
    }

    private void UnselectAllButton_Click(object sender, RoutedEventArgs e)
    {
      RootNodes.SelectAll(false);
      SelectAllButton.Visibility = Visibility.Visible;
      UnselectAllButton.Visibility = Visibility.Collapsed;
    }
  }
}
