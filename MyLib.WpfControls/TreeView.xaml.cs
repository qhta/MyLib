using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

public partial class TreeViewExtender
{

  private FrameworkElement FindTemplateElement(FrameworkElement aItem, string aName)
  {
    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(aItem); i++)
    {
      DependencyObject child = VisualTreeHelper.GetChild(aItem, i);
      if (child is FrameworkElement)
      {
        if (((FrameworkElement)child).Name == aName)
          return (FrameworkElement)child;
        FrameworkElement result = FindTemplateElement((FrameworkElement)child, aName);
        if (result != null)
          return result;
      }
    }
    return null;
  }

  private void InvalidateElement(FrameworkElement aElement)
  {
    aElement.InvalidateMeasure();
    Debug.WriteLine(String.Format("\"{0}\" invalidated", aElement.Name));
    FrameworkElement aParent=VisualTreeHelper.GetParent(aElement) as FrameworkElement;
    if (aParent != null)
      InvalidateElement(aParent);
  }

  public void TreeViewItemExpanded(object sender, RoutedEventArgs e)
  {
    TreeViewItem aItem = (TreeViewItem)e.OriginalSource;
    Debug.WriteLine(String.Format("\"{0}\" expanded", aItem.Header));
    FrameworkElement aLines = (FrameworkElement)aItem.Template.FindName("TreeLines", aItem);
    if (aLines != null)
    {
      InvalidateElement(aLines);
      Debug.WriteLine(String.Format("\"{0}\".Lines invalidated", aItem.Header));
    }
    aItem.InvalidateMeasure();
    aItem.InvalidateArrange();

    Control aParent = (Control)aItem.Parent;
    while (aParent is TreeViewItem)
    {
      TreeViewItem bItem = (TreeViewItem)aParent;
      aLines = FindTemplateElement(bItem,"TreeLines");
      if (aLines != null)
      {
        InvalidateElement(aLines);
        Debug.WriteLine(String.Format("\"{0}\".Lines invalidated", (aParent as TreeViewItem).Header));
      }
      aParent.InvalidateArrange();
      aParent = (Control)aParent.Parent;
    }
    aParent.InvalidateMeasure();
    aParent.InvalidateArrange();

    Debug.WriteLine(String.Format("\"{0}\" expanded handled", aItem.Header));
  }

  public void TreeViewItemCollapsed(object sender, RoutedEventArgs e)
  {
    TreeViewItemExpanded(sender, e);
  }
}

