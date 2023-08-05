namespace Qhta.WPF.DataGridUtils;

/// <summary>
/// Specific implementation of DataGridBoundColumn with <see cref="ContentTemplate"/>
/// </summary>
public class DataGridContentBoundColumn : DataGridBoundColumn
{
  /// <summary>
  /// DataTemplate to be used in <see cref="GenerateElement"/>. Must be set if WPF.
  /// Sets also <see cref="ContentEditingTemplate"/>.
  /// </summary>
  public DataTemplate ContentTemplate { get; set; } = null!;

  /// <summary>
  /// DataTemplate to be used in <see cref="GenerateEditingElement"/>. Can be set in WPF.
  /// </summary>
  public DataTemplate ContentEditingTemplate { get; set; } = null!;

  /// <summary>
  /// Overriden abstract method to generate view element in DataGrid.
  /// </summary>
  /// <param name="cell"></param>
  /// <param name="dataItem"></param>
  /// <returns></returns>
  protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
  {
    var control = new ContentControl();
    control.ContentTemplate = ContentTemplate;
    BindingOperations.SetBinding(control, ContentControl.ContentProperty, Binding);
    return control;
  }

  /// <summary>
  /// Overriden abstract method to generate editing element in DataGrid.
  /// </summary>
  /// <param name="cell"></param>
  /// <param name="dataItem"></param>
  /// <returns></returns>
  protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
  {
    var control = new ContentControl();
    control.ContentTemplate = ContentEditingTemplate ?? ContentTemplate;
    BindingOperations.SetBinding(control, ContentControl.ContentProperty, Binding);
    return control;
  }
}