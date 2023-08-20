namespace Qhta.MVVM;

/// <summary>
/// This attribute can be used in DataGrid.AutoGeneratingColumns to format a column.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class DataGridColumnAttribute : System.Attribute
{

  /// <summary>
  /// Indicates whether Find button should be displayed in the column header.
  /// This is a named property that uses nullable <see cref="CanUserResize"/> propertie.
  /// </summary>
  public bool CanUserReorderEnabled { get => CanUserReorder!=true; set=> CanUserReorder= value; }

  /// <summary>
  /// Indicates whether the user can change the column display position by dragging the column header.
  /// This is a nullable property. 
  /// If it is not set, then DataGrid CanUserReorder or 
  /// CollectionViewBehavior attached property CanUserReorder is used.
  /// </summary>
  public bool? CanUserReorder { get; set; }

  /// <summary>
  /// Indicates whether Find button should be displayed in the column header.
  /// This is a named property that uses nullable <see cref="CanUserResize"/> propertie.
  /// </summary>
  public bool CanUserResizeEnabled { get => CanUserResize!=true; set=> CanUserResize= value; }

  /// <summary>
  /// Indicates whether the user can adjust the column width by using the mouse.
  /// This is a nullable property. 
  /// If it is not set, then DataGrid CanUserResize or 
  /// CollectionViewBehavior attached property CanUserResize is used.
  /// </summary>
  public bool? CanUserResize { get; set; }

  /// <summary>
  /// Indicates whether the user can sort the column by clicking the column header
  /// This is a named property that uses nullable <see cref="CanUserSort"/> propertie.
  /// </summary>
  public bool CanUserSortEnabled { get => CanUserSort!=true; set=> CanUserSort= value; }

  /// <summary>
  /// Indicates whether the user can sort the column by clicking the column header.
  /// This is a nullable property. 
  /// If it is not set, then DataGrid CanUserSort or 
  /// CollectionViewBehavior attached property CanUserSort is used.
  /// </summary>
  public bool? CanUserSort { get; set; }

  /// <summary>
  /// IIndicates whether the user can search the column for a value provided in FindDialog.
  /// This is a named property that uses nullable <see cref="CanUserFind"/> propertie.
  /// </summary>
  public bool CanUserFindEnabled { get => CanUserFind!=true; set=> CanUserFind= value; }

  /// <summary>
  /// Indicates whether the user can search the column for a value provided in FindDialog.
  /// This is a nullable property. 
  /// If it is not set, then CollectionViewBehavior attached property CanUserFind is used.
  /// </summary>
  public bool? CanUserFind { get; set; }

  /// <summary>
  /// Indicates whether Find button should be displayed in the column header.
  /// This is a named property that uses nullable <see cref="ShowFindButton"/> property.
  /// </summary>
  public bool ShowFindButtonEnabled { get => ShowFindButton!=true; set=> ShowFindButton= value; }

  /// <summary>
  /// Indicates whether Find button should be displayed in the column header.
  /// This is a nullable property. 
  /// If it is not set, then CollectionViewBehavior attached property ShowFindButton is used.
  /// </summary>
  public bool? ShowFindButton { get; set; }


  /// <summary>
  /// Indicates whether the user can filter column with a predicate provided in FilterDialog.
  /// This is a named property that uses nullable <see cref="CanUserFilter"/> propertie.
  /// </summary>
  public bool CanUserFilterEnabled { get => CanUserFilter!=true; set=> CanUserFilter= value; }

  /// <summary>
  /// Indicates whether the user can filter column with a predicate provided in FilterDialog.
  /// This is a nullable property. 
  /// If it is not set, then CollectionViewBehavior attached property CanUserFilter is used.
  /// </summary>
  public bool? CanUserFilter { get; set; }

  /// <summary>
  /// Indicates whether Filter button should be displayed in the column header.
  /// This is a named property that uses nullable <see cref="ShowFilterButton"/> property.
  /// </summary>
  public bool ShowFilterButtonEnabled { get => ShowFilterButton!=true; set=> ShowFilterButton= value; }

  /// <summary>
  /// Indicates whether Filter button should be displayed in the column header.
  /// This is a nullable property. 
  /// If it is not set, then CollectionViewBehavior attached property ShowFilterButton is used.
  /// </summary>
  public bool? ShowFilterButton { get; set; }

  /// <summary>
  /// Specifies the binding path of properties to use when getting or setting cell content for the clipboard.
  /// Default is null.
  /// </summary>
  public string? ClipboardContentPath { get; set; }

  /// <summary>
  /// Gets or sets the display position of the column relative to the other columns in the DataGrid.
  /// Default is -1.
  /// </summary>
  public int DisplayIndex { get; set; } = -1;

  /// <summary>
  /// Gets or sets the column header text content.
  /// </summary>
  public object? Header { get; set; }

  /// <summary>
  /// Gets or sets the format pattern to apply to the content of the column header.
  /// </summary>
  public string? HeaderStringFormat { get; set; }

  /// <summary>
  /// Gets or sets the column header tooltip.
  /// </summary>
  public string? HeaderTooltip { get; set; }

  /// <summary>
  /// A key to HeaderTemplate resource that defines layout of content bound column.
  /// </summary>
  public string? HeaderTemplateResourceKey { get; set; }

  /// <summary>
  /// A key to Header content resource that defines layout of content bound column.
  /// </summary>
  public string? HeaderResourceKey { get; set; }

  /// <summary>
  /// A key to Header tooltip resource that defines layout of content bound column.
  /// </summary>
  public string? HeaderTooltipResourceKey { get; set; }

  /// <summary>
  /// Gets or sets the column hidden header string.
  /// </summary>
  public string? HiddenHeader { get; set; }

  /// <summary>
  /// A key to hidden header content resource that defines layout of content bound column.
  /// </summary>
  public string? HiddenHeaderResourceKey { get; set; }

  /// <summary>
  /// Gets a value that indicates whether the column should be auto-generated.
  /// Default is true.
  /// </summary>
  public bool IsAutoGenerated { get; set; } = true;

  /// <summary>
  /// Gets a value that indicates whether cells in the column can be edited.
  /// Default is null.
  /// </summary>
  public bool IsReadOnly { get; set; } = false;

  /// <summary>
  /// Gets or sets the maximum width constraint of the column. Default is PositiveInfinity.
  /// </summary>
  public double MaxWidth { get; set; } = double.PositiveInfinity;

  /// <summary>
  /// Gets or sets the minimum width constraint of the column. Default is 20.
  /// </summary>
  public double MinWidth { get; set; } = 20;

  /// <summary>
  /// Gets or sets the sort direction (ascending or descending) of the column. Default is null;
  /// </summary>
  public ListSortDirection? SortDirection { get; set; }

  /// <summary>
  /// Gets or sets a property name, or a period-delimited hierarchy of property names, 
  /// that indicates the member to sort by.
  /// Default is null.
  /// </summary>
  public string? SortMemberPath { get; set; }

  /// <summary>
  /// Gets or sets the visibility of the column. Default is Visible
  /// </summary>
  public Visibility Visibility { get; set; } = Visibility.Visible;

  /// <summary>
  /// Gets or sets the column width or automatic sizing mode.
  /// Default is NaN.
  /// </summary>
  public double Width { get; set; } = double.NaN;

  /// <summary>
  /// A key to DataTemplate resource that defines layout of content bound column
  /// </summary>
  public string? DataTemplateResourceKey { get; set; }

  /// <summary>
  /// A key to DataEditingTemplate resource that defines layout of content bound column
  /// </summary>
  public string? DataEditingTemplateResourceKey { get; set; }

  ///// <summary>
  ///// A key to context menu resource that defines functionality of the header.
  ///// </summary>
  //public string? HeaderContextMenuResourceKey {get; set; }
}
