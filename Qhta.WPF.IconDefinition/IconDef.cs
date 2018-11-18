using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Qhta.Drawing;
using Size = System.Windows.Size;
using DrawingContext = Qhta.Drawing.DrawingContext;
using System.Windows.Data;
using Qhta.WPF.Utils;

namespace Qhta.WPF.IconDefinition
{
  public class IconDef : FrameworkElement
  {

    //#region Resources property
    //public ResourceDictionary Resources
    //{
    //  get => (ResourceDictionary)GetValue(ResourcesProperty);
    //  set => SetValue(ResourcesProperty, value);
    //}

    //public static DependencyProperty ResourcesProperty = DependencyProperty.Register
    //  ("Resources", typeof(ResourceDictionary), typeof(IconDef),
    //   new PropertyMetadata(new ResourceDictionary()));
    //#endregion

    #region Drawing property
    public Drawing Drawing
    {
      get => (Drawing)GetValue(DrawingProperty);
      set => SetValue(DrawingProperty, value);
    }

    public static DependencyProperty DrawingProperty = DependencyProperty.Register
      ("Drawing", typeof(Drawing), typeof(IconDef),
        new FrameworkPropertyMetadata(null, 
          FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
          DrawingPropertyChanged)
      );
    
    private static void DrawingPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as IconDef).DrawingChanged();
    }

    private void DrawingChanged()
    {
      if (Drawing!=null)
      {
        Drawing.Invalidated+=Drawing_Invalidated;
      }
    }

    private void Drawing_Invalidated(object sender, EventArgs e)
    {
      this.InvalidateVisual();
      Invalidated?.Invoke(this, new EventArgs());
    }

    public event EventHandler Invalidated;

    #endregion

    #region Source property
    public string Source
    {
      get => (string)GetValue(SourceProperty);
      set => SetValue(SourceProperty, value);
    }

    public static DependencyProperty SourceProperty = DependencyProperty.Register
      ("Source", typeof(string), typeof(IconDef),
       new PropertyMetadata(null, SourcePropertyChanged));

    private static void SourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as IconDef).SourceChanged();
    }

    private void SourceChanged()
    {
      Load(Source);
    }
    #endregion



    public void Load(string source)
    {
      if (!File.Exists(source))
      {
        var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        source = Path.Combine(path, source);
      }
      if (File.Exists(source))
        LoadFromFile(source);
    }

    public void LoadFromFile(string filename)
    {
      using (var inputStream = File.OpenRead(filename))
      {
        var obj = XamlReader.Load(inputStream);
        if (obj is Drawing drawing)
        {
          this.Drawing=drawing;
        }
        else if ((obj is IconDef iconDef))
        {
          this.Resources = iconDef.Resources;
          this.Drawing = iconDef.Drawing;
        }
      }
    }

    #region Images property / invalidate
    public Dictionary<int, Bitmap> Images => _Images;
    private Dictionary<int, Bitmap> _Images = new Dictionary<int, Bitmap>();

    public void Invalidate()
    {
      Images.Clear();
      foreach (DictionaryEntry entry in Resources)
      {
        var obj = entry.Value;
        if (obj is Parameter parameter)
          obj = parameter.Value;
        if (obj is System.Windows.Media.Color color)
        { 
          Debug.WriteLine($"{entry.Key}={color}");
        }
        if (obj is SolidColorBrush solidBrush)
        {
          var binding = BindingOperations.GetBindingExpressionBase(solidBrush, SolidColorBrush.ColorProperty);
          if (binding!=null)
          {
            //Debug.WriteLine(binding.GetString());
            binding.UpdateTarget();
          }
          //(solidBrush as DependencyObject).CoerceValue(SolidColorBrush.ColorProperty);
          //(solidBrush as DependencyObject).InvalidateProperty(SolidColorBrush.ColorProperty);
          var color1 = solidBrush.Color;
          //Debug.WriteLine($"{entry.Key}.Color={color1}");
        }
      }
      if (Drawing!=null)
        Drawing.InvalidateBindings();
    }
    #endregion

    #region producing BitmapImage
    public BitmapSource GetBitmapSource(int size)
    {
      //Debug.WriteLine($"GetBitmapSource");
      if (Drawing==null)
        return null;

      var bitmap = GetImage(size);
      WriteableBitmap result = new WriteableBitmap(size, size, 96, 96, PixelFormats.Bgra32, null);
      var pixelArray = bitmap.GetPixelArray();
      result.SetPixelArray(pixelArray);
      //if (size<=32)
      //  for (int y = 0; y<size; y++)
      //  {
      //    for (int x = 0; x<size; x++)
      //      Debug.Write($"{(uint)pixelArray[x, y]:X8} ");
      //    Debug.WriteLine("");
      //  }
      return result;
    }

    public Bitmap GetImage(int size)
    {
      if (Drawing==null)
        return null;

      if (Images.TryGetValue(size, out var image))
        return image;
      var drawing = Drawing;
      var bitmap = new Bitmap(size, size);
      Graphics bitmapGraphics = Graphics.FromImage(bitmap);
      DrawingContext context = new DrawingContext { Graphics=bitmapGraphics, ScaleX=size/drawing.Width, ScaleY=size/drawing.Height };
      drawing.Draw(context);
      _Images.Add(size, bitmap);
      return bitmap;
    }
    #endregion
  }
}
