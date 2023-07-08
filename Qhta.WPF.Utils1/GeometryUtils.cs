using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Qhta.WPF.Utils
{
  public static class GeometryUtils
  {

    /// <summary>
    /// Kąt punktu w biegunowym układzie współrzędnych
    /// </summary>
    /// <param name="p">wejściowy punkt</param>
    /// <param name="center">Środek układu współrzędnych</param>
    /// <returns></returns>
    public static double Angle(this Point p, Point center)
    {
      double dx = p.X-center.X;
      double dy = p.Y-center.Y;
      double angle = Math.Atan2(dy, dx);
      return angle;
    }

    /// <summary>
    /// Odległość punktu od środka układu współrzędnych
    /// </summary>
    /// <param name="p">wejściowy punkt</param>
    /// <param name="center">Środek układu współrzędnych</param>
    /// <returns></returns>
    public static double Radius(this Point p, Point center)
    {
      double dx = p.X-center.X;
      double dy = p.Y-center.Y;
      double radius = Math.Sqrt(dx*dx+dy*dy);
      return radius;
    }

    /// <summary>
    /// Przesunięcie współrzędnych punktu radialnie wzlędem środka o pewną odległość
    /// </summary>
    /// <param name="p">Przesuwany punkt</param>
    /// <param name="center">Środek układu współrzędnych</param>
    /// <param name="M">Odległość przesunięcia. 
    /// Wartość dodatnia oznacza zwiększenie, ujemna - zmniejszenie odległości od środka</param>
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
    /// Przesunięcie współrzędnych punktu po okręgu względem środka o pewien kąt
    /// </summary>
    /// <param name="p">Przesuwany punkt</param>
    /// <param name="center">Środek układu współrzędnych</param>
    /// <param name="A">Kąt obrotu. 
    /// Wartość dodatnia oznacza zwiększenie, ujemna - zmniejszenie odległości od środka</param>
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
    /// Utworzenie z geometrii nakładających się obszarów.
    /// </summary>
    /// <param name="geometry">geometria wejściowa</param>
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
}
