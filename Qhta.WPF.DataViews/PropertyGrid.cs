using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Qhta.MVVM;

namespace Qhta.WPF.DataViews
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
      valueColumn.Binding = new Binding("Value") { Mode = BindingMode.OneWay };
      valueColumn.IsReadOnly=true;
      Columns.Add(valueColumn);
    }

    private void PropertyGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (e.NewValue!=null)
        Update(e.NewValue);
    }

    public PropertyListViewModel PropertiesSource { get; private set; }

    public void Update()
    {
      Update(DataContext);
    }

    public void Update(object source)
    {
      PropertiesSource = new PropertyListViewModel();
      if (source!=null)
        PropertiesSource.AddRange(GetDisplayProperties(source).Select(item => new PropertyViewModel { Model=item, Instance=source }));
      ItemsSource=PropertiesSource;
      InvalidateVisual();
    }

    private static IEnumerable<PropertyInfo> GetDisplayProperties(object source)
    {
      return source.GetType().GetProperties().Where(item => item.GetCustomAttribute<DisplayPropertyAttribute>()!=null);
    }
  }
}
