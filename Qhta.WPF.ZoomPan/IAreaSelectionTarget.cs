using System.Windows.Media;

namespace Qhta.WPF.ZoomPan
{
  public interface IAreaSelectionTarget
  {
    /// <summary>
    /// Notification of end of drawing of area selection shape.
    /// </summary>
    /// <param name="sender">object that sends the notification</param>
    /// <param name="geometry">geometry in coordinates of the target</param>
    void NotifyAreaSelection(object sender, Geometry geometry);
  }
}
