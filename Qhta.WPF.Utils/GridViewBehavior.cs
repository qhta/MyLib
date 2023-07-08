using Qhta.HtmlUtils;
using Qhta.MVVM;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    #region Command property

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
    #endregion

    #region SortEnabled property
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
    #endregion

    #region PropertyName property
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
    #endregion

    #region Width property
    public static Double GetWidth(DependencyObject obj)
    {
      return (Double)obj.GetValue(WidthProperty);
    }

    public static void SetWidth(DependencyObject obj, Double value)
    {
      obj.SetValue(WidthProperty, value);
    }

    public static readonly DependencyProperty WidthProperty =
        DependencyProperty.RegisterAttached(
            "Width",
            typeof(double),
            typeof(GridViewBehavior),
            new UIPropertyMetadata(double.NaN)
        );
    #endregion

    #region ShowColumnWidthChangeCursor property
    public static bool GetShowColumnWidthChangeCursor(DependencyObject obj)
    {
      return (bool)obj.GetValue(ShowColumnWidthChangeCursorProperty);
    }

    public static void SetShowColumnWidthChangeCursor(DependencyObject obj, bool value)
    {
      obj.SetValue(ShowColumnWidthChangeCursorProperty, value);
    }

    public static readonly DependencyProperty ShowColumnWidthChangeCursorProperty =
        DependencyProperty.RegisterAttached("ShowColumnWidthChangeCursor", typeof(bool), typeof(GridViewBehavior),
          new UIPropertyMetadata(
                false,
                (obj, e) =>
                {
                  ListView listView = obj as ListView;
                  GridView gridView = obj as GridView;
                  if (gridView == null && listView != null)
                    gridView = listView.View as GridView;


                  if (gridView != null)
                  {
                    bool oldValue = (bool)e.OldValue;
                    bool newValue = (bool)e.NewValue;
                    if (oldValue && !newValue)
                    {
                      listView.RemoveHandler(GridViewColumnHeader.MouseMoveEvent, new RoutedEventHandler(ColumnHeader_MouseMove));
                    }
                    if (!oldValue && newValue)
                    {
                      listView.AddHandler(GridViewColumnHeader.MouseMoveEvent, new RoutedEventHandler(ColumnHeader_MouseMove));
                    }
                  }
                }
            ));
    #endregion

    #region ColumnWidthChangeCursor property
    public static Cursor GetColumnWidthChangeCursor(DependencyObject obj)
    {
      return (Cursor)obj.GetValue(ColumnWidthChangeCursorProperty);
    }

    public static void SetColumnWidthChangeCursor(DependencyObject obj, Cursor value)
    {
      obj.SetValue(ColumnWidthChangeCursorProperty, value);
    }

    public static readonly DependencyProperty ColumnWidthChangeCursorProperty =
        DependencyProperty.RegisterAttached("ColumnWidthChangeCursor", typeof(Cursor), typeof(GridViewBehavior), new UIPropertyMetadata(Cursors.SizeWE));
    #endregion

    #region ShowSortGlyph property
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
    #endregion

    #region SortGlyphAscending property
    public static ImageSource GetSortGlyphAscending(DependencyObject obj)
    {
      return (ImageSource)obj.GetValue(SortGlyphAscendingProperty);
    }

    public static void SetSortGlyphAscending(DependencyObject obj, ImageSource value)
    {
      obj.SetValue(SortGlyphAscendingProperty, value);
    }

    public static readonly DependencyProperty SortGlyphAscendingProperty =
        DependencyProperty.RegisterAttached("SortGlyphAscending", typeof(ImageSource), typeof(GridViewBehavior), new UIPropertyMetadata(null));
    #endregion

    #region SortGlyphDescending property
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
    #endregion

    #region ColumnHeader property
    public static readonly DependencyProperty ColumnHeaderProperty =
        DependencyProperty.RegisterAttached("ColumnHeader", typeof(string), typeof(GridViewBehavior), new UIPropertyMetadata(null));

    public static string GetColumnHeader(DependencyObject obj)
    {
      return (string)obj.GetValue(ColumnHeaderProperty);
    }

    public static void SetColumnHeader(DependencyObject obj, string value)
    {
      obj.SetValue(ColumnHeaderProperty, value);
    }

    #endregion

    #region Event handlers

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

    private static void ColumnHeader_MouseMove(object sender, System.Windows.RoutedEventArgs args)
    {
      var listView = args.Source as ListView;
      var gridViewHeaderRowPresenter = VisualTreeHelperExt.FindDescentant<GridViewHeaderRowPresenter>(listView);
      if (gridViewHeaderRowPresenter != null)
      {
        var pos = Mouse.GetPosition(gridViewHeaderRowPresenter);
        //if (pos.X >= 0 && pos.Y >= 0 && pos.X < gridViewHeaderRowPresenter.ActualWidth && pos.Y < gridViewHeaderRowPresenter.ActualHeight)
        {
          Debug.WriteLine($"over gridViewHeaderRowPresenter({pos.X}, {pos.Y})");
          //  listView.
          //  GridViewColumnHeader;
          //if (header != null && header.Column != null)
          //{
          //  header.Cursor = Cursors.SizeWE;
          //}
        }
      }
    }


    #endregion

    #region Sort methods

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
      bool addSortColumn = Keyboard.GetKeyStates(Key.LeftShift).HasFlag(KeyStates.Down);
      SortDescription sortDescription = default(SortDescription);
      if (TryGetPropertyNames(propertyName, out var propertyNames, out var directions))
      {
        for (int i = 0; i < propertyNames.Count(); i++)
        {
          if (i == 0)
            sortDescription = ApplySort(view, propertyNames[i], listView, addSortColumn, directions[i]);
          else
            ApplySort(view, propertyNames[i], listView, true, directions[i]);
        }
      }
      else
      {
        sortDescription = ApplySort(view, propertyName, listView, addSortColumn, directions[0]);
      }
      if (!addSortColumn)
        RemoveAllSortGlyphs(listView);
      if (sortDescription != default(SortDescription))
      {
        RemoveSortGlyph(clickedHeader);
        var direction = sortDescription.Direction;
        AddSortGlyph(clickedHeader,
                    direction,
                    direction == ListSortDirection.Ascending ? GetSortGlyphAscending(listView) : GetSortGlyphDescending(listView));
      }
    }

    private static bool TryGetPropertyNames(string properyNames, out string[] names, out ListSortDirection[] directions)
    {
      names = properyNames.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
      directions = new ListSortDirection[names.Count()];
      for (int i = 0; i < names.Count(); i++)
        if (names[i].StartsWith("!"))
        {
          directions[i] = ListSortDirection.Descending;
          names[i] = names[i].Substring(1);
        }
      return names.Count() > 1;
    }

    public static string[] GetPropertyNames(string properyNames)
    {
      var names = properyNames.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
      for (int i = 0; i < names.Count(); i++)
        if (names[i].StartsWith("!"))
        {
          names[i] = names[i].Substring(1);
        }
      return names;
    }

    private static ListSortDirection Opposite(ListSortDirection direction)
           => direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;

    private static SortDescription ApplySort(ICollectionView view, string propertyName, ListView listView, bool addSortColumn, ListSortDirection direction)
    {
      SortDescription sortDescription = view.SortDescriptions.FirstOrDefault(item => item.PropertyName == propertyName);
      // if (property was already selected to sort)
      if (sortDescription != default(SortDescription))
      {
        // if (property was sorted according to direction)
        if (sortDescription.Direction == direction)
        {
          // switch property sort to opposite direction
          view.SortDescriptions.Remove(sortDescription);
          sortDescription = new SortDescription(propertyName, Opposite(direction));
          view.SortDescriptions.Add(sortDescription);
          return sortDescription;
        }
        else
        {
          // switch property sort to none
          view.SortDescriptions.Remove(sortDescription);
          return default(SortDescription);
        }
      }
      else
      {
        // if (not should add sort)
        if (!addSortColumn)
          // remove all sort info
          view.SortDescriptions.Clear();
        // switch property sort to ascending
        sortDescription = new SortDescription(propertyName, direction);
        view.SortDescriptions.Add(sortDescription);
        return sortDescription;
      }
    }

    private static void RemoveAllSortGlyphs(ListView listView)
    {
      var headerRowPresenter = VisualTreeHelperExt.FindDescentant<GridViewHeaderRowPresenter>(listView);
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

    #region CopyToClipboard methods
    public static int CopyToClipboard(ListView listView, Type itemType, ColumnsViewInfo columnsInfo)
    {
      var itemsView = listView.Items;
      SortDescriptionCollection sortDescriptions = null;
      if (GridViewBehavior.GetSortEnabled(listView))
      {
        sortDescriptions = (itemsView as ICollectionView).SortDescriptions;
      }
      var selectedColumns = columnsInfo.Where(item => item.IsVisible).ToList();
      StringWriter text = new StringWriter();
      StringBuilder html = new StringBuilder();
      var headers = selectedColumns.Select(item => item.Header).ToArray();
      text.WriteLine(String.Join("\t", headers));
      html.Append("<table>");
      html.Append("<tr>");
      for (int i = 0; i < headers.Count(); i++)
        html.Append($"<td><p>{HtmlUtils.HtmlTextUtils.EncodeHtmlEntities(headers[i])}</p></td>");
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
          var values = new string[selectedColumns.Count()];
          for (int i = 0; i < selectedColumns.Count(); i++)
            values[i] = selectedColumns[i].PropertyInfo.GetValue(item)?.ToString() ?? string.Empty;
          text.WriteLine(String.Join("\t", values));
          html.Append("<tr>");
          for (int i = 0; i < values.Count(); i++)
            html.Append($"<td>{HtmlUtils.HtmlTextUtils.EncodeHtmlEntities(values[i])}</td>");
          html.Append("</tr>");
        }
      }
      html.Append("</table>");
      text.Flush();
      var plainText = text.ToString();
      var htmlText = html.ToString();
      var htmlFormat = HtmlUtils.HtmlTextUtils.EncodeHtmlEntities(htmlText);
      var dataObject = new DataObject();
      dataObject.SetData(DataFormats.Html, htmlFormat);
      dataObject.SetData(DataFormats.Text, plainText);
      Clipboard.SetDataObject(dataObject, true);
      return count;
    }

    #endregion

    #region ColumnsViewInfo property and methods
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
            new UIPropertyMetadata(null, ColumnsViewInfoChanged)
        );
    public static void ColumnsViewInfoChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      //ChangeColumnsView(obj as ListView, args.NewValue as ColumnsViewInfo);
    }

    public static ColumnsViewInfo InitColumnsViewInfo(ListView listView)
    {
      var result = new ColumnsViewInfo();
      var gridView = listView.View as GridView;
      for (int i = 0; i < gridView.Columns.Count; i++)
      {
        var column = gridView.Columns[i];
        bool isVisible = true;
        var columnHeader = column.Header?.ToString();
        var columnWidth = GetWidth(column);
        if (Double.IsNaN(columnWidth))
        {
          columnWidth = column.Width;
        }
        else if (column.Width == 0)
          isVisible = false;
        var columnInfo = new ColumnViewInfo { Header = columnHeader, IsVisible = isVisible, Width = columnWidth };
        result.Add(columnInfo);
        var path = new PropertyPath("(0)[(1)].ActualWidth", new object[] { GridViewBehavior.ColumnsViewInfoProperty, i });
        var binding = new Binding
        {
          Path = path,
          Source = listView
        };
        BindingOperations.SetBinding(column, GridViewColumn.WidthProperty, binding);
      }
      SetColumnsViewInfo(listView, result);
      return result;
    }

    //public static void ChangeColumnsView(ListView listView, ColumnsViewInfo newColumns)
    //{
    //  var gridView = listView.View as GridView;
    //  var oldColumns = gridView.Columns.ToList();
    //  var columnsNeedAdd = newColumns.Count > oldColumns.Count;
    //  var columnsNeedRemove = newColumns.Count < oldColumns.Count;
    //  var columnsNeedMove = false;
    //  for (int i = 0; i < oldColumns.Count && i < newColumns.Count; i++)
    //  {
    //    if (oldColumns[i].PropertyName != newColumns[i].PropertyName)
    //    {
    //      columnsNeedMove = true;
    //      break;
    //    }
    //  }
    //  if (columnsNeedMove || columnsNeedRemove)
    //  {
    //    var oldGridColumns = gridView.Columns.ToList();
    //    gridView.Columns.Clear();
    //    var newGridColumns = new GridViewColumn[newColumns.Count];
    //    for (int i = 0; i < oldColumns.Count(); i++)
    //    {
    //      if (oldColumns[i].PropertyName == newColumns[i].PropertyName)
    //        newGridColumns[i] = oldGridColumns[i];
    //      else
    //      {
    //        for (int j = 0; j < newColumns.Count; j++)
    //        {
    //          if (oldColumns[i].PropertyName == newColumns[j].PropertyName)
    //          {
    //            newGridColumns[j] = oldGridColumns[i];
    //            break;
    //          }
    //        }
    //      }
    //    }
    //    for (int i = 0; i < newGridColumns.Count(); i++)
    //      gridView.Columns.Add(newGridColumns[i]);
    //  }
    //  for (int i = 0; i < gridView.Columns.Count; i++)
    //  {
    //    var column = gridView.Columns[i];
    //    var columnInfo = newColumns[i];
    //    var columnHeader = column.Header?.ToString();
    //    if (columnHeader != columnInfo.Header)
    //      column.Header = columnInfo.Header;
    //    if (columnInfo.IsVisible == false)
    //      column.Width = 0;
    //    else
    //    {
    //      if (!Double.IsNaN(columnInfo.Width))
    //        column.Width = columnInfo.Width;
    //    }
    //  }
    //}

    public static ColumnsViewInfo GetColumnsViewInfo(GridViewColumnCollection columns, Type itemType)
    {
      var infos = new ColumnsViewInfo();
      for (int i = 0; i < columns.Count; i++)
      {
        var column = columns[i];
        string propertyName = null;
        var binding = column.DisplayMemberBinding;
        if (binding is Binding dataBinding)
          propertyName = dataBinding.Path.Path;
        if (propertyName == null)
          propertyName = GridViewBehavior.GetPropertyName(column);
        if (propertyName != null)
        {
          var info = new ColumnViewInfo();
          info.PropertyName = propertyName;
          string header = column.Header?.ToString();
          string header2 = GetColumnHeader(column);
          if (header2 != null)
            header = header2;
          info.Header = header;
          var propertyNames = GetPropertyNames(propertyName);
          var headerStrings = header.Split(new char[] { ',', ';' });
          for (int k = 0; k < propertyNames.Count(); k++)
          {
            propertyName = propertyNames[k];
            info.PropertyName = propertyName;
            header = k < headerStrings.Count() ? headerStrings[k] : headerStrings.LastOrDefault();
            info.Header = header;

            var isVisible = true;
            var width = column.ActualWidth;
            if (Double.IsNaN(width))
            {
              width = GetWidth(column);
            }
            else if (column.ActualWidth == 0)
            {
              width = GetWidth(column);
              isVisible = false;
            }
            info.Width = width;
            info.IsVisible = isVisible;

            var property = itemType.GetProperty(propertyName);
            if (property == null)
              throw new InvalidOperationException($"Property \"{propertyName}\" not found in {itemType} type");
            info.PropertyInfo = property;
            infos.Add(info);
            info = info.Duplicate();
          }
        }
      }
      return infos;
    }

    public static void FindProperties(ColumnsViewInfo infos, Type itemType)
    {
      for (int i = 0; i < infos.Count; i++)
      {
        var info = infos[i];
        string propertyName = info.PropertyName;
        var propertyNames = GetPropertyNames(propertyName);
        for (int k = 0; k < propertyNames.Count(); k++)
        {
          propertyName = propertyNames[k];
          info.PropertyName = propertyName;
          var property = itemType.GetProperty(propertyName);
          if (property == null)
            throw new InvalidOperationException($"Property \"{propertyName}\" not found in {itemType} type");
          info.PropertyInfo = property;
        }
      }
    }
    #endregion

    #region FitLastColumnWidth
    public static readonly DependencyProperty FitLastColumnWidthProperty = DependencyProperty.RegisterAttached(
        "FitLastColumnWidth",
        typeof(bool),
        typeof(GridViewBehavior),
        new PropertyMetadata(default(bool), OnFitLastColumnWidthChanged));

    public static bool GetFitLastColumnWidth(DependencyObject target)
    {
      return (bool)target.GetValue(FitLastColumnWidthProperty);
    }

    public static void SetFitLastColumnWidth(DependencyObject target, bool value)
    {
      target.SetValue(FitLastColumnWidthProperty, value);
    }

    private static void OnFitLastColumnWidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var listView = sender as ListView;
      if (listView == null || e.NewValue == null)
        return;
      listView.Dispatcher.Invoke(() =>
      {
        listView.UpdateLayout();
        listView.FitLastColumnWidthEnable((bool)e.NewValue);
        listView.UpdateLayout();
      });
    }

    public static void FitLastColumnWidthEnable(ListView listView, bool value)
    {
      if (value)
      {
        listView.Loaded += ListView_Loaded;
        listView.SizeChanged += ListView_SizeChanged;
      }
      else
      {
        listView.Loaded -= ListView_Loaded;
        listView.SizeChanged -= ListView_SizeChanged;
      }
    }

    private static void ListView_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
      //Debug.WriteLine("ListView_Loaded");
      if (sender is ListView listView)
      {
        GridView gridView = listView.View as GridView;
        foreach (var column in gridView.Columns)
        {
          RegisterColumn(column, listView);
          (column as INotifyPropertyChanged).PropertyChanged += Column_PropertyChanged;
        }
        FitLastColumn(listView);
      }
    }

    /// <summary>
    /// Registering columns needed as there is no backward relationship between GridViewColumn an its parent ListView
    /// </summary>
    /// <param name="column"></param>
    /// <param name="listView"></param>
    private static void RegisterColumn(GridViewColumn column, ListView listView)
    {
      if (!ColumnsMapping.ContainsKey(column))
        ColumnsMapping.Add(column, listView);
    }

    private static Dictionary<GridViewColumn, ListView> ColumnsMapping = new Dictionary<GridViewColumn, ListView>();


    private static void Column_PropertyChanged(object sender, PropertyChangedEventArgs args)
    {
      //Debug.WriteLine("Column_PropertyChanged");
      if (sender is GridViewColumn column)
      {
        if (ColumnsMapping.TryGetValue(column, out var listView))
        {
          if (GetFitLastColumnWidth(listView))
          {
            GridView gridView = listView.View as GridView;
            if (column != gridView.Columns.LastOrDefault())
            {
              if (args.PropertyName == "ActualWidth")
              {
                //Debug.WriteLine($"Column_PropertyChanged({sender}, {args.PropertyName})");
                FitLastColumn(listView);
              }
            }
          }
        }
      }
    }

    private static void ListView_SizeChanged(object sender, SizeChangedEventArgs args)
    {
      if (sender is ListView listView)
        FitLastColumn(listView);
    }

    private static void FitLastColumn(ListView listView)
    {
      if (listView.View is GridView gridView)
      {
        var workingWidth = listView.ActualWidth
        - SystemParameters.VerticalScrollBarWidth
        - 10; // Additional margin set in ListView template
        double sumWidth = 0;
        int n = gridView.Columns.Count - 1;
        for (int i = 0; i < n; i++)
          sumWidth += gridView.Columns[i].ActualWidth;
        if (sumWidth < workingWidth - 20)
          gridView.Columns[n].Width = workingWidth - sumWidth;
      }
    }


    #endregion FitLastColumnWidth

  }
}
