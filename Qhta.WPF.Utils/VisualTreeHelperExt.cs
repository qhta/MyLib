using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;

namespace Qhta.WPF.Utils
{
  /// <summary>
  /// Extensions for VisualTreeHelper
  /// </summary>
  public static class VisualTreeHelperExt
  {

    public static DependencyObject FindRootVisualParent(DependencyObject obj)
    {
      DependencyObject result = null;
      while (obj!=null)
      {
        obj = VisualTreeHelper.GetParent(obj);
        if (obj!=null)
        result = obj;
      }
      return result;
    }

    public static T FindRootVisualParent<T>(DependencyObject obj) where T: FrameworkElement
    {
      T result = null;
      while (obj != null)
      {
        obj = VisualTreeHelper.GetParent(obj);
        if (obj is T parent)
          result = parent;
      }
      return result;
    }

    public static T FindRootParent<T>(DependencyObject obj) where T : FrameworkElement
    {
      T result = null;
      while (obj != null)
      {
        var obj1 = VisualTreeHelper.GetParent(obj);
        if (obj1 == null && obj is FrameworkElement element)
          obj1 = element.Parent;
        obj = obj1;
        if (obj is T parent)
          result = parent;
      }
      return result;
    }
    public static T FindAncestor<T>(DependencyObject obj) where T : class
    {
      DependencyObject result = obj;
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

    public static T FindAncestor<T>(string elementName, DependencyObject obj) where T : FrameworkElement
    {
      DependencyObject result = obj;
      do
      {
        result = VisualTreeHelper.GetParent(result);
        if ((result is T element) && element.Name == elementName)
          break;
      }
      while (result != null);
      return result as T;
    }

    public static IEnumerable<T> FindAllDescendants<T>(DependencyObject obj) where T : class
    {
      var result = new List<T>();
      var c = VisualTreeHelperExt.GetChildrenCount(obj);
      for (int i = 0; i < c; i++)
      {
        var child = VisualTreeHelperExt.GetChild(obj, i);
        if (child is T)
          result.Add(child as T);
        result.AddRange(FindAllDescendants<T>(child));
      }
      return result;
    }

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

    public static DependencyObject GetChild(DependencyObject obj, int n)
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

    public static T FindDescentant<T>(DependencyObject obj) where T : class
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

    public static T FindDescendant<T>(string elementName, DependencyObject obj) where T : FrameworkElement
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
    public static T FindDescendant<T>(Type[] subtypes, DependencyObject obj) where T : class
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

   public static IInputElement FindFirstFocusableDescendant(DependencyObject obj)
    {
      IInputElement focusableElement = null;

      int count = VisualTreeHelper.GetChildrenCount(obj);
      for (int i = 0; i < count && null == focusableElement; i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(obj, i);
        IInputElement inputElement = child as IInputElement;
        if (null != inputElement && inputElement.Focusable)
        {
          focusableElement = inputElement;
        }
        else
        {
          focusableElement = FindFirstFocusableDescendant(child);
        }
      }
      return focusableElement;
    }

    public static IEnumerable<IInputElement> FindAllFocusableDescendants(DependencyObject obj)
    {
      List<IInputElement> focusableElements = new List<IInputElement>();

      int count = VisualTreeHelper.GetChildrenCount(obj);
      for (int i = 0; i < count; i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(obj, i);
        IInputElement inputElement = child as IInputElement;
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

    public static void DumpElement(DependencyObject obj, string filename)
    {
      using (XmlWriter writer = new XmlTextWriter(File.CreateText(filename)))
      {
        writer.WriteStartDocument();
        DumpElement(obj, writer);
      }
    }


    public static void DumpElement(DependencyObject obj, XmlWriter writer)
    {
      string[] propertiesToList = new string[] { "Text", "Content" };
      Type objType = obj.GetType();
      string elementName = objType.Name;
      int count = VisualTreeHelper.GetChildrenCount(obj);
      writer.WriteStartElement(elementName);
      foreach (var propName in propertiesToList)
      {
        PropertyInfo propertyInfo = objType.GetProperty(propName);
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

    public static IEnumerable<FrameworkElement> FindColumnsContent(ListViewItem listViewItem)
    {
      GridViewRowPresenter rowPresenter = FindDescentant<GridViewRowPresenter>(listViewItem);
      int colCount = 0;
      if (rowPresenter!=null)
        colCount = VisualTreeHelper.GetChildrenCount(rowPresenter);
      FrameworkElement[] result = new FrameworkElement[colCount];
      for (int i = 0; i < colCount; i++)
        result[i] = VisualTreeHelper.GetChild(rowPresenter, i) as FrameworkElement;
      return result;
    }

    public static IEnumerable<ElementType> FindColumnsContent<ElementType>(ListViewItem listViewItem) where ElementType: FrameworkElement
    {
      var columns = FindColumnsContent(listViewItem).ToArray();
      int colCount = columns.Count();
      ElementType[] result = new ElementType[colCount];
      for (int i = 0; i < colCount; i++)
        result[i] = VisualTreeHelperExt.FindDescentant<ElementType>(columns[i]);
      return result;
    }

    public static IEnumerable<ElementType> FindColumnsContent<ElementType>(Type[] subtypes, ListViewItem listViewItem) where ElementType : FrameworkElement
    {
      var columns = FindColumnsContent(listViewItem).ToArray();
      int colCount = columns.Count();
      ElementType[] result = new ElementType[colCount];
      for (int i = 0; i < colCount; i++)
      {
        result[i] = VisualTreeHelperExt.FindDescendant<ElementType>(subtypes, columns[i]);
      }
      return result;
    }

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
    public static string DebugText(this IInputElement obj) => (obj is FrameworkElement element) ? DebugText(element) : obj?.GetType().Name;

    public static string DebugText(this Object obj) => (obj is FrameworkElement element) ? DebugText(element) : obj?.GetType().Name;
  }

}
