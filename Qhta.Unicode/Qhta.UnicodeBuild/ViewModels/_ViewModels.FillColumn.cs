using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Qhta.MVVM;
using Qhta.TypeUtils;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Views;
using Qhta.WPF.Utils;

using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class _ViewModels
{
  public IRelayCommand FillColumnCommand { get; }

  private void FillColumnCommandExecute(object? sender)
  {
    if (sender is TextBlock textBlock)
    {
      if (textBlock.TemplatedParent is ContentPresenter contentPresenter)
      {
        var dataGrid = VisualTreeHelperExt.FindAncestor<SfDataGrid>(contentPresenter);
        if (dataGrid != null)
        {
          var viewRecords = dataGrid.View.Records;
          var firstItem = viewRecords.FirstOrDefault();
          if (firstItem == null) return;
          var headerCellControl = VisualTreeHelperExt.FindAncestor<GridHeaderCellControl>(contentPresenter);
          if (headerCellControl?.Column is GridComboBoxColumn comboBoxColumn)
          {
            var mappingName = comboBoxColumn.MappingName;
            var property = firstItem.Data.GetType().GetProperty(mappingName);
            if (property == null) return;
            var propertyType = property.PropertyType;
            if (propertyType == typeof(UcdBlockViewModel))
            {
              var itemsSource = _ViewModels.Instance.SelectableBlocks;
              if (itemsSource is IList itemsList)
              {
                itemsList.Insert(0, null); // Add a null item at the beginning
              }
              var selectValueWindow = new SelectValueWindow
              {
                Prompt = String.Format(Resources.Strings.SelectValueTitle, mappingName),
                ItemsSource = itemsSource
              };
              if (selectValueWindow.ShowDialog() == true)
              {
                var selectedValue = selectValueWindow.SelectedItem;
                if (selectedValue != null)
                {
                  Debug.WriteLine($"Column: {mappingName}, Selected Value: {selectedValue}");
                  foreach (var record in viewRecords)
                  {
                    if (record.Data is not null/* && record.Data.GetType().GetProperty(mappingName) is { } prop*/)
                    {
                      property.SetValue(record.Data, selectedValue);
                    }
                  }
                }
              }
            }
          }
        }
      }
    }

    bool PrintParent(object? obj)
    {
      if (obj is null) return false;
      if (obj is FrameworkElement fe)
      {
        Debug.WriteLine($"Parent: {fe.GetType().Name}");
        if (PrintParent(fe.Parent))
          return true;
        return PrintTemplatedParent(fe.TemplatedParent);
      }
      return false;
    }

    bool PrintTemplatedParent(object? obj)
    {
      if (obj is null) return false;
      if (obj is FrameworkElement fe)
      {
        Debug.WriteLine($"TemplatedParent: {fe.GetType().Name}");
        if (PrintParent(fe.Parent))
          return true;
        return PrintTemplatedParent(fe.TemplatedParent);
      }
      return false;
    }
  }
}