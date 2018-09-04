using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MyLib.MVVM;

namespace MyLib.WPF.DataViews
{
  public class PropertyGrid: DataGrid
  {
    public PropertyGrid()
    {
      AutoGenerateColumns=false;
      DataContextChanged+=PropertyGrid_DataContextChanged;
      DataGridTextColumn nameColumn = new DataGridTextColumn();
      nameColumn.Header = "Property";
      nameColumn.Binding = new Binding("Name");
      nameColumn.IsReadOnly=true;
      Columns.Add(nameColumn);
      DataGridTextColumn valueColumn = new DataGridTextColumn();
      valueColumn.Header = "Value";
      valueColumn.Binding = new Binding("Value");
      valueColumn.IsReadOnly=true;
      Columns.Add(valueColumn);
    }

    private void PropertyGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (e.NewValue!=null)
        BuildDataGrid(e.NewValue);
    }

    public PropertyListViewModel PropertiesSource { get; private set; }

    public void BuildDataGrid(object source)
    {
      PropertiesSource = new PropertyListViewModel();
      PropertiesSource.AddRange(GetDisplayProperties(source).Select(item => new PropertyViewModel { Model=item, Instance=source }));
      ItemsSource=PropertiesSource;
    }

    private static IEnumerable<PropertyInfo> GetDisplayProperties(object source)
    {
      return source.GetType().GetProperties().Where(item => item.GetCustomAttribute<DisplayPropertyAttribute>()!=null);
    }
  }
}
