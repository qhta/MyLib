using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;
using Qhta.Drawing;

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
          foreach (var Brush in CustomBrushes)
          {
            if (Brush.Brush is SolidColorBrush solidBrush)
            {
              xmlWriter.WriteStartElement("SolidBrush");
              if (!String.IsNullOrEmpty(Brush.Name) && !Brush.Name.StartsWith("#"))
                xmlWriter.WriteAttributeString("name", Brush.Name);
              xmlWriter.WriteString(solidBrush.Color.ToString());
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
                var brushStr = brushElement.InnerText;
                if (brushStr!=null)
                {
                  if (string.IsNullOrEmpty(brushName))
                    brushName = brushStr;
                  var color = (Color)ColorConverter.ConvertFromString(brushStr);
                  var brush = new SolidColorBrush(color);
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
