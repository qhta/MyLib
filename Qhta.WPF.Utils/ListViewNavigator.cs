namespace Qhta.WPF.Utils;

/// <summary>
/// Helper class with several methods invoking on moving selection in ListView.
/// </summary>
public class ListViewNavigator
{

  /// <summary>
  /// Event to invoke on property update.
  /// </summary>
  public event PropUpdateEventHandler? PropUpdate;

  #region TryUpdateProperty

  /// <summary>
  /// Method that tries to invoke focusable fields in ListView.
  /// </summary>
  /// <param name="sender"></param>
  /// <returns></returns>
  /// <exception cref="InvalidCastException"></exception>
  public bool TryUpdateProperty(object sender)
  {
    //Debug.WriteLine("TryUpdateField2");
    if (sender is FrameworkElement element)
    {
      var listView = VisualTreeHelperExt.FindAncestor<ListView>(element);
      if (listView != null)
      {
        var list = listView.ItemsSource as IList;
        if (list == null)
          throw new InvalidCastException($"Items source of {listView} cannot be cast to IList");
        var internalElement = element;
        if (!focusableElements.Contains(element.GetType()))
          internalElement = VisualTreeHelperExt.FindDescendant<FrameworkElement>(focusableElements, element);
        if (internalElement != null)
        {
          var targetObject = internalElement.DataContext;
          var index = listView.Items.IndexOf(targetObject);
          listView.SelectedIndex = index;
          return TryUpdateProperty(internalElement, list, out _);
        }
      }
    }
    return false;
  }

  private bool TryUpdateProperty(FrameworkElement sender, ICollection collection, out object? updatedObject)
  {
    //Debug.WriteLine("TryUpdateField3");
    if (sender is TextBox textBox)
      return TryUpdateField(sender, TextBox.TextProperty, collection, out updatedObject);
    else
    if (sender is CheckBox checkBox)
      return TryUpdateField(sender, CheckBox.IsCheckedProperty, collection, out updatedObject);
    throw new InvalidCastException($"Cannot update field for {sender.GetType()} type");
  }

  private bool TryUpdateField(FrameworkElement sender, DependencyProperty dependencyProperty, ICollection collection, out object? updatedObject)
  {
    //Debug.WriteLine("TryUpdateField4");
    var dictionary = collection as IDictionary;
    var sourceObject = sender.DataContext;
    updatedObject = sourceObject;
    var binding = sender.GetBindingExpression(dependencyProperty);
    if (binding?.IsDirty == true)
    {
      //Debug.WriteLine("binding.IsDirty");
      var targetProperty = binding.TargetProperty;
      var newValue = sender.GetValue(targetProperty);
      var source = binding.ResolvedSource;
      var sourceProperty = source.GetType().GetProperty(binding.ResolvedSourcePropertyName);
      if (sourceProperty != null)
      {
        var oldValue = sourceProperty.GetValue(source);
        if (newValue != oldValue)
        {
          //Debug.WriteLine("newValue!=oldValue");
          var cancel = false;
          if (PropUpdate != null)
          {
            var args = new PropUpdateEventArgs(sourceObject, sourceProperty, newValue, oldValue);
            PropUpdate.Invoke(sender, args);
            updatedObject = args.TargetObject;
            cancel = args.Cancel;
          }
          if (!cancel)
          {
            binding.UpdateSource();
            return true;
          }
          else
          {
            binding.UpdateTarget();
            updatedObject = null;
            return false;
          }
        }
      }
    }
    return false;
  }

  #endregion

  #region Navigation on key down
  //private bool switchSelectOnFocus = true;
  //private bool switchFocusOnSelect = false;
  private int columnToSwitch = -1;

  private readonly Type[] focusableElements = new Type[] { typeof(TextBox), typeof(CheckBox) };

  /// <summary>
  /// Handler for ListViewItem PreviewKeyDown event.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="args"></param>
  /// <exception cref="InvalidCastException"></exception>
  public void ListViewItem_PreviewKeyDown(object sender, KeyEventArgs args)
  {
    if (sender is FrameworkElement element)
    {
      var listView = VisualTreeHelperExt.FindAncestor<ListView>(element);
      if (listView != null)
      {
        var list = listView.ItemsSource as IList;
        if (list == null)
          throw new InvalidCastException($"Items source of {listView} cannot be cast to IList");
        var internalElement = element;
        if (!focusableElements.Contains(element.GetType()))
          internalElement = VisualTreeHelperExt.FindDescendant<FrameworkElement>(focusableElements, element);
        if (internalElement != null)
        {
          var targetObject = internalElement.DataContext;
          var index = listView.Items.IndexOf(targetObject);
          listView.SelectedIndex = index;
          object? updatedObject = null;
          if (args.Key == Key.Enter)
          {
            TryUpdateProperty(internalElement, list, out updatedObject);
            args.Handled = true;
          }
          else
          if (args.Key == Key.Down || args.Key == Key.Up)
          {
            int c = list.Count;
            TryUpdateProperty(internalElement, list, out updatedObject);
            if (updatedObject != null)
            {
              if (list.Count == c)
                ListView_MoveFocus(listView, new TraversalRequest((args.Key == Key.Down) ? FocusNavigationDirection.Down : FocusNavigationDirection.Up), updatedObject);
              else
                ListView_MoveFocusToObject(listView, updatedObject);
            }
            args.Handled = true;
          }
          else
          if (args.Key == Key.F2 && internalElement is TextBox textBox)
          {
            textBox.SelectAll();
            args.Handled = true;
          }
          else
          {
            var comboBox = VisualTreeHelperExt.FindAncestor<ComboBox>(internalElement);
            if (args.Key == Key.Space && comboBox != null && !comboBox.IsDropDownOpen)
            {
              comboBox.IsDropDownOpen = true;
              args.Handled = true;
            }
          }
        }
      }
    }
  }

  /// <summary>
  /// Handler for ListView SelectionChanged event.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="args"></param>
  public void ListView_SelectionChanged(object sender, SelectionChangedEventArgs args)
  {
    if (sender is ListView listView)
    {
      if (listView != null)
      {
      }
    }
  }

  /// <summary>
  /// Handler for ListView MoveFocus event.
  /// </summary>
  /// <param name="listView"></param>
  /// <param name="request"></param>
  /// <param name="selectedItem"></param>
  private void ListView_MoveFocus(ListView listView, TraversalRequest request, Object selectedItem)
  {
    int index = listView.SelectedIndex;
    if (index == -1)
    {
      index = listView.Items.IndexOf(listView.SelectedItem);
    }
    switch (request.FocusNavigationDirection)
    {
      case FocusNavigationDirection.Down:
        {
          if (index < listView.Items.Count - 1)
          {
            listView.SelectedIndex = index + 1;
            ListView_MoveFocusToSelectedIndex(listView);
          }
        }
        break;
      case FocusNavigationDirection.Up:
        {
          if (index > 0)
          {
            listView.SelectedIndex = index - 1;
            ListView_MoveFocusToSelectedIndex(listView);
          }
        }
        break;
      case FocusNavigationDirection.Next:
        {
          if (index < listView.Items.Count - 1)
          {
            listView.SelectedIndex = index + 1;
            ListView_MoveFocusToSelectedIndex(listView);
          }
        }
        break;
      case FocusNavigationDirection.Previous:
        {
          if (index > 0)
          {
            listView.SelectedIndex = index - 1;
            ListView_MoveFocusToSelectedIndex(listView);
          }
        }
        break;
    }
  }

  //private void DescriptionsView_MoveFocus(ListView listView, TraversalRequest request)
  //{
  //  switch (request.FocusNavigationDirection)
  //  {
  //    case FocusNavigationDirection.Down:
  //      {
  //        int index = listView.SelectedIndex;
  //        if (index < listView.Items.Count - 1)
  //        {
  //          listView.SelectedIndex = index + 1;
  //          ListView_MoveFocusToSelectedIndex(listView);
  //        }
  //        else
  //        {
  //          index = ResultsView.SelectedIndex;
  //          if (ResultsView.ItemsSource is IList list)
  //          {
  //            if (index < list.Count - 1)
  //            {
  //              var selectedColumn = columnToSwitch;
  //              ListView_MoveFocus(ResultsView, new TraversalRequest(FocusNavigationDirection.Down), ResultsView.SelectedItem);
  //              columnToSwitch = selectedColumn;
  //              FocusOnColumn(ResultsView, selectedColumn, true);
  //            }
  //          }
  //        }
  //      }
  //      break;
  //    case FocusNavigationDirection.Up:
  //      {
  //        int index = listView.SelectedIndex;
  //        if (index > 0)
  //        {
  //          listView.SelectedIndex = index - 1;
  //          ListView_MoveFocusToSelectedIndex(listView);
  //        }
  //        else
  //        {
  //          index = ResultsView.SelectedIndex;
  //          if (ResultsView.ItemsSource is IList list)
  //          {
  //            if (index > 0)
  //            {
  //              var selectedColumn = columnToSwitch;
  //              ListView_MoveFocus(ResultsView, new TraversalRequest(FocusNavigationDirection.Up), ResultsView.SelectedItem);
  //              columnToSwitch = selectedColumn;
  //              FocusOnColumn(ResultsView, selectedColumn, true);
  //              listView = VisualTreeHelperExt.FindInVisualTreeUp<ListView>(Keyboard.FocusedElement as DependencyObject);
  //              DescriptionsView_MoveFocus(listView, new TraversalRequest(FocusNavigationDirection.Last));
  //            }
  //          }
  //        }
  //      }
  //      break;
  //    case FocusNavigationDirection.Last:
  //      {

  //        int index = listView.Items.Count - 1;
  //        if (index > 0)
  //        {
  //          listView.SelectedIndex = index;
  //          ListView_MoveFocusToSelectedIndex(listView);
  //        }
  //      }
  //      break;
  //  }
  //}

  private void ListView_MoveFocusToObject(ListView listView, Object targetObject)
  {
    int index = listView.Items.IndexOf(targetObject);
    if (index >= 0 && index < listView.Items.Count)
      ListView_MoveFocusToSelectedIndex(listView);
  }

  private void ListView_MoveFocusToSelectedIndex(ListView listView)
  {
    ItemContainerGenerator generator = listView.ItemContainerGenerator;
    if (listView.SelectedIndex >= 0)
    {
      ListViewItem? selectedItem =
          generator.ContainerFromIndex(listView.SelectedIndex) as ListViewItem;
      if (selectedItem != null)
      {
        if (/*listView == ResultsView && */columnToSwitch >= 0)
          FocusOnColumn(listView, columnToSwitch, true);
        else
        {
          var internalElement = VisualTreeHelperExt.FindDescendant<FrameworkElement>(focusableElements, selectedItem);
          if (internalElement != null)
          {
            internalElement.Focus();
            if (internalElement is TextBox textBox)
              textBox.SelectAll();
          }
        }
      }
    }
  }

  private void ListView_GotoIndex(ListView listView, int index)
  {
    IList items = new List<Object>(listView.ItemsSource.Cast<Object>());
    var newItem = items[index];
    //switchFocusOnSelect = true;
    listView.SelectedItem = newItem;
  }

  private bool FocusOnColumn(ListView listView, int columnIndex, bool selectAllText = false)
  {
    ItemContainerGenerator generator = listView.ItemContainerGenerator;
    if (listView.SelectedIndex >= 0)
    {
      ListViewItem selectedItem =
        (ListViewItem)generator.ContainerFromIndex(listView.SelectedIndex);
      var columns = VisualTreeHelperExt.FindColumnsContent<FrameworkElement>(focusableElements, selectedItem).ToArray();
      if (columnIndex < columns.Count() && columns[columnIndex] != null)
      {
        var focusElement = columns[columnIndex];
        if (focusElement.Focusable)
        {
          focusElement.Focus();
          if (selectAllText && focusElement is TextBox textBox)
            textBox.SelectAll();
        }
      }
    }
    return false;
  }

  #endregion
}
