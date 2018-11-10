using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Qhta.WPF.IconDefinition;

namespace Qhta.WPF.IconControls
{
  public class IconDrawControl : Control
  {
    #region IconDef property
    public IconDef IconDef
    {
      get => (IconDef)GetValue(IconDefProperty);
      set => SetValue(IconDefProperty, value);
    }

    public static readonly DependencyProperty IconDefProperty = DependencyProperty.Register
      ("IconDef", typeof(IconDef), typeof(IconDrawControl),
        new FrameworkPropertyMetadata(null, 
          FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender)
      );
    #endregion

    protected override void OnRender(DrawingContext drawingContext)
    {
      if (IconDef==null)
        return;
      if (IconDef.Drawing==null)
        return;
      var scaleTransform = new ScaleTransform(ActualWidth/IconDef.Drawing.Width, ActualHeight/IconDef.Drawing.Height);
      drawingContext.PushTransform(scaleTransform);
      foreach (var item in IconDef.Drawing.Items)
        item.Draw(drawingContext);
      drawingContext.Pop();
    }
  }
}
