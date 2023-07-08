namespace Qhta.WPF.DataGridUtils;

public class DataGridCollectionBoundColumn : DataGridBoundColumn
{
  public DataTemplate ContentTemplate { get; set; } = null!;

  protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
  {
    return GenerateElement(cell, dataItem);
  }

  protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
  {
    var control = new ContentControl();
    control.ContentTemplate = ContentTemplate;
    BindingOperations.SetBinding(control, ContentControl.ContentProperty, Binding);
    //BindingOperations.SetBinding(cell, DataGridCell.ContentProperty, ClipboardContentBinding);
    return control;
  }
}