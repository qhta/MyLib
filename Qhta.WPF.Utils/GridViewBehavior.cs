using Qhta.HtmlUtils;
using Qhta.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Qhta.WPF.Utils
{
  public class GridViewBehavior
  {
    #region Public attached properties

    public static ICommand GetCommand(DependencyObject obj)
    {
      return (ICommand)obj.GetValue(CommandProperty);
    }

    public static void SetCommand(DependencyObject obj, ICommand value)
    {
      obj.SetValue(CommandProperty, value);
    }

    // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(GridViewBehavior),
            new UIPropertyMetadata(
                null,
                (o, e) =>
                {
                  ItemsControl listView = o as ItemsControl;
                  if (listView != null)
                  {
                    if (!GetSortEnabled(listView)) // Don't change click handler if SortEnabled enabled
                    {
                      if (e.OldValue != null && e.NewValue == null)
                      {
                        listView.RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                      }
                      if (e.OldValue == null && e.NewValue != null)
                      {
                        listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                      }
                    }
                  }
                }
            )
        );

    public static bool GetSortEnabled(DependencyObject obj)
    {
      return (bool)obj.GetValue(SortEnabledProperty);
    }

    public static void SetSortEnabled(DependencyObject obj, bool value)
    {
      obj.SetValue(SortEnabledProperty, value);
    }

    // Using a DependencyProperty as the backing store for SortEnabled.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SortEnabledProperty =
        DependencyProperty.RegisterAttached(
            "SortEnabled",
            typeof(bool),
            typeof(GridViewBehavior),
            new UIPropertyMetadata(
                false,
                (o, e) =>
                {
                  ListView listView = o as ListView;
                  if (listView != null)
                  {
                    if (GetCommand(listView) == null) // Don't change click handler if a command is set
                    {
                      bool oldValue = (bool)e.OldValue;
                      bool newValue = (bool)e.NewValue;
                      if (oldValue && !newValue)
                      {
                        listView.RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                      }
                      if (!oldValue && newValue)
                      {
                        listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                      }
                    }
                  }
                }
            )
        );

    public static string GetPropertyName(DependencyObject obj)
    {
      return (string)obj.GetValue(PropertyNameProperty);
    }

    public static void SetPropertyName(DependencyObject obj, string value)
    {
      obj.SetValue(PropertyNameProperty, value);
    }

    public static readonly DependencyProperty PropertyNameProperty =
        DependencyProperty.RegisterAttached(
            "PropertyName",
            typeof(string),
            typeof(GridViewBehavior),
            new UIPropertyMetadata(null)
        );

    public static Double GetHiddenWidth(DependencyObject obj)
    {
      return (Double)obj.GetValue(HiddenWidthProperty);
    }

    public static void SetHiddenWidth(DependencyObject obj, Double value)
    {
      obj.SetValue(HiddenWidthProperty, value);
    }

    public static readonly DependencyProperty HiddenWidthProperty =
        DependencyProperty.RegisterAttached(
            "HiddenWidth",
            typeof(double),
            typeof(GridViewBehavior),
            new UIPropertyMetadata(double.NaN)
        );

    public static bool GetShowSortGlyph(DependencyObject obj)
    {
      return (bool)obj.GetValue(ShowSortGlyphProperty);
    }

    public static void SetShowSortGlyph(DependencyObject obj, bool value)
    {
      obj.SetValue(ShowSortGlyphProperty, value);
    }

    // Using a DependencyProperty as the backing store for ShowSortGlyph.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ShowSortGlyphProperty =
        DependencyProperty.RegisterAttached("ShowSortGlyph", typeof(bool), typeof(GridViewBehavior), new UIPropertyMetadata(true));

    public static ImageSource GetSortGlyphAscending(DependencyObject obj)
    {
      return (ImageSource)obj.GetValue(SortGlyphAscendingProperty);
    }

    public static void SetSortGlyphAscending(DependencyObject obj, ImageSource value)
    {
      obj.SetValue(SortGlyphAscendingProperty, value);
    }

    // Using a DependencyProperty as the backing store for SortGlyphAscending.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SortGlyphAscendingProperty =
        DependencyProperty.RegisterAttached("SortGlyphAscending", typeof(ImageSource), typeof(GridViewBehavior), new UIPropertyMetadata(null));

    public static ImageSource GetSortGlyphDescending(DependencyObject obj)
    {
      return (ImageSource)obj.GetValue(SortGlyphDescendingProperty);
    }

    public static void SetSortGlyphDescending(DependencyObject obj, ImageSource value)
    {
      obj.SetValue(SortGlyphDescendingProperty, value);
    }

    // Using a DependencyProperty as the backing store for SortGlyphDescending.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SortGlyphDescendingProperty =
        DependencyProperty.RegisterAttached("SortGlyphDescending", typeof(ImageSource), typeof(GridViewBehavior), new UIPropertyMetadata(null));

    public static SortDescriptionCollection GetSortDescriptions(ListView listView)
    {
      return listView.Items.SortDescriptions;
    }

    #endregion

    //#region Private attached properties

    //private static GridViewColumnHeader GetSortedColumnHeader(DependencyObject obj)
    //{
    //  return (GridViewColumnHeader)obj.GetValue(SortedColumnHeaderProperty);
    //}

    //private static void SetSortedColumnHeader(DependencyObject obj, GridViewColumnHeader value)
    //{
    //  obj.SetValue(SortedColumnHeaderProperty, value);
    //}

    //// Using a DependencyProperty as the backing store for SortedColumn.  This enables animation, styling, binding, etc...
    //private static readonly DependencyProperty SortedColumnHeaderProperty =
    //    DependencyProperty.RegisterAttached("SortedColumnHeader", typeof(GridViewColumnHeader), typeof(GridViewSort), new UIPropertyMetadata(null));

    //#endregion

    #region Column header click event handler

    private static void ColumnHeader_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      GridViewColumnHeader clickedHeader = e.OriginalSource as GridViewColumnHeader;
      if (clickedHeader != null && clickedHeader.Column != null)
      {
        string propertyName = GetPropertyName(clickedHeader.Column);
        if (!string.IsNullOrEmpty(propertyName))
        {
          ListView listView = GetAncestor<ListView>(clickedHeader);
          if (listView != null)
          {
            ICommand command = GetCommand(listView);
            if (command != null)
            {
              if (command.CanExecute(propertyName))
              {
                command.Execute(propertyName);
              }
            }
            else if (GetSortEnabled(listView))
            {
              ApplySort(listView.Items, propertyName, listView, clickedHeader);
            }
          }
        }
      }
    }

    #endregion

    #region Helper methods

    public static T GetAncestor<T>(DependencyObject reference) where T : DependencyObject
    {
      DependencyObject parent = VisualTreeHelper.GetParent(reference);
      while (!(parent is T))
      {
        parent = VisualTreeHelper.GetParent(parent);
      }
      if (parent != null)
        return (T)parent;
      else
        return null;
    }

    public static void ApplySort(ICollectionView view, string propertyName, ListView listView, GridViewColumnHeader clickedHeader)
    {
      SortDescription clickedPropertySort = view.SortDescriptions.FirstOrDefault(item => item.PropertyName == propertyName);
      if (clickedPropertySort != default(SortDescription))
      {
        if (clickedPropertySort.Direction == ListSortDirection.Ascending)
        {
          var direction = ListSortDirection.Descending;
          view.SortDescriptions.Remove(clickedPropertySort);
          RemoveSortGlyph(clickedHeader);
          clickedPropertySort = new SortDescription(propertyName, direction);
          view.SortDescriptions.Add(clickedPropertySort);
          AddSortGlyph(clickedHeader,
                      direction,
                      direction == ListSortDirection.Ascending ? GetSortGlyphAscending(listView) : GetSortGlyphDescending(listView));
        }
        else
        if (clickedPropertySort.Direction == ListSortDirection.Descending)
        {
          view.SortDescriptions.Remove(clickedPropertySort);
          RemoveSortGlyph(clickedHeader);
        }
      }
      else
      {
        if (!Keyboard.GetKeyStates(Key.LeftShift).HasFlag(KeyStates.Down))
        {
          view.SortDescriptions.Clear();
          var headerRowPresenter = VisualTreeHelperExt.FindInVisualTreeDown<GridViewHeaderRowPresenter>(listView);
          if (headerRowPresenter != null)
          {

            int n = VisualTreeHelper.GetChildrenCount(headerRowPresenter);
            for (int i = 0; i < n; i++)
            {
              var columnHeader = VisualTreeHelper.GetChild(headerRowPresenter, i) as GridViewColumnHeader;
              if (columnHeader != null)
                RemoveSortGlyph(columnHeader);
            }
          }
        }
        var direction = ListSortDirection.Ascending;
        clickedPropertySort = new SortDescription(propertyName, direction);
        view.SortDescriptions.Add(clickedPropertySort);
        AddSortGlyph(clickedHeader,
                    direction,
                    direction == ListSortDirection.Ascending ? GetSortGlyphAscending(listView) : GetSortGlyphDescending(listView));
      }
    }

    private static void AddSortGlyph(GridViewColumnHeader columnHeader, ListSortDirection direction, ImageSource sortGlyph)
    {
      AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
      adornerLayer.Add(
          new SortGlyphAdorner(
              columnHeader,
              direction,
              sortGlyph
              ));
    }

    private static void RemoveSortGlyph(GridViewColumnHeader columnHeader)
    {
      if (columnHeader != null)
      {
        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
        Adorner[] adorners = adornerLayer.GetAdorners(columnHeader);
        if (adorners != null)
        {
          foreach (Adorner adorner in adorners)
          {
            if (adorner is SortGlyphAdorner)
            {
              adornerLayer.Remove(adorner);
            }
          }
        }
      }
    }

    #endregion

    #region SortGlyphAdorner nested class

    private class SortGlyphAdorner : Adorner
    {
      private GridViewColumnHeader _columnHeader;
      private ListSortDirection _direction;
      private ImageSource _sortGlyph;

      public SortGlyphAdorner(GridViewColumnHeader columnHeader, ListSortDirection direction, ImageSource sortGlyph)
          : base(columnHeader)
      {
        _columnHeader = columnHeader;
        _direction = direction;
        _sortGlyph = sortGlyph;
      }

      private Geometry GetDefaultGlyph()
      {
        double x1 = _columnHeader.ActualWidth - 7;
        double x2 = x1 + 4;
        double x3 = x1 + 2;
        double y1 = _columnHeader.ActualHeight - 7;
        double y2 = y1 + 4;

        if (_direction == ListSortDirection.Ascending)
        {
          double tmp = y1;
          y1 = y2;
          y2 = tmp;
        }

        PathSegmentCollection pathSegmentCollection = new PathSegmentCollection();
        pathSegmentCollection.Add(new LineSegment(new Point(x2, y1), true));
        pathSegmentCollection.Add(new LineSegment(new Point(x3, y2), true));

        PathFigure pathFigure = new PathFigure(
            new Point(x1, y1),
            pathSegmentCollection,
            true);

        PathFigureCollection pathFigureCollection = new PathFigureCollection();
        pathFigureCollection.Add(pathFigure);

        PathGeometry pathGeometry = new PathGeometry(pathFigureCollection);
        return pathGeometry;
      }

      protected override void OnRender(DrawingContext drawingContext)
      {
        base.OnRender(drawingContext);

        if (_sortGlyph != null)
        {
          double x = _columnHeader.ActualWidth - 7;
          double y = _columnHeader.ActualHeight - 7;
          Rect rect = new Rect(x, y, 4, 4);
          drawingContext.DrawImage(_sortGlyph, rect);
        }
        else
        {
          drawingContext.DrawGeometry(Brushes.Gray, new Pen(Brushes.Gray, 1.0), GetDefaultGlyph());
        }
      }
    }
    #endregion



    public static int CopyToClipboard(ListView listView, Type itemType)
    {
      var itemsView = listView.Items;
      var gridView = listView.View as GridView;
      var columns = GetColumnsViewInfo(gridView, itemType);
      SortDescriptionCollection sortDescriptions = null;
      if (GridViewBehavior.GetSortEnabled(listView))
      {
        sortDescriptions = (itemsView as ICollectionView).SortDescriptions;
      }

      StringWriter text = new StringWriter();
      StringBuilder html = new StringBuilder();
      var headers = columns.Select(item => item.Header).ToArray();
      text.WriteLine(String.Join("\t", headers));
      html.Append("<table>");
      html.Append("<tr>");
      for (int i = 0; i < headers.Count(); i++)
        html.Append($"<td>{headers[i]}</td>");
      html.Append("</tr>");
      var collection = listView.ItemsSource.Cast<ISelectable>();
      if (sortDescriptions != null)
      {
        for (int i = 0; i < sortDescriptions.Count; i++)
        {
          var sortDescription = sortDescriptions[i];
          if (i == 0)
          {
            if (sortDescription.Direction == ListSortDirection.Ascending)
              collection = collection.OrderBy(item => item.GetType().GetProperty(sortDescription.PropertyName).GetValue(item));
            else
              collection = collection.OrderByDescending(item => item.GetType().GetProperty(sortDescription.PropertyName).GetValue(item));
          }
          else
          {
            if (sortDescription.Direction == ListSortDirection.Ascending)
              collection = ((IOrderedEnumerable<ISelectable>)collection).ThenBy(item => item.GetType().GetProperty(sortDescription.PropertyName).GetValue(item));
            else
              collection = ((IOrderedEnumerable<ISelectable>)collection).ThenByDescending(item => item.GetType().GetProperty(sortDescription.PropertyName).GetValue(item));
          }
        }
      }
      int count = 0;
      foreach (var item in collection.ToList())
      {
        if (item.IsSelected)
        {
          count++;
          var values = new object[columns.Count()];
          for (int i = 0; i < columns.Count(); i++)
            values[i] = columns[i].Property.GetValue(item);
          text.WriteLine(String.Join("\t", values));
          html.Append("<tr>");
          for (int i = 0; i < values.Count(); i++)
            html.Append($"<td>{values[i]}</td>");
          html.Append("</tr>");
        }
      }
      html.Append("</table>");
      text.Flush();
      var plainText = text.ToString();
      var htmlFormat = HtmlTextUtils.FormatHtmlForClipboard(html.ToString());
      var dataObject = new DataObject();
      dataObject.SetData(DataFormats.Html, htmlFormat);
      dataObject.SetData(DataFormats.Text, plainText);
      Clipboard.SetDataObject(dataObject, true);
      return count;
    }

    public static List<GridViewColumnInfo> GetColumnsViewInfo(GridView gridView, Type itemType)
    {
      var infos = new List<GridViewColumnInfo>();
      for (int i = 0; i < gridView.Columns.Count; i++)
      {
        var column = gridView.Columns[i];
        string propertyName = null;
        var binding = column.DisplayMemberBinding;
        if (binding is Binding dataBinding)
          propertyName = dataBinding.Path.Path;
        if (propertyName == null)
          propertyName = GridViewBehavior.GetPropertyName(column);
        if (propertyName != null)
        {
          var info = new GridViewColumnInfo();
          info.PropertyName = propertyName;
          string header = column.Header?.ToString();
          info.Header = header;
          var property = itemType.GetProperty(propertyName);
          if (property == null)
            throw new InvalidOperationException($"Property \"{propertyName}\" not found in {itemType} type");
          info.Property = property;
          infos.Add(info);
        }
      }
      return infos;
    }

    public static ColumnsViewInfo GetColumnsViewInfo(DependencyObject obj)
    {
      return (ColumnsViewInfo)obj.GetValue(ColumnsViewInfoProperty);
    }

    public static void SetColumnsViewInfo(DependencyObject obj, ColumnsViewInfo value)
    {
      obj.SetValue(ColumnsViewInfoProperty, value);
    }

    public static readonly DependencyProperty ColumnsViewInfoProperty =
        DependencyProperty.RegisterAttached(
            "ColumnsViewInfo",
            typeof(ColumnsViewInfo),
            typeof(GridViewBehavior),
            new UIPropertyMetadata(null)
        );

    public static ColumnsViewInfo InitColumnsViewInfo(ListView listView)
    {
      var result = new ColumnsViewInfo();
      var gridView = listView.View as GridView;
      for (int i = 0; i < gridView.Columns.Count; i++)
      {
        var column = gridView.Columns[i];
        bool isVisible = true;
        if (column.Header is String columnHeader)
        {
          var columnWidth = GetHiddenWidth(column);
          if (Double.IsNaN(columnWidth))
          {
            columnWidth = column.Width;
          }
          else if (column.Width == 0)
            isVisible = false;
          var columnInfo = new GridViewColumnInfo { ColumnNumber = i, Header = columnHeader, IsVisible = isVisible, HiddenWidth = columnWidth};
          result.Add(columnInfo);
          var path = new PropertyPath("(0)[(1)].ActualWidth", new object[] { GridViewBehavior.ColumnsViewInfoProperty, i });
          var binding = new Binding
          {
            Path = path,
            Source = listView
          };
          BindingOperations.SetBinding(column, GridViewColumn.WidthProperty, binding);
        }
      }
      SetColumnsViewInfo(listView, result);
      return result;
    }



    public class GridViewColumnInfo: ViewModel
    {
      public int ColumnNumber { get; set; }
      public string Header { get; set; }
      public string PropertyName { get; set; }

      public PropertyInfo Property { get; set; }

      /// <summary>
      /// Property to change column visibility.
      /// A converter to Control.Visibility may be needed to bind to column Visibility.
      /// If IsVisibile is changed then ActualWidth is also changed.
      /// </summary>
      public bool IsVisible
      {
        get => _isVisible;
        set
        {
          if (_isVisible != value)
          {
            _isVisible = value;
            if (IsVisible)
              ActualWidth = HiddenWidth;
            else
              ActualWidth = 0;
            NotifyPropertyChanged(nameof(IsVisible));
          }
        }
      }
      private bool _isVisible = true;

      /// <summary>
      /// Actual width of the column. Is changed between 0 and HiddenWidth
      /// when IsVisible is changed.
      /// </summary>
      public double ActualWidth
      {
        get => _actualWidth;
        set
        {
          if (_actualWidth != value)
          {
            _actualWidth = value;
            NotifyPropertyChanged(nameof(ActualWidth));
          }
        }
      }
      private double _actualWidth = double.NaN;

      /// <summary>
      /// Hidden width of the column. Should be initialized 
      /// if ActualWidth is to be changed when IsVisible changes.
      public double HiddenWidth
      {
        get => _hiddenWidth;
        set
        {
          if (_hiddenWidth != value)
          {
            _hiddenWidth = value;
            NotifyPropertyChanged(nameof(HiddenWidth));
            if (IsVisible)
              ActualWidth = value;
          }
        }

      }
      private double _hiddenWidth = double.NaN;
    }

    public class ColumnsViewInfo : List<GridViewColumnInfo>
    {
    }

  }
}
