using System.Windows.Controls;

namespace Qhta.UnicodeBuild.Helpers;

public static class TreeViewHelper
{
  public static void SetSelectedItemInTreeView(this TreeView treeView, object selectedItem)
  {
    var treeViewItem = FindTreeViewItem(treeView, selectedItem);
    if (treeViewItem != null)
    {
      treeViewItem.IsSelected = true;
      treeViewItem.BringIntoView();
    }
  }

  public static TreeViewItem? FindTreeViewItem(ItemsControl? container, object item)
  {
    if (container == null)
      return null;

    if (container.DataContext.Equals(item))
      return container as TreeViewItem;

    for (int i = 0; i < container.Items.Count; i++)
    {
      if (container.ItemContainerGenerator.ContainerFromIndex(i) is TreeViewItem childContainer)
      {
        childContainer.IsExpanded = true;
        childContainer.UpdateLayout();

        var result = FindTreeViewItem(childContainer, item);
        if (result != null)
          return result;
      }
    }
    return null;
  }

}