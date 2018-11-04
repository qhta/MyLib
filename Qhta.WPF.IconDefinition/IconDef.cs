using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Qhta.WPF
{
  public class IconDef:/* DependencyObject,*/ INotifyPropertyChanged
  {

    Qhta.Drawing.IconDef _DrawingIcon;

    public void Load(string source)
    {
      if (!File.Exists(source))
      {
        var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        source = Path.Combine(path, source);
        Debug.WriteLine(source);
      }
      if (File.Exists(source))
        LoadFromFile(source);
    }

    public void LoadFromFile(string filename)
    {
      using (var inputStream = File.OpenRead(filename))
      {
        var obj = XamlReader.Load(inputStream);
        if (obj is Qhta.Drawing.Drawing drawing)
        {
          _DrawingIcon = new Drawing.IconDef { Drawing=drawing };
          Drawing = new Qhta.Drawing.WPF.DrawingToWpfConverter().Convert(drawing);
        }
      }
    }

    #region Source property
    public string Source
    {
      get => _Source;
      set
      {
        if (_Source!=value)
        {
          _Source=value;
          SourceChanged();
          NotifyPropertyChanged(nameof(Source));
        }
      }
    }
    private string _Source;

    //{
    //  get => (string)GetValue(SourceProperty);
    //  set => SetValue(SourceProperty, value);
    //}

    //public DependencyProperty SourceProperty = DependencyProperty.Register
    //  ("Source", typeof(string), typeof(IconDef),
    //    new PropertyMetadata(null,SourcePropertyChanged));

    //private static void SourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    //{
    //  (sender as IconDef).SourceChanged();
    //}

    private void SourceChanged()
    {
      if (Source!=null)
        Load(Source);
    }
    #endregion

    #region Drawing property
    public DrawingGroup Drawing
    {
      get => _Drawing;
      set
      {
        if (_Drawing!=value)
        {
          _Drawing=value;
          NotifyPropertyChanged(nameof(Drawing));
        }
      }
    }
    private DrawingGroup _Drawing;
    #endregion

    #region INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(string propertyName)
    {
      if (PropertyChanged!=null)
        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
  }
}
