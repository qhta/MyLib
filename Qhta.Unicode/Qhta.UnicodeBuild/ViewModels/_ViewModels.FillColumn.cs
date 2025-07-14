using System.Collections;

using Qhta.MVVM;
using Qhta.UnicodeBuild.Views;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class _ViewModels
{
  /// <summary>
  /// Command to fill the current column in the data grid with a selected value.
  /// </summary>
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