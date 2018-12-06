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
          foreach (var color in CustomColors)
          {
            xmlWriter.WriteStartElement("CustomColor");
            if (!String.IsNullOrEmpty(color.Name) && !color.Name.StartsWith("#"))
              xmlWriter.WriteAttributeString("name", color.Name);
            xmlWriter.WriteString(color.Color.ToString());
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
      var hsv1 = first.Color.ToDrawingColor().ToAhsv();
      var hsv2 = second.Color.ToDrawingColor().ToAhsv();
      return CompareAlpha(hsv1, hsv2);
    }

    private int CompareAlpha(AhsvColor hsv1, AhsvColor hsv2)
    {
      if (hsv1.A<hsv2.A)
        return -1;
      else
      if (hsv1.A>hsv2.A)
        return +1;
      return CompareAchromatic(hsv1, hsv2);
    }

    private int CompareAchromatic(AhsvColor hsv1, AhsvColor hsv2)
    {
      // Compare achromatic colors
      if (hsv1.S==0 && hsv2.S==0)
        return -hsv1.V.CompareTo(hsv2.V);
      if (hsv1.S==0 && hsv2.S>0)
        return -1;
      if (hsv1.S>0 && hsv2.S==0)
        return +1;
      return ComparePastel(hsv1, hsv2);
    }

    private int ComparePastel(AhsvColor hsv1, AhsvColor hsv2)
    {
      //Compare pastele colors
      var p1 = ToPastelFamily(hsv1);
      var p2 = ToPastelFamily(hsv2);
      if (p1>p2)
        return +1;
      if (p1<p2)
        return -1;
      if (p1==p2 && p1==0)
        return CompareSaturation(hsv1, hsv2);
      return CompareHue(hsv1, hsv2);
    }

    private int CompareSaturation(AhsvColor hsv1, AhsvColor hsv2)
    {
      if (hsv1.S==hsv2.S)
        return hsv2.V.CompareTo(hsv1.V);
      return CompareHue(hsv1, hsv2);
    }

    private int ToPastelFamily(AhsvColor hsv)
    {
      var p = Math.Sqrt(hsv.S*hsv.S+(1-hsv.V)*(1-hsv.H));
      if (p<=0.33)
        return 0;
      return 1;
    }

    private int CompareValue(AhsvColor hsv1, AhsvColor hsv2)
    {
      return hsv2.V.CompareTo(hsv1.V);
    }

    private int CompareHue(AhsvColor hsv1, AhsvColor hsv2)
    {
      var h1 = hsv1.H;// ToHueFamily(hsv1);
      var h2 = hsv2.H;// ToHueFamily(hsv2);
      if (h1>h2)
        return +1;
      if (h1<h2)
        return -1;
      return CompareValue(hsv1, hsv2);
    }

    private int ToHueFamily(AhsvColor hsv)
    {
      return (int)hsv.H.ToHueFamily();
    }

  }

}
