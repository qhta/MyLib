using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Qhta.MVVM;

namespace Qhta.WPF.DataViews
{

  public partial class PropertyGrid : UserControl
  {
    public PropertyGrid()
    {
      InitializeComponent();
      MainDataGrid.AutoGenerateColumns=false;
      MainDataGrid.DataContextChanged+=PropertyGrid_DataContextChanged;
    }

    #region PropertyColumnHeader property
    public object PropertyColumnHeader
    {
      get => GetValue(PropertyColumnHeaderProperty);
      set => SetValue(PropertyColumnHeaderProperty, value);
    }

    public static readonly DependencyProperty PropertyColumnHeaderProperty = DependencyProperty.Register
      ("PropertyColumnHeader", typeof(object), typeof(PropertyGrid),
      new FrameworkPropertyMetadata("Property",
        FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyColumnHeaderProperty_Changed));

    private static void PropertyColumnHeaderProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as PropertyGrid).PropertyColumn.Header = args.NewValue;
    }
    #endregion

    #region PropertyColumnWidth property
    public DataGridLength PropertyColumnWidth
    {
      get => (DataGridLength)GetValue(PropertyColumnWidthProperty);
      set => SetValue(PropertyColumnWidthProperty, value);
    }

    public static DependencyProperty PropertyColumnWidthProperty = DependencyProperty.Register
      ("PropertyColumnWidth", typeof(DataGridLength), typeof(PropertyGrid),
      new FrameworkPropertyMetadata(default(DataGridLength),
        FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyColumnWidthPropertyChanged));

    private static void PropertyColumnWidthPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as PropertyGrid).PropertyColumn.Width = (DataGridLength)args.NewValue;
    }
    #endregion

    #region ValueColumnHeader property
    public object ValueColumnHeader
    {
      get => GetValue(ValueColumnHeaderProperty);
      set => SetValue(ValueColumnHeaderProperty, value);
    }

    public static readonly DependencyProperty ValueColumnHeaderProperty = DependencyProperty.Register
      ("ValueColumnHeader", typeof(object), typeof(PropertyGrid),
      new FrameworkPropertyMetadata("Value",
        FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        ValueColumnHeaderProperty_Changed));

    private static void ValueColumnHeaderProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as PropertyGrid).ValueColumn.Header = args.NewValue;
    }
    #endregion

    #region ValueColumnWidth property
    public DataGridLength ValueColumnWidth
    {
      get => (DataGridLength)GetValue(ValueColumnWidthProperty);
      set => SetValue(ValueColumnWidthProperty, value);
    }

    public static DependencyProperty ValueColumnWidthProperty = DependencyProperty.Register
      ("ValueColumnWidth", typeof(DataGridLength), typeof(PropertyGrid),
      new FrameworkPropertyMetadata(default(DataGridLength),
        FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        ValueColumnWidthPropertyChanged));

    private static void ValueColumnWidthPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as PropertyGrid).ValueColumn.Width = (DataGridLength)args.NewValue;
    }
    #endregion

    #region IsEditable property
    public bool IsEditable
    {
      get => (bool)GetValue(IsEditableProperty);
      set => SetValue(IsEditableProperty, value);
    }

    public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register
      ("IsEditable", typeof(bool), typeof(PropertyGrid),
      new FrameworkPropertyMetadata(false,
        FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        IsEditablePropertyChanged));

    private static void IsEditablePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as PropertyGrid).ValueColumn.IsReadOnly = !(bool)args.NewValue;
    }
    #endregion

    private void PropertyGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (e.NewValue!=null)
        Update(e.NewValue);
    }

    #region PropertiesSource property
    public PropertyListViewModel PropertiesSource { get; private set; }

    public void Update()
    {
      Update(DataContext);
    }

    public void Update(object source)
    {
      PropertiesSource = new PropertyListViewModel();
      if (source!=null)
        PropertiesSource.AddRange(GetDisplayProperties(source).Select(item => new PropertyViewModel(item) { Instance=source }));
      MainDataGrid.ItemsSource=PropertiesSource;
      InvalidateVisual();
    }

    private static IEnumerable<PropertyInfo> GetDisplayProperties(object source)
    {
      Type sourceType = source.GetType();
      var result = sourceType.GetProperties()
        .Where(item => item.GetCustomAttribute<DisplayPropertyAttribute>()!=null)
        .ToList();
      if (result.Count()==0 && source is DependencyObject dependencyObject)
      {
        var dependencyProperties =
          sourceType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
          .Where(item => item.FieldType==typeof(DependencyProperty))
          .ToList();
        var dependencyPropertyValues = dependencyProperties
          .Select(item => item.GetValue(null))
          .Cast<DependencyProperty>()
          .ToList();
        var dependencyPropertyNames = dependencyPropertyValues
          .Select(item => item.Name)
          .ToList();
        result = sourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
          .Where(item => dependencyPropertyNames.Contains(item.Name))
          .ToList();
      }
      return result;
    }
    #endregion

    private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs args)
    {
      var comboBox = sender as ComboBox;
      foreach (var item in args.AddedItems)
      {
        comboBox.Text = item.ToString();
        BindingOperations.GetBindingExpressionBase(comboBox, ComboBox.TextProperty)?.UpdateSource();
        break;
      }

    }
  }
}
