using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Qhta.WPF.Utils
{
  public class ListViewNavigator
  {

    #region TryUpdateFields

    public object TryUpdateField(FrameworkElement sender, ICollection collection)
    {
      if (sender is TextBox textBox)
        return TryUpdateField(sender, TextBox.TextProperty, collection);
      else
      if (sender is CheckBox checkBox)
        return TryUpdateField(sender, CheckBox.IsCheckedProperty, collection);
      throw new InvalidCastException($"Cannot update field for {sender.GetType()} type");
    }

    private object TryUpdateField(FrameworkElement sender, DependencyProperty dependencyProperty, ICollection collection)
    {
      var dictionary = collection as IDictionary;
      var targetObject = sender.DataContext;
      var updatedObject = targetObject;
      var binding = sender.GetBindingExpression(dependencyProperty);
      if (binding?.IsDirty == true)
      {
        var targetProperty = binding.TargetProperty;
        var newValue = sender.GetValue(targetProperty);
        var source = binding.ResolvedSource;
        var sourceProperty = source.GetType().GetProperty(binding.ResolvedSourcePropertyName);
        var oldValue = sourceProperty.GetValue(source);
        if (newValue != oldValue)
        {
          if (sourceProperty.GetCustomAttributes(typeof(KeyAttribute), true).FirstOrDefault() == null)
          {
            binding.UpdateSource();
          }
          else
          {
            if (dictionary == null)
              throw new ArgumentNullException("Collection argument in TryUpdateField can't be null");
            var entryTypeName = source.GetType().Name;
            if (newValue == null || newValue is String newKey && newKey == "")
            {
              var res = MessageBox.Show($"{entryTypeName} {sourceProperty.Name} cannot be empty."
                  + " Do you want to delete this entry?", "Confirm", MessageBoxButton.OKCancel);
              if (res == MessageBoxResult.OK)
              {
                dictionary.Remove(oldValue);
              }
              else
                binding.UpdateTarget();
            }
            else
            if (dictionary.Contains(newValue))
            {
              var res = MessageBox.Show($"{entryTypeName} \"{newValue}\" already exists. When you change \"{oldValue}\" to \"{newValue}\" the entries will be merged."
                  + " Do you want to continue?", "Confirm", MessageBoxButton.OKCancel);
              if (res == MessageBoxResult.OK)
              {
                {
                  binding.UpdateSource();
                  //var index = collection.IndexOf(newValue);
                  //ResultsView.SelectedIndex = index;
                  //targetObject = abbreviations[index];
                  targetObject = newValue;
                }
              }
              else
                binding.UpdateTarget();
            }

          }
        }
      }
      return updatedObject;
    }

    #endregion

    #region Navigation on key
    private bool switchSelectOnFocus = true;
    private bool switchFocusOnSelect = false;
    private int columnToSwitch = -1;

    private readonly Type[] focusableElements = new Type[] { typeof(TextBox), typeof(CheckBox) };

    public void ListViewItem_PreviewKeyDown(object sender, KeyEventArgs args)
    {
      if (sender is FrameworkElement element)
      {
        var listView = VisualTreeHelperExt.FindInVisualTreeUp<ListView>(element);
        if (listView != null)
        {
          var list = listView?.ItemsSource as IList;
          if (list == null)
            throw new InvalidCastException($"Items source of {listView} cannot be cast to IList");
          var internalElement = VisualTreeHelperExt.FindInVisualTreeDown<FrameworkElement>(focusableElements, element);
          if (internalElement != null)
          {
            var targetObject = internalElement.DataContext;
            var index = listView.Items.IndexOf(targetObject);
            listView.SelectedIndex = index;
            if (args.Key == Key.Enter)
            {
              TryUpdateField(internalElement, list);
              args.Handled = true;
            }
            else
            if (args.Key == Key.Down)
            {
              int c = list.Count;
              var updatedItem = TryUpdateField(internalElement, list);
              var i1 = listView.Items.IndexOf(updatedItem);
              if (list.Count == c)
                ListView_MoveFocus(listView, new TraversalRequest(FocusNavigationDirection.Down), updatedItem);
              else
                ListView_MoveFocusToObject(listView, updatedItem);
              args.Handled = true;
            }
            else
            if (args.Key == Key.Up)
            {
              int c = list.Count;
              var updatedItem = TryUpdateField(internalElement, list);
              if (list.Count == c)
                ListView_MoveFocus(listView, new TraversalRequest(FocusNavigationDirection.Up), updatedItem);
              else
                ListView_MoveFocusToObject(listView, updatedItem);
              args.Handled = true;
            }
            if (args.Key == Key.F2 && internalElement is TextBox textBox)
            {
              textBox.SelectAll();
              args.Handled = true;
            }
            else
            {
              var comboBox = VisualTreeHelperExt.FindInVisualTreeUp<ComboBox>(internalElement);
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

    private void ListView_MoveFocus(ListView listView, TraversalRequest request, Object selectedItem)
    {
      //  if (listView == ResultsView)
      //    ResultsView_MoveFocus(request, selectedItem);
      //  else
      //    DescriptionsView_MoveFocus(listView, request);
      //}

      //private void ResultsView_MoveFocus(TraversalRequest request, Object selectedItem)
      //{
      //  var listView = ResultsView;
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
        ListViewItem selectedItem =
            generator.ContainerFromIndex(listView.SelectedIndex) as ListViewItem;
        if (selectedItem != null)
        {
          if (/*listView == ResultsView && */columnToSwitch >= 0)
            FocusOnColumn(listView, columnToSwitch, true);
          else
          {
            var internalElement = VisualTreeHelperExt.FindInVisualTreeDown<FrameworkElement>(focusableElements, selectedItem);
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
      switchFocusOnSelect = true;
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
}
