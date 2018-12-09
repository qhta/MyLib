using System.Windows.Media;
using Qhta.WPF.Utils;

namespace Qhta.WPF.Controls
{
  public static class UndoManagers
  {
    public static readonly UndoManager<Brush> BrushUndoManager = new UndoManager<Brush>();
  }
}
