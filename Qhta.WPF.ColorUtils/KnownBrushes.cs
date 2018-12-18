using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace Qhta.WPF
{

  public class KnownBrushes : ObservableCollection<KnownBrush>
  {
    public static bool AutoSaveCustomBrushes {get; set;}

    public static string CustomBrushesPath { get; set; }

    protected List<KnownBrush> SystemBrushes { get; private set; } = new List<KnownBrush>();

    public CustomBrushes CustomBrushes { get; private set; } = new CustomBrushes();

    private static KnownBrushes _Instance;
    public static KnownBrushes Instance
    {
      get
      {
        if (_Instance==null)
          _Instance = new KnownBrushes();
        return _Instance;
      }
    }
    /// <summary>
    /// Private constructor inhibits external instance creation
    /// </summary>
    private KnownBrushes()
    {
      Type BrushesType = typeof(Brushes);
      PropertyInfo[] BrushesProperty = BrushesType.GetProperties();

      foreach (PropertyInfo property in BrushesProperty)
      {
        SystemBrushes.Add(new KnownBrush
        {
          Name = property.Name,
          Brush = (Brush)property.GetValue(null)
        });
      }
      Sort();
      if (CustomBrushesPath!=null)
      {
        LoadCustomBrushes(CustomBrushesPath);
        foreach (var customBrush in CustomBrushes)
        {
          this.Add(customBrush);
          customBrush.PropertyChanged+=CustomBrush_PropertyChanged;
        }
      }
      CustomBrushes.CollectionChanged+=CustomBrushes_CollectionChanged;
    }

    private void CustomBrush_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName==nameof(CustomBrush.Name))
        if (AutoSaveCustomBrushes && CustomBrushesPath!=null)
          SaveCustomBrushes(CustomBrushesPath);

    }

    public void SaveCustomBrushes(string filePath)
    {
      try
      {
        using (XmlWriter xmlWriter = XmlWriter.Create(filePath, new XmlWriterSettings { Indent=true }))
        {
          xmlWriter.WriteStartElement("CustomBrushes");
          foreach (var item in CustomBrushes)
          {
            if (item.Brush is SolidColorBrush solidBrush)
            {
              xmlWriter.WriteStartElement("SolidBrush");
              if (!String.IsNullOrEmpty(item.Name) && !item.Name.StartsWith("#"))
                xmlWriter.WriteAttributeString("name", item.Name);
              xmlWriter.WriteString(solidBrush.Color.ToString());
              xmlWriter.WriteEndElement();
            }
            else if (item.Brush is LinearGradientBrush linearBrush)
            {
              xmlWriter.WriteStartElement("LinearGradientBrush");
              if (!String.IsNullOrEmpty(item.Name))
                xmlWriter.WriteAttributeString("name", item.Name);
              xmlWriter.WriteAttributeString("start", linearBrush.StartPoint.ToString(CultureInfo.InvariantCulture));
              xmlWriter.WriteAttributeString("end", linearBrush.EndPoint.ToString(CultureInfo.InvariantCulture));
              if (linearBrush.GradientStops!=null)
              {
                xmlWriter.WriteStartElement("GradientStops");
                foreach (var stop in linearBrush.GradientStops)
                {
                  xmlWriter.WriteStartElement("GradientStop");
                  xmlWriter.WriteAttributeString("offset", stop.Offset.ToString(CultureInfo.InvariantCulture));
                  xmlWriter.WriteString(stop.Color.ToString());
                  xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
              }
              xmlWriter.WriteEndElement();
            }
            else if (item.Brush is RadialGradientBrush radialBrush)
            {
              xmlWriter.WriteStartElement("RadialGradientBrush");
              if (!String.IsNullOrEmpty(item.Name))
                xmlWriter.WriteAttributeString("name", item.Name);
              xmlWriter.WriteAttributeString("origin", radialBrush.GradientOrigin.ToString(CultureInfo.InvariantCulture));
              xmlWriter.WriteAttributeString("center", radialBrush.Center.ToString(CultureInfo.InvariantCulture));
              xmlWriter.WriteAttributeString("radiusX", radialBrush.RadiusX.ToString(CultureInfo.InvariantCulture));
              xmlWriter.WriteAttributeString("radiusY", radialBrush.RadiusY.ToString(CultureInfo.InvariantCulture));
              if (radialBrush.GradientStops!=null)
              {
                xmlWriter.WriteStartElement("GradientStops");
                foreach (var stop in radialBrush.GradientStops)
                {
                  xmlWriter.WriteStartElement("GradientStop");
                  xmlWriter.WriteAttributeString("offset", stop.Offset.ToString(CultureInfo.InvariantCulture));
                  xmlWriter.WriteString(stop.Color.ToString());
                  xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
              }
              xmlWriter.WriteEndElement();
            }
          }
          xmlWriter.WriteEndElement();
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Error when writing to file {filePath}: {ex.Message}");
      }
    }
    public void LoadCustomBrushes(string filePath)
    {
      XmlDocument xmlDocument = new XmlDocument();
      try
      {
        xmlDocument.Load(filePath);
        var docElement = xmlDocument.DocumentElement;
        if (docElement.Name=="CustomBrushes")
        {
          CustomBrushes.Clear();
          foreach (var item in docElement)
            if (item is XmlElement brushElement)
            {
              var brushName = brushElement.GetAttribute("name");
              if (brushElement.Name=="SolidBrush")
              {
                var colorStr = brushElement.InnerText;
                if (colorStr!=null)
                {
                  if (string.IsNullOrEmpty(brushName))
                    brushName = colorStr;
                  var color = (Color)ColorConverter.ConvertFromString(colorStr);
                  var brush = new SolidColorBrush(color);
                  CustomBrushes.Add(new CustomBrush { Name=brushName, Brush=brush });
                }
              }
              else
              if (brushElement.Name=="LinearGradientBrush")
              {
                var startStr = brushElement.GetAttribute("start");
                var endStr = brushElement.GetAttribute("end");
                GradientStopCollection gradientStops = null;
                foreach (var gradienstStopsElement in brushElement.GetElementsByTagName("GradientStops").Cast<XmlElement>())
                {
                  gradientStops = new GradientStopCollection();
                  foreach (var gradientStopElement in gradienstStopsElement.GetElementsByTagName("GradientStop").Cast<XmlElement>())
                  {
                    var offsetStr = gradientStopElement.GetAttribute("offset");
                    var offset = Double.Parse(offsetStr, CultureInfo.InvariantCulture);
                    var colorStr = gradientStopElement.InnerText;
                    if (colorStr!=null)
                    {
                      var color = (Color)ColorConverter.ConvertFromString(colorStr);
                      var gradientStop = new GradientStop(color, offset);
                      gradientStops.Add(gradientStop);
                    }
                  }
                }
                if (gradientStops!=null)
                {
                  var brush = new LinearGradientBrush(gradientStops);
                  if (!String.IsNullOrEmpty(startStr))
                    brush.StartPoint=Point.Parse(startStr);
                  if (!String.IsNullOrEmpty(endStr))
                    brush.EndPoint=Point.Parse(endStr);
                  CustomBrushes.Add(new CustomBrush { Name=brushName, Brush=brush });
                }
              }
              else
              if (brushElement.Name=="RadialGradientBrush")
              {
                var originStr = brushElement.GetAttribute("origin");
                var centerStr = brushElement.GetAttribute("center");
                var radiusXStr = brushElement.GetAttribute("radiusX");
                var radiusYStr = brushElement.GetAttribute("radiusY");
                GradientStopCollection gradientStops = null;
                foreach (var gradienstStopsElement in brushElement.GetElementsByTagName("GradientStops").Cast<XmlElement>())
                {
                  gradientStops = new GradientStopCollection();
                  foreach (var gradientStopElement in gradienstStopsElement.GetElementsByTagName("GradientStop").Cast<XmlElement>())
                  {
                    var offsetStr = gradientStopElement.GetAttribute("offset");
                    var offset = Double.Parse(offsetStr, CultureInfo.InvariantCulture);
                    var colorStr = gradientStopElement.InnerText;
                    if (colorStr!=null)
                    {
                      var color = (Color)ColorConverter.ConvertFromString(colorStr);
                      var gradientStop = new GradientStop(color, offset);
                      gradientStops.Add(gradientStop);
                    }
                  }
                }
                if (gradientStops!=null)
                {
                  var brush = new RadialGradientBrush(gradientStops);
                  if (!String.IsNullOrEmpty(originStr))
                    brush.GradientOrigin=Point.Parse(originStr);
                  if (!String.IsNullOrEmpty(centerStr))
                    brush.Center=Point.Parse(centerStr);
                  if (!String.IsNullOrEmpty(radiusXStr))
                    brush.RadiusX=Double.Parse(radiusXStr, CultureInfo.InvariantCulture);
                  if (!String.IsNullOrEmpty(radiusYStr))
                    brush.RadiusY=Double.Parse(radiusYStr, CultureInfo.InvariantCulture);
                  CustomBrushes.Add(new CustomBrush { Name=brushName, Brush=brush });
                }
              }
            }
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Error when reading from file {filePath}: {ex.Message}");
      }
    }

    private void CustomBrushes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      switch (args.Action)
      {
        case NotifyCollectionChangedAction.Add:
          foreach (var item in args.NewItems.Cast<KnownBrush>())
          {
            item.PropertyChanged+=CustomBrush_PropertyChanged;
            this.Add(item);
          }
          if (AutoSaveCustomBrushes && CustomBrushesPath!=null)
            SaveCustomBrushes(CustomBrushesPath);
          break;
        case NotifyCollectionChangedAction.Remove:
          foreach (var item in args.OldItems.Cast<KnownBrush>())
          {
            item.PropertyChanged-=CustomBrush_PropertyChanged;
            this.Remove(item);
          }
          if (AutoSaveCustomBrushes && CustomBrushesPath!=null)
            SaveCustomBrushes(CustomBrushesPath);
          break;
      }
    }

    public void Sort()
    {
      base.Clear();
      SystemBrushes.Sort(KnownBrushComparison);
      foreach (var item in SystemBrushes)
      {
        this.Add(item);
      }
    }
    private int KnownBrushComparison(KnownBrush first, KnownBrush second)
    {
      if (first.Brush is SolidColorBrush firstBrush && second.Brush is SolidColorBrush secondBrush)
        return ColorComparer.Compare(firstBrush.Color, secondBrush.Color);
      return 0;
    }

    static ColorComparer ColorComparer = new ColorComparer();

  }

}
