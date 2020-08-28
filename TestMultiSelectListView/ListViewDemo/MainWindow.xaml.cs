using System.Collections.ObjectModel;
using System.Windows;

namespace TestMultiSelectListView
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {

      RootNodes = BuildListModel();
      RootNodes.CollectionChanged += SelectedNodes_CollectionChanged;
      RootNodes.PropertyChanged += RootNodes_PropertyChanged;
      InitializeComponent();
    }


    private static ListViewModel BuildListModel()
    {
      var model = new ListViewModel();
      for (int i = 1; i <= 1000; i++)
        model.Add(new ListItemViewModel($"Node {i}"));
      return model;
    }

    public ListViewModel RootNodes { get; set; }

    //public ObservableCollection<ListItemViewModel> SelectedNodes { get; set; }

    private void SelectedNodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
