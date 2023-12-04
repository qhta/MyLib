namespace Qhta.WPF.Utils;

/// <summary>
/// Utility class that contains some geometry calculation methods.
/// </summary>
public static class GeometryUtils
{

  /// <summary>
  /// The angle of the point in the polar coordinate system.
  /// </summary>
  /// <param name="p">Input point</param>
  /// <param name="center">The center of the coordinate system</param>
  /// <returns></returns>
  public static double Angle(this Point p, Point center)
  {
    double dx = p.X-center.X;
    double dy = p.Y-center.Y;
    double angle = Math.Atan2(dy, dx);
    return angle;
  }

  /// <summary>
  /// The distance of a point from the center of the coordinate system.
  /// </summary>
  /// <param name="p">Input point</param>
  /// <param name="center">The center of the coordinate system</param>
  /// <returns></returns>
  public static double Radius(this Point p, Point center)
  {
    double dx = p.X-center.X;
    double dy = p.Y-center.Y;
    double radius = Math.Sqrt(dx*dx+dy*dy);
    return radius;
  }

  /// <summary>
  /// Shift the coordinates of a point radially from the center by some distance.
  /// </summary>
  /// <param name="p">Moved point</param>
  /// <param name="center">The center of the coordinate system</param>
  /// <param name="M">Move distance</param>
  /// <returns></returns>
  public static Point Move(this Point p, Point center, double M)
  {
    double dx = p.X-center.X;
    double dy = p.Y-center.Y;
    double angle = Math.Atan2(dy, dx);
    double radius = Math.Sqrt(dx*dx+dy*dy);
    radius += M;
    var result = new Point(center.X+radius*Math.Cos(angle), center.Y+radius*Math.Sin(angle));
    return result;
  }

  /// <summary>
  /// Shifting the coordinates of a point in a circle relative to the center by some angle.
  /// </summary>
  /// <param name="p">Moved point</param>
  /// <param name="center">The center of the coordinate system</param>
  /// <param name="A">Rotation angle</param>
  /// <returns></returns>
  public static Point Rotate(this Point p, Point center, double A)
  {
    double dx = p.X-center.X;
    double dy = p.Y-center.Y;
    double angle = Math.Atan2(dy, dx);
    double radius = Math.Sqrt(dx*dx+dy*dy);
    angle += A;
    var result = new Point(center.X+radius*Math.Cos(angle), center.Y+radius*Math.Sin(angle));
    return result;
  }

  /// <summary>
  /// Merging overlapping areas from the geometry.
  /// </summary>
  /// <param name="geometry">Input geometry</param>
  /// <returns></returns>
  public static Geometry GetOverlappingGeometry(this Geometry geometry)
  {
    var geometries = new List<Geometry>();
    if (geometry is GeometryGroup geometryGroup)
      geometries.AddRange(geometryGroup.Children);
    else
      geometries.Add(geometry);
    var overlappingGeometries = new List<Geometry>();
    for (int i = 0; i<geometries.Count; i++)
      for (int j = i+1; j<geometries.Count; j++)
      {
        var g1 = geometries[i];
        var g2 = geometries[j];
        var combinedGeometry = new CombinedGeometry(GeometryCombineMode.Intersect, g1, g2);
        if (!combinedGeometry.IsEmpty())
          overlappingGeometries.Add(combinedGeometry);
      }
    var result = new GeometryGroup();
    foreach (var item in overlappingGeometries)
      result.Children.Add(item);
    return result;
  }

  /// <summary>
  /// Extracting points from the PathGeometry.
  /// </summary>
  /// <param name="geometry"></param>
  /// <returns></returns>
  public static PointCollection GetPoints(this PathGeometry geometry)
  {
    var result = new PointCollection();
    foreach (var figure in geometry.Figures)
    {
      result.Add(figure.StartPoint);
      foreach (var segment in figure.Segments)
      {
        if (segment is LineSegment line)
          result.Add(line.Point);
        else
        {
          var points = new PointCollection();
          if (segment is PolyLineSegment polyLine)
            points = polyLine.Points;
          else if (segment is PolyBezierSegment polyBezier)
            points = polyBezier.Points;
          else if (segment is PolyQuadraticBezierSegment polyQuad)
            points = polyQuad.Points;
          foreach (var point in points)
            result.Add(point);
        }
      }
    }
    return result;
  }
}
