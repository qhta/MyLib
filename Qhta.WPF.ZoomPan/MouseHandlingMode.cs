namespace Qhta.WPF.ZoomPan
{
  /// <summary>
  /// Defines the current state of the mouse handling logic.
  /// </summary>
  public enum MouseHandlingMode
    {
        /// <summary>
        /// Not in any special mode.
        /// </summary>
        None,

        /// <summary>
        /// The user is using left mouse button to draw a selecting rectangle over the viewport or its surrounding space
        /// </summary>
        Selecting,

        /// <summary>
        /// The user is using left-mouse-button-to pan (drag) the viewport
        /// </summary>
        Panning,

        /// <summary>
        /// The user is holding down shift and left-clicking or right-clicking to zoom in or out.
        /// </summary>
        Zooming,
    }
}
