namespace Qhta.WPF.Utils
{
  /// <summary>
  /// Extensions for VisualTreeHelper
  /// </summary>
  public static class VisualTreeHelperExt
  {

    #region Get Visuals

    /// <summary>
    /// Gets a collection of visual children of the specified type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static List<T> GetVisualChildCollection<T>(object parent) where T : Visual
    {
      List<T> visualCollection = new List<T>();

      if (parent is DependencyObject dependencyObject)
        GetVisualChildCollection(dependencyObject, visualCollection);
      return visualCollection;
    }

    /// <summary>
    /// Gets a collection of visual children of the specified type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent"></param>
    /// <param name="visualCollection"></param>
    public static void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : Visual
    {
      int count = VisualTreeHelper.GetChildrenCount(parent);
      for (int i = 0; i < count; i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(parent, i);
        if (child is T tChild)
        {
          visualCollection.Add(tChild);
        }
        if (child != null)
        {
          GetVisualChildCollection(child, visualCollection);
        }
      }
    }

    #endregion // Get Visuals

    /// <summary>
    /// Finds a root visual parent.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static DependencyObject? FindRootVisualParent(DependencyObject obj)
    {
      DependencyObject? result = null;
      while (obj!=null)
      {
        obj = VisualTreeHelper.GetParent(obj);
        if (obj!=null)
        result = obj;
      }
      return result;
    }

    /// <summary>
    /// Finds a root visual parent of the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T? FindRootVisualParent<T>(DependencyObject obj) where T: FrameworkElement
    {
      T? result = null;
      while (obj != null)
      {
        obj = VisualTreeHelper.GetParent(obj);
        if (obj is T parent)
          result = parent;
      }
      return result;
    }

    /// <summary>
    /// Finds a root paretn of the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T? FindRootParent<T>(DependencyObject obj) where T : FrameworkElement
    {
      T? result = null;
      while (obj != null)
      {
        var obj1 = VisualTreeHelper.GetParent(obj);
        if (obj1 == null && obj is FrameworkElement element)
          obj1 = element.Parent;
        if (obj1 == null)
          return null;
        obj = obj1;
        if (obj is T parent)
          result = parent;
      }
      return result;
    }

    /// <summary>
    /// Find ancestor of the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T? FindAncestor<T>(DependencyObject obj) where T : class
    {
      DependencyObject? result = obj;
      do
      {
        var parent= VisualTreeHelper.GetParent(result);
        if (parent == null && result is FrameworkElement element)
        {
          parent = element.TemplatedParent;
          if (parent == null)
            parent = element.Parent;
        }
        result = parent;
      }
      while (result != null && !(result is T));
      return result as T;
    }

    /// <summary>
    /// Find ancestor of the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="elementName"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T? FindAncestor<T>(string elementName, DependencyObject obj) where T : FrameworkElement
    {
      DependencyObject? result = obj;
      do
      {
        result = VisualTreeHelper.GetParent(result);
        if ((result is T element) && element.Name == elementName)
          break;
      }
      while (result != null);
      return result as T;
    }

    /// <summary>
    /// Find all descendants of the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static IEnumerable<T> FindAllDescendants<T>(DependencyObject obj) where T : class
    {
      var result = new List<T>();
      var c = VisualTreeHelperExt.GetChildrenCount(obj);
      for (int i = 0; i < c; i++)
      {
        var child = VisualTreeHelperExt.GetChild(obj, i);
        if (child is T tChild)
          result.Add(tChild);
        if (child is DependencyObject dependencyChild)
          result.AddRange(FindAllDescendants<T>(dependencyChild));
      }
      return result;
    }

    /// <summary>
    /// Returns all children count.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static int GetChildrenCount(DependencyObject obj)
    {
      if (obj is TextBlock textBlock)
        return textBlock.Inlines.Count;
      else
      if (obj is Inline inline)
      {
        if (inline is AnchoredBlock block)
          return block.Blocks.Count;
        if (inline is InlineUIContainer container)
          return container.Child!=null ? 1 : 0;
        return 0;
      }
      else
        return VisualTreeHelper.GetChildrenCount(obj);
    }

    /// <summary>
    /// Get a child of the specific index.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static DependencyObject? GetChild(DependencyObject obj, int n)
    {
      if (obj is TextBlock textBlock)
        return textBlock.Inlines.ToList()[n];
      else
      if (obj is Inline inline)
      {
        if (inline is AnchoredBlock block)
          return block.Blocks.ToList()[n];
        if (inline is InlineUIContainer container)
          return container.Child;
        return null;
      }
      else
        return VisualTreeHelper.GetChild(obj, n);
    }

    /// <summary>
    /// Find first descendent of the specific type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T? FindDescentant<T>(DependencyObject obj) where T : class
    {
      var c = VisualTreeHelper.GetChildrenCount(obj);
      for (int i = 0; i < c; i++)
      {
        var child = VisualTreeHelper.GetChild(obj, i);
        if (child is T)
          return child as T;
      }
      for (int i = 0; i < c; i++)
      {
        var child = VisualTreeHelper.GetChild(obj, i);
        var result = FindDescentant<T>(child);
        if (result is T)
          return result as T;
      }
      return null;
    }

    /// <summary>
    /// Find first descendant of the specific type and specific name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="elementName"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T? FindDescendant<T>(string elementName, DependencyObject obj) where T : FrameworkElement
    {
      var c = VisualTreeHelper.GetChildrenCount(obj);
      for (int i = 0; i < c; i++)
      {
        var child = VisualTreeHelper.GetChild(obj, i);
        if (child is T element && element.Name==elementName)
          return child as T;
      }
      for (int i = 0; i < c; i++)
      {
        var child = VisualTreeHelper.GetChild(obj, i);
        var result = FindDescendant<T>(elementName, child);
        if (result is T)
          return result as T;
      }
      return null;
    }

    /// <summary>
    /// Find first descendant of one of the specific types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="subtypes"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T? FindDescendant<T>(Type[] subtypes, DependencyObject obj) where T : class
    {
      var c = VisualTreeHelper.GetChildrenCount(obj);
      for (int i = 0; i < c; i++)
      {
        var child = VisualTreeHelper.GetChild(obj, i);
        for (int j=0; j<subtypes.Count(); j++)
          if (child is T && (child.GetType()==subtypes[j] || child.GetType().IsSubclassOf(subtypes[j])))
            return child as T;
      }
      for (int i = 0; i < c; i++)
      {
        var child = VisualTreeHelper.GetChild(obj, i);
        var result = FindDescendant<T>(subtypes, child);
        if (result is T)
          return result as T;
      }
      return null;
    }

    /// <summary>
    /// First first focusable descendant.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
   public static IInputElement? FindFirstFocusableDescendant(DependencyObject obj)
    {
      IInputElement? focusableElement = null;

      int count = VisualTreeHelper.GetChildrenCount(obj);
      for (int i = 0; i < count && null == focusableElement; i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(obj, i);
        if (child is IInputElement inputElement)
            {
        if (null != inputElement && inputElement.Focusable)
        {
          focusableElement = inputElement;
        }
        else
        {
          focusableElement = FindFirstFocusableDescendant(child);
        }
      }
      }
      return focusableElement;
    }

    /// <summary>
    /// Find all focusable descenants.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static IEnumerable<IInputElement> FindAllFocusableDescendants(DependencyObject obj)
    {
      List<IInputElement> focusableElements = new List<IInputElement>();

      int count = VisualTreeHelper.GetChildrenCount(obj);
      for (int i = 0; i < count; i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(obj, i);
        IInputElement? inputElement = child as IInputElement;
        if (null != inputElement && inputElement.Focusable)
        {
          focusableElements.Add(inputElement);
        }
        else
        {
          focusableElements.AddRange(FindAllFocusableDescendants(child));
        }
      }
      return focusableElements;
    }

    /// <summary>
    /// Helper method to dump element to XAML file.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="filename"></param>
    public static void DumpElement(DependencyObject obj, string filename)
    {
      using (XmlWriter writer = new XmlTextWriter(File.CreateText(filename)))
      {
        writer.WriteStartDocument();
        DumpElement(obj, writer);
      }
    }

    /// <summary>
    /// Helper method to dump element to the specific Xml writer.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="writer"></param>
    public static void DumpElement(DependencyObject obj, XmlWriter writer)
    {
      string[] propertiesToList = new string[] { "Text", "Content" };
      Type objType = obj.GetType();
      string elementName = objType.Name;
      int count = VisualTreeHelper.GetChildrenCount(obj);
      writer.WriteStartElement(elementName);
      foreach (var propName in propertiesToList)
      {
        PropertyInfo? propertyInfo = objType.GetProperty(propName);
        if (propertyInfo != null)
        {
          var val = propertyInfo.GetValue(obj);
          if (val != null)
            writer.WriteAttributeString(propName, val.ToString());
        }
      }
      for (int i = 0; i < count; i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(obj, i);
        DumpElement(child, writer);
      }
      writer.WriteEndElement();
    }

    /// <summary>
    /// Find columns witch have content framework elements.
    /// </summary>
    /// <param name="listViewItem"></param>
    /// <returns></returns>
    public static IEnumerable<FrameworkElement> FindColumnsContent(ListViewItem listViewItem)
    {
      GridViewRowPresenter? rowPresenter = FindDescentant<GridViewRowPresenter>(listViewItem);
      int colCount = 0;
      if (rowPresenter!=null)
        colCount = VisualTreeHelper.GetChildrenCount(rowPresenter);
      FrameworkElement[] result = new FrameworkElement[colCount];
      for (int i = 0; i < colCount; i++)
        if (VisualTreeHelper.GetChild(rowPresenter, i) is FrameworkElement frameworkElement)
        result[i] = frameworkElement;
      return result;
    }

    /// <summary>
    /// Find columns witch have content framework elements.
    /// </summary>
    /// <typeparam name="ElementType"></typeparam>
    /// <param name="listViewItem"></param>
    /// <returns></returns>
    public static IEnumerable<ElementType> FindColumnsContent<ElementType>(ListViewItem listViewItem) where ElementType: FrameworkElement
    {
      var columns = FindColumnsContent(listViewItem).ToArray();
      int colCount = columns.Count();
      ElementType[] result = new ElementType[colCount];
      for (int i = 0; i < colCount; i++)
        if (VisualTreeHelperExt.FindDescentant<ElementType>(columns[i]) is ElementType element)
          result[i] = element;
      return result;
    }

    /// <summary>
    /// Find columns witch have content framework elements.
    /// </summary>
    /// <typeparam name="ElementType"></typeparam>
    /// <param name="subtypes"></param>
    /// <param name="listViewItem"></param>
    /// <returns></returns>
    public static IEnumerable<ElementType> FindColumnsContent<ElementType>(Type[] subtypes, ListViewItem listViewItem) where ElementType : FrameworkElement
    {
      var columns = FindColumnsContent(listViewItem).ToArray();
      int colCount = columns.Count();
      ElementType[] result = new ElementType[colCount];
      for (int i = 0; i < colCount; i++)
      {
        if (VisualTreeHelperExt.FindDescendant<ElementType>(subtypes, columns[i]) is ElementType element)
          result[i] = element;
      }
      return result;
    }

    /// <summary>
    /// Helper method to get debug text of the framework element.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string DebugText(FrameworkElement obj)
    {
      string s = obj.Name;
      if (String.IsNullOrEmpty(s))
        s = obj.GetType().Name;
      if (obj is TextBox textBox)
        s = s + $"({textBox.Text})";
      else
      if (obj is CheckBox checkBox)
        s = s + $"({checkBox.IsChecked})";
      return s;
    }
    /// <summary>
    /// Helper method to get debug text of the input element
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string? DebugText(this IInputElement obj) => (obj is FrameworkElement element) ? DebugText(element) : obj?.GetType().Name;

    /// <summary>
    /// Helper method to get debug text of the object.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string? DebugText(this Object obj) => (obj is FrameworkElement element) ? DebugText(element) : obj?.GetType().Name;
  }

}
