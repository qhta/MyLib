using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Qhta.SF.WPF.Tools.Views;

/// <summary>
/// An adorner that indicates the drop position in a ListBox during drag-and-drop operations.
/// </summary>
public class DropPositionAdorner : Adorner
{
  private readonly ListBox _listBox;
  private readonly int _targetIndex;

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="listBox"></param>
  /// <param name="targetIndex"></param>
  public DropPositionAdorner(ListBox listBox, int targetIndex)
    : base(listBox)
  {
    _listBox = listBox;
    _targetIndex = targetIndex;
  }

  /// <summary>
  /// Method to render the adorner, drawing a line at the target index position.
  /// </summary>
  /// <param name="drawingContext"></param>
  protected override void OnRender(DrawingContext drawingContext)
  {
    base.OnRender(drawingContext);

    if (_targetIndex < 0 || _targetIndex >= _listBox.Items.Count)
      return;
    //Debug.WriteLine($"_targetIndex={_targetIndex}");
    var container = _listBox.ItemContainerGenerator.ContainerFromIndex(_targetIndex) as FrameworkElement;
    if (container == null)
      return;

    var rect = VisualTreeHelper.GetDescendantBounds(container);
    var topLeft = container.TranslatePoint(new Point(0, 0), _listBox);

    var lineY = topLeft.Y;
    var pen = new Pen(Brushes.Blue, 2);

    drawingContext.DrawLine(pen, new Point(0, lineY), new Point(_listBox.ActualWidth, lineY));
  }
}