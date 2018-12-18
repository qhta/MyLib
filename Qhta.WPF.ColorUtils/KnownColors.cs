using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Xml;
using Qhta.Drawing;

namespace Qhta.WPF
{

  public class KnownColors : ObservableCollection<KnownColor>
  {
    public static bool AutoSaveCustomColors {get; set;}

    public static string CustomColorsPath { get; set; }

    protected List<KnownColor> SystemColors { get; private set; } = new List<KnownColor>();

    public CustomColors CustomColors { get; private set; } = new CustomColors();

    private static KnownColors _Instance;
    public static KnownColors Instance
    {
      get
      {
        if (_Instance==null)
          _Instance = new KnownColors();
        return _Instance;
      }
    }
    /// <summary>
    /// Private constructor inhibits external instance creation
    /// </summary>
    private KnownColors()
    {
      Type ColorsType = typeof(Colors);
      PropertyInfo[] ColorsProperty = ColorsType.GetProperties();

      foreach (PropertyInfo property in ColorsProperty)
      {
        SystemColors.Add(new KnownColor
        {
          Name = property.Name,
          Color = (Color)System.Windows.Media.ColorConverter.ConvertFromString(property.Name)
        });
      }
      Sort();
      if (CustomColorsPath!=null)
      {
        LoadCustomColors(CustomColorsPath);
        foreach (var customColor in CustomColors)
        {
          this.Add(customColor);
          customColor.PropertyChanged+=CustomColor_PropertyChanged;
        }
      }
      CustomColors.CollectionChanged+=CustomColors_CollectionChanged;
    }

    private void CustomColor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName==nameof(CustomColor.Name))
        if (AutoSaveCustomColors && CustomColorsPath!=null)
          SaveCustomColors(CustomColorsPath);

    }

    public void SaveCustomColors(string filePath)
    {
      try
      {
        using (XmlWriter xmlWriter = XmlWriter.Create(filePath, new XmlWriterSettings { Indent=true }))
        {
          xmlWriter.WriteStartElement("CustomColors");
          foreach (var item in CustomColors)
          {
            xmlWriter.WriteStartElement("CustomColor");
            if (!String.IsNullOrEmpty(item.Name) && !item.Name.StartsWith("#"))
              xmlWriter.WriteAttributeString("name", item.Name);
            xmlWriter.WriteString(item.Color.ToString());
            xmlWriter.WriteEndElement();
          }
          xmlWriter.WriteEndElement();
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Error when writing to file {filePath}: {ex.Message}");
      }
    }
    public void LoadCustomColors(string filePath)
    {
      XmlDocument xmlDocument = new XmlDocument();
      try
      {
        if (File.Exists(filePath))
        {
          xmlDocument.Load(filePath);
          var docElement = xmlDocument.DocumentElement;
          if (docElement.Name=="CustomColors")
          {
            CustomColors.Clear();
            foreach (var colorElement in docElement.GetElementsByTagName("CustomColor").Cast<XmlElement>())
            {
              var colorName = colorElement.GetAttribute("name");
              var colorStr = colorElement.InnerText;
              if (colorStr!=null)
              {
                if (string.IsNullOrEmpty(colorName))
                  colorName = colorStr;
                var color = (Color)ColorConverter.ConvertFromString(colorStr);
                CustomColors.Add(new CustomColor { Name=colorName, Color=color });
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

    private void CustomColors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      switch (args.Action)
      {
        case NotifyCollectionChangedAction.Add:
          foreach (var item in args.NewItems.Cast<KnownColor>())
          {
            item.PropertyChanged+=CustomColor_PropertyChanged;
            this.Add(item);
          }
          if (AutoSaveCustomColors && CustomColorsPath!=null)
            SaveCustomColors(CustomColorsPath);
          break;
        case NotifyCollectionChangedAction.Remove:
          foreach (var item in args.OldItems.Cast<KnownColor>())
          {
            item.PropertyChanged-=CustomColor_PropertyChanged;
            this.Remove(item);
          }
          if (AutoSaveCustomColors && CustomColorsPath!=null)
            SaveCustomColors(CustomColorsPath);
          break;
      }
    }

    public void Sort()
    {
      base.Clear();
      SystemColors.Sort(KnownColorComparison);
      foreach (var item in SystemColors)
      {
        this.Add(item);
      }
    }
    private int KnownColorComparison(KnownColor first, KnownColor second)
    {
      return ColorComparer.Compare(first, second);
    }

    static ColorComparer ColorComparer = new ColorComparer();

  }

}
