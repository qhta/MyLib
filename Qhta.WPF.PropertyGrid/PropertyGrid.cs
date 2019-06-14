using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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
            //ValueColumn.CellEditingTemplateSelector = _TemplateSelector;
          }
        }
      }
    }
    public PropertyTemplateSelector _TemplateSelector;

    public PropertyGrid()
    {
      SetColumns();
      //DataContextChanged += PropertyGrid_DataContextChanged;
    }

    private void SetColumns()
    {
      Columns.Clear();
      NameColumn = new DataGridTextColumn();
      NameColumn.Header = "Name";
      NameColumn.Binding = new Binding("Name");
      NameColumn.IsReadOnly = true;
      NameColumn.Width = 0;
      //NameColumn.Width = new DataGridLength(2, DataGridLengthUnitType.SizeToCells);
      //NameColumn.CellStyle = (Style)FindResource("PropertyGrid.NameCellStyle");

      ValueColumn = new DataGridTemplateColumn();
      ValueColumn.Header = "Value";
      //ValueColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
      ValueColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
      //ValueColumn.CellStyle = (Style)FindResource("PropertyGrid.ValueCellStyle");

      TemplateSelector = new PropertyTemplateSelector(this);
      Columns.Add(NameColumn);
      Columns.Add(ValueColumn);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      SetBinding(ItemsSourceProperty, new Binding());
      if (NameColumn!=null)
        NameColumn.CellStyle = (Style)FindResource("PropertyGrid.NameCellStyle");
      if (ValueColumn != null)
        ValueColumn.CellStyle = (Style)FindResource("PropertyGrid.ValueCellStyle");
    }

    private double nameColumnWidth = 0;
    protected override void OnLoadingRow(DataGridRowEventArgs args)
    {
      base.OnLoadingRow(args);
      Debug.WriteLine($"OnLoadingRow {args.Row.DataContext}");
      var viewModel = args.Row.DataContext as IPropertyViewModel;
      if (viewModel!=null)
      {
        var name = viewModel.Name;
        var width = TextWidth(name)+10;
        if (width > nameColumnWidth)
        {
          nameColumnWidth = width;
          NameColumn.Width = new DataGridLength(nameColumnWidth, DataGridLengthUnitType.Pixel);
        }
      }
    }

    public static double TextWidth(string str)
    {
      TextBlock textBlock = new TextBlock();
      textBlock.Text = str;
      //textBlock.FontFamily = new System.Windows.Media.FontFamily("Arial");
      //textBlock.FontSize = 40;

      Size size = ShapeMeasure(textBlock);
      return size.Width;
    }

    public static Size ShapeMeasure(TextBlock tb)
    {
      // Measured Size is bounded to be less than maxSize
      Size maxSize = new Size(
           double.PositiveInfinity,
           double.PositiveInfinity);
      tb.Measure(maxSize);
      return tb.DesiredSize;
    }


  }
}
