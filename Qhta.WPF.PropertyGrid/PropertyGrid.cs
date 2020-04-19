using Qhta.WPF.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Qhta.TextUtils;
using Qhta.WPF.Utils;

namespace Qhta.WPF.PropertyGrid
{
  public class PropertyGrid : DataGrid
  {
    public DataGridTextColumn NameColumn { get; private set; }

    public DataGridTemplateColumn ValueColumn { get; private set; }

    public PropertyTemplateSelector TemplateSelector
    {
      get => _TemplateSelector;
      set
      {
        if (_TemplateSelector != value)
        {
          _TemplateSelector = value;
          if (ValueColumn != null)
          {
            ValueColumn.CellTemplateSelector = _TemplateSelector;
          }
        }
      }
    }
    public PropertyTemplateSelector _TemplateSelector;

    public PropertyGrid()
    {
      SetColumns();
    }

    private void SetColumns()
    {
      Columns.Clear();
      NameColumn = new DataGridTextColumn();
      NameColumn.Header = "Name";
      var nameBinding = new Binding("DisplayName");
      nameBinding.Converter = DisplayNameConverter = new CamelStringConverter();
      NameColumn.Binding = nameBinding;
      NameColumn.IsReadOnly = true;
      NameColumn.Width = 0;

      ValueColumn = new DataGridTemplateColumn();
      ValueColumn.Header = "Value";
      ValueColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);

      TemplateSelector = new PropertyTemplateSelector(this);
      Columns.Add(NameColumn);
      Columns.Add(ValueColumn);
    }
    private IValueConverter DisplayNameConverter;

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      SetBinding(ItemsSourceProperty, new Binding());
      if (NameColumn!=null)
        NameColumn.CellStyle = FindResource("PropertyGrid.NameCellStyle") as Style;
      if (ValueColumn != null)
        ValueColumn.CellStyle = FindResource("PropertyGrid.ValueCellStyle") as Style;
    }

    private double nameColumnWidth = 0;
    protected override void OnLoadingRow(DataGridRowEventArgs args)
    {
      base.OnLoadingRow(args);
      var viewModel = args.Row.DataContext as IPropertyViewModel;
      if (viewModel!=null)
      {
        var name = viewModel.DisplayName;
        name = (string)DisplayNameConverter.Convert(name, typeof(string), null, CultureInfo.InvariantCulture);
        var width = name.TextWidth()+10;
        if (width > nameColumnWidth)
        {
          nameColumnWidth = width;
          NameColumn.Width = new DataGridLength(nameColumnWidth, DataGridLengthUnitType.Pixel);
        }
      }
    }

  }
}
