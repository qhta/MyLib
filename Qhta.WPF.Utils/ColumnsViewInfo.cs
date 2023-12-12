namespace Qhta.WPF.Utils;

/// <summary>
/// Observable collection of <see cref="ColumnViewInfo"/>.
/// </summary>
public class ColumnsViewInfo : ObservableCollection<ColumnViewInfo>
{
  /// <summary>
  /// Default constructor.
  /// </summary>
  public ColumnsViewInfo() { }

  /// <summary>
  /// Checks if all the items are selected.
  /// </summary>
  [XmlIgnore]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public bool? AllSelected
  {
    get
    {
      bool areAllSelected = true;
      bool areAllUnselected = true;
      foreach (var item in this)
      {
        if (item.IsVisible)
          areAllUnselected = false;
        else
          areAllSelected = false;
      }
      if (areAllSelected && !areAllUnselected)
        return true;
      if (!areAllSelected && areAllUnselected)
        return false;
      return null;
    }
    set
    {
      if (value == true)
      {
        foreach (var item in this)
          item.IsVisible = true;
      }
      else
      if (value == false)
      {
        foreach (var item in this)
          item.IsVisible = false;
      }

    }
  }
}
