namespace Qhta.WPF.Utils;

/// <summary>
/// Behavior class that defines properties for TreeView.
/// </summary>
public partial class TreeViewBehavior
{
  private static Dictionary<DependencyObject, TreeViewSelectedItemBehavior> behaviors = new Dictionary<DependencyObject, TreeViewSelectedItemBehavior>();

  #region SelectedItem support

  /// <summary>
  /// Getter for SelectedItem property.
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  public static object GetSelectedItem(DependencyObject obj)
  {
    return obj.GetValue(SelectedItemProperty);
  }

  /// <summary>
  /// Setter for SelectedItem property.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="value"></param>
  public static void SetSelectedItem(DependencyObject obj, object value)
  {
    if (value != null)
    {
      //Debug.WriteLine($"TreeViewItemBehavior.SetSelectedItem({value})");
      obj.SetValue(SelectedItemProperty, value);
    }
  }

  /// <summary>
  /// Dependency property to store SelectedItem property.
  /// </summary>
  public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.RegisterAttached
    ("SelectedItem", typeof(object), typeof(TreeViewBehavior),
        new UIPropertyMetadata(null, SelectedItemChanged));
  #endregion

  #region UseSelectedItemChangeEvent property

  /// <summary>
  /// Getter for UseSelectedItemChangeEvent property.
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  public static bool GetUseSelectedItemChangeEvent(DependencyObject obj)
  {
    return (bool)obj.GetValue(UseSelectedItemChangeEventProperty);
  }

  /// <summary>
  /// Setter for UseSelectedItemCHangeEvent property.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="value"></param>
  public static void SetUseSelectedItemChangeEvent(DependencyObject obj, bool value)
  {
    obj.SetValue(UseSelectedItemChangeEventProperty, value);
  }

  /// <summary>
  /// Dependency property to store UseSelectedItemChangeEvent
  /// </summary>
  public static readonly DependencyProperty UseSelectedItemChangeEventProperty = DependencyProperty.RegisterAttached
    ("UseSelectedItemChangeEvent", typeof(bool), typeof(TreeViewBehavior),
        new UIPropertyMetadata(false));
  #endregion

  #region SelectedItemChange event

  /// <summary>
  /// Add method for SelectedItemChange event handler.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="handler"></param>
  public static void AddSelectedItemChangeHandler(DependencyObject obj, RoutedEventHandler handler)
  {
    if (obj is UIElement element)
      element.AddHandler(TreeViewBehavior.SelectedItemChangeEvent, handler);
  }

  /// <summary>
  /// Remove method for SelectedItemChange event handler.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="handler"></param>
  public static void RemoveSelectedItemChangeHandler(DependencyObject obj, RoutedEventHandler handler)
  {
    if (obj is UIElement element)
      element.RemoveHandler(TreeViewBehavior.SelectedItemChangeEvent, handler);
  }

  /// <summary>
  /// RoutedEvent to store SelectedItemChange event handler.
  /// </summary>
  public static readonly RoutedEvent SelectedItemChangeEvent = EventManager.RegisterRoutedEvent
    ("SelectedItemChange", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TreeViewBehavior));


  private static void SelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
  {
    if (obj is TreeView treeView)
    {
      //Debug.WriteLine($"TreeViewBehavior.SelectedItemChanged({args.OldValue}, {args.NewValue})");
      var done = false;
      if (GetUseSelectedItemChangeEvent(treeView))
        try
        {
          treeView.RaiseEvent(new RoutedPropertyChangedEventArgs<object>(args.OldValue, args.NewValue)
          { RoutedEvent = TreeViewBehavior.SelectedItemChangeEvent, Source = obj });
          done = true;
        }
        catch
        {
          done = false;
        }
      //Debug.WriteLine($"TreeViewBehavior.SelectedItemChanged done={done}");
      if (!done)
      {
        if (!behaviors.ContainsKey(obj))
          behaviors.Add(obj, new TreeViewSelectedItemBehavior(treeView));
        TreeViewSelectedItemBehavior behavior = behaviors[obj];
        behavior.ChangeSelectedItem(args.NewValue);
      }
    }
  }
  #endregion


  #region CopyToClipboard methods

  /// <summary>
  /// Copies TreeView content fo clipboard as HTML table.
  /// </summary>
  /// <param name="treeView"></param>
  /// <param name="itemType"></param>
  /// <param name="columns"></param>
  /// <returns></returns>
  public static int CopyToClipboard(TreeView treeView, Type itemType, ColumnsViewInfo columns)
  {
    var itemsView = treeView.Items;

    StringWriter text = new StringWriter();
    StringBuilder html = new StringBuilder();
    var headers = columns.Select(item => item.Header).ToArray();
    text.WriteLine(String.Join("\t", headers));
    html.Append("<table>");
    html.Append("<tr>");
    for (int i = 0; i < headers.Count(); i++)
      html.Append($"<td><p>{HtmlUtils.HtmlTextUtils.EncodeHtmlEntities(headers[i])}</p></td>");
    html.Append("</tr>");

    int count = WriteCollection(text, html, treeView.ItemsSource.Cast<ISelectable>(), columns);

    html.Append("</table>");
    text.Flush();
    var plainText = text.ToString();
    var htmlText = html.ToString();
    var htmlFormat = HtmlUtils.HtmlTextUtils.FormatHtmlForClipboard(htmlText);
    var dataObject = new DataObject();
    dataObject.SetData(DataFormats.Html, htmlFormat);
    dataObject.SetData(DataFormats.Text, plainText);
    Clipboard.SetDataObject(dataObject, true);
    return count;
  }

  private static int WriteCollection(StringWriter text, StringBuilder html, IEnumerable<ISelectable> collection, ColumnsViewInfo columns)
  {
    int count = 0;

    foreach (var item in collection.ToList())
    {
      if (item.IsSelected)
      {
        count++;
        var values = new string[columns.Count()];
        for (int i = 0; i < columns.Count(); i++)
          values[i] = columns[i].PropertyInfo?.GetValue(item)?.ToString() ?? "";
        text.WriteLine(String.Join("\t", values));
        html.Append("<tr>");
        for (int i = 0; i < values.Count(); i++)
          html.Append($"<td>{HtmlUtils.HtmlTextUtils.EncodeHtmlEntities(values[i])}</td>");
        html.Append("</tr>");
      }
      if (item is ICompoundItem container)
      {
        count+=WriteCollection(text, html, container.Items.Cast<ISelectable>(), columns);
      }
    }
    return count;
  }
  #endregion

  #region TreeViewSelectedItemBehavior
  private class TreeViewSelectedItemBehavior
  {
    private TreeView TreeView;

    public TreeViewSelectedItemBehavior(TreeView treeView)
    {
      this.TreeView = treeView;
      treeView.SelectedItemChanged += TreeView_SelectedItemChanged;
    }

    private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> args)
    {
      if (sender is TreeView treeView)
        SetSelectedItem(treeView, args.NewValue);
    }

    /// <summary>
    ///   Recursively search for an item in this subtree.
    /// </summary>
    /// <param name="container">
    ///   The parent ItemsControl. This can be a TreeView or a TreeViewItem.
    /// </param>
    /// <param name="obj">
    ///   The object to search item for.
    /// </param>
    /// <returns>
    ///   The TreeViewItem that DataContext is the specified object.
    /// </returns>
    internal TreeViewItem? GetTreeViewItem(ItemsControl container, object obj)
    {
      if (container != null)
      {
        if (container.DataContext == obj)
        {
          return container as TreeViewItem;
        }

        // Expand the current container
        if (container is TreeViewItem && !((TreeViewItem)container).IsExpanded)
        {
          container.SetValue(TreeViewItem.IsExpandedProperty, true);
        }

        // Try to generate the ItemsPresenter and the ItemsPanel.
        // by calling ApplyTemplate.  Note that in the
        // virtualizing case even if the item is marked
        // expanded we still need to do this step in order to
        // regenerate the visuals because they may have been virtualized away.
        //Debug.WriteLine($"TreeViewBehavior.GetTreeViewItem({container}, {obj})");
        container.ApplyTemplate();
        ItemsPresenter? itemsPresenter =
            (ItemsPresenter)container.Template.FindName("ItemsHost", container);
        if (itemsPresenter != null)
        {
          itemsPresenter.ApplyTemplate();
        }
        else
        {
          // The Tree template has not named the ItemsPresenter,
          // so walk the descendents and find the child.
          itemsPresenter = VisualTreeHelperExt.FindDescendant<ItemsPresenter>(container);
          if (itemsPresenter == null)
          {
            container.UpdateLayout();

            itemsPresenter = VisualTreeHelperExt.FindDescendant<ItemsPresenter>(container);
          }
        }

        Panel itemsHostPanel = (Panel)VisualTreeHelper.GetChild(itemsPresenter, 0);

        // Ensure that the generator for this panel has been created.
        UIElementCollection children = itemsHostPanel.Children;

        VirtualizingStackPanel? virtualizingPanel =
            itemsHostPanel as VirtualizingStackPanel;

        for (int i = 0, count = container.Items.Count; i < count; i++)
        {
          TreeViewItem subContainer;
          if (virtualizingPanel != null)
          {
            // Bring the item into view so
            // that the container will be generated.
            // We must invoke virtualizingPanel.BringIndexIntoView(i)
            // through type reflection as this method is protected.
            var method = virtualizingPanel.GetType().GetMethod("BringIndexIntoView",
              BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            method?.Invoke(virtualizingPanel, new Object[] { i });


            subContainer =
                (TreeViewItem)container.ItemContainerGenerator.
                ContainerFromIndex(i);
          }
          else
          {
            subContainer =
                (TreeViewItem)container.ItemContainerGenerator.
                ContainerFromIndex(i);

            // Bring the item into view to maintain the
            // same behavior as with a virtualizing panel.
            subContainer.BringIntoView();
          }

          if (subContainer != null)
          {
            // Search the next level for the object.
            TreeViewItem? resultContainer = GetTreeViewItem(subContainer, obj);
            if (resultContainer != null)
            {
              return resultContainer;
            }
            else
            {
              // The object is not under this TreeViewItem
              // so collapse it.
              subContainer.IsExpanded = false;
            }
          }
        }
      }
      return null;
    }

    internal void ChangeSelectedItem(object obj)
    {
      //Debug.WriteLine($"TreeViewBehavior.ChangeSelectedItem({obj})");
      var item = this.GetTreeViewItem(this.TreeView, obj);
      if (item != null)
      {
        //Debug.WriteLine($"  selectedItem = {item}");
        item.IsSelected = true;
      }
    }
  }
  #endregion
}
