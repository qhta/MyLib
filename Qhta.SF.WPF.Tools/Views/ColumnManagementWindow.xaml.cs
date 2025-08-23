using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Qhta.SF.WPF.Tools.Views;
/// <summary>
/// Window for managing columns in a data grid.
/// </summary>
public partial class ColumnManagementWindow : Window, INotifyPropertyChanged
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ColumnManagementWindow"/> class.
  /// </summary>
  public ColumnManagementWindow()
  {
    InitializeComponent();
    Loaded += ColumnManagementWindow_OnLoaded;
  }

  private void ColumnManagementWindow_OnLoaded(object sender, RoutedEventArgs e)
  {
    Title = Qhta.SF.WPF.Tools.Resources.Strings.ColumnManagement;
    ColumnsListBox.SelectionChanged += ColumnsListBox_SelectionChanged;
    // Notify property changes after the ColumnsListBox is loaded
    NotifyPropertyChanged(nameof(IsMoveUpEnabled));
    NotifyPropertyChanged(nameof(IsMoveDownEnabled));

  }

  private void ColumnsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
  {
    NotifyPropertyChanged(nameof(IsMoveUpEnabled));
    NotifyPropertyChanged(nameof(IsMoveDownEnabled));
  }

  private void MoveUpButton_OnClick(object sender, RoutedEventArgs e)
  {
    var selectedItems = ColumnsListBox.SelectedItems.Cast<ColumnSelectionItem>().ToList();
    var itemsSource = ColumnsListBox.ItemsSource as ColumnSelectionCollection;
    if (itemsSource == null)
      return;
    foreach (var item in selectedItems)
    {
      var index = itemsSource.IndexOf(item);
      if (index > 0)
      {
        //Debug.WriteLine($"RemoveItem {item} at index {index}");
        itemsSource.RemoveAt(index);
        //Debug.WriteLine($"InsertItem {item} at index {index-1}");
        itemsSource.Insert(index - 1, item);
      }
    }
    ColumnsListBox.SelectedItems.Clear();
    foreach (var item in selectedItems)
    {
      ColumnsListBox.SelectedItems.Add(item);
    }
  }

  private void MoveDownButton_OnClick(object sender, RoutedEventArgs e)
  {
    var selectedItems = ColumnsListBox.SelectedItems.Cast<ColumnSelectionItem>().ToList();
    var itemsSource = ColumnsListBox.ItemsSource as ColumnSelectionCollection;
    if (itemsSource == null)
      return;
    for (int i=selectedItems.Count-1; i>=0; i--)
    {
      var item = selectedItems[i];
      var index = itemsSource.IndexOf(item);
      if (index < itemsSource.Count-1)
      {
        //Debug.WriteLine($"RemoveItem {item} at index {index}");
        itemsSource.RemoveAt(index);
        //Debug.WriteLine($"InsertItem {item} at index {index-1}");
        itemsSource.Insert(index + 1, item);
      }
    }
    ColumnsListBox.SelectedItems.Clear();
    foreach (var item in selectedItems)
    {
      ColumnsListBox.SelectedItems.Add(item);
    }
  }

  private void OkButton_OnClick(object sender, RoutedEventArgs e)
  {
    (DataContext as ColumnSelectionCollection)?.SetDataGridColumns();
    DialogResult = true;
    this.Close();
  }

  private void CancelButton_OnClick(object sender, RoutedEventArgs e)
  {
    DialogResult = false;
    this.Close();
  }

  #region INotifyPropertyChanged implementation
  /// <summary>
  /// Event for INotifyPropertyChanged implementation.
  /// </summary>
  public event PropertyChangedEventHandler? PropertyChanged;

  /// <summary>
  /// Method to notify that a property has changed.
  /// </summary>
  /// <param name="propertyName"></param>
  protected virtual void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  #endregion

  #region IsMoveUpEnabled and IsMoveDownEnabled properties
  /// <summary>
  /// Checks if the first item in the collection is selected.
  /// </summary>
  public bool IsMoveUpEnabled
  {
    get
    {
      var selectedItem = ColumnsListBox.SelectedItems.Cast<ColumnSelectionItem>().FirstOrDefault();
      return selectedItem != null && selectedItem != ColumnsListBox.ItemsSource?.Cast<ColumnSelectionItem>().FirstOrDefault();
    }
  }

  /// <summary>
  /// Checks if the last item in the collection is selected.
  /// </summary>
  public bool IsMoveDownEnabled
  {
    get
    {
      var selectedItem = ColumnsListBox.SelectedItems.Cast<ColumnSelectionItem>().LastOrDefault();
      return selectedItem != null && selectedItem != ColumnsListBox.ItemsSource?.Cast<ColumnSelectionItem>().LastOrDefault();
    }
  }
  #endregion
}
