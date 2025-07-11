﻿using System.Collections;
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
    if (sender is SfDataGrid dataGrid)
    {
      var column = dataGrid.CurrentColumn;
      if (column != null)
      {
        var viewRecords = dataGrid.View.Records;
        var firstItem = viewRecords.FirstOrDefault();
        if (firstItem == null) return;

        if (column is GridComboBoxColumn comboBoxColumn)
        {
          var mappingName = comboBoxColumn.MappingName;
          var property = firstItem.Data.GetType().GetProperty(mappingName);
          if (property == null) return;
          var propertyType = property.PropertyType;
          var itemsSource = comboBoxColumn.ItemsSource;
          var selectValueWindow = new SelectValueWindow
          {
            Prompt = String.Format(Resources.Strings.SelectValueTitle, mappingName),
            ItemsSource = itemsSource
          };
          if (selectValueWindow.ShowDialog() == true)
          {
            var selectedValue = selectValueWindow.SelectedItem;
            var emptyCellsOnly = selectValueWindow.EmptyCellsOnly;
            if (selectedValue != null)
            {
              //Debug.WriteLine($"Setting column: {mappingName}, Selected Value: {selectedValue}");
              foreach (var record in viewRecords)
              {
                if (record.Data is not null)
                {
                  if (emptyCellsOnly)
                  {
                    var currentValue = property.GetValue(record.Data);
                    if (currentValue == null)
                      property.SetValue(record.Data, selectedValue);
                  }
                  else
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
}