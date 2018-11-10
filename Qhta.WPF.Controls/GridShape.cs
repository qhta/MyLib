using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Qhta.WPF.Controls
{
  public class GridShape: Shape
  {
    #region ResX
    public int ResX
    {
      get =>
          ((int)base.GetValue(ResXProperty));
      set =>
          base.SetValue(ResXProperty, value);
    }

    public static readonly DependencyProperty ResXProperty = DependencyProperty.Register
      ("ResX", typeof(int), typeof(GridShape),
      new PropertyMetadata(8));
    #endregion

    #region ResY
    public int ResY
    {
      get =>
          ((int)base.GetValue(ResYProperty));
      set =>
          base.SetValue(ResYProperty, value);
    }

    public static readonly DependencyProperty ResYProperty = DependencyProperty.Register
      ("ResY", typeof(int), typeof(GridShape),
      new PropertyMetadata(8));
    #endregion

    protected override Geometry DefiningGeometry
    {
      get
      {
        var group = new GeometryGroup();
        for (int y = 0; y<=ResY; y++)
        {
          double Y = ActualHeight/ResY*y;
          var line = new LineGeometry(new Point(0, Y), new Point(ActualWidth, Y));
          group.Children.Add(line);
        }
        for (int x = 0; x<=ResY; x++)
        {
          double X = ActualWidth/ResX*x;
          var line = new LineGeometry(new Point(X, 0), new Point(X, ActualHeight));
          group.Children.Add(line);
        }
        return group;
      }
    }

  }
}
