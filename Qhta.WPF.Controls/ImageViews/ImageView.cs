using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Qhta.WPF.Controls
{
  /// <summary>
  /// 
  /// </summary>
  [TemplatePart(Name = "PART_Image", Type = typeof(ImageRawView))]
  public class ImageView : System.Windows.Controls.UserControl
  {

    public ImageView()
    {
      DefaultStyleKey = typeof(ImageView);
    }

    ImageRawView PART_Image;
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      PART_Image = Template.FindName("PART_Image", this) as ImageRawView;
      PART_Image.Source = this.Source;
    }

    #region Source
    public static DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(BitmapSource), typeof(ImageView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSourceChanged));

    public BitmapSource Source
    {
      get => (BitmapSource)GetValue(SourceProperty);
      set => SetValue(SourceProperty, value);
    }
    static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as ImageView).OnSourceChanged((BitmapSource)e.NewValue);
    }
    protected virtual void OnSourceChanged(BitmapSource value)
    { 
      if (PART_Image!=null)
        PART_Image.Source = this.Source;
    }

    #endregion

    //#region SourceHeight
    //public static DependencyProperty SourceHeightProperty = DependencyProperty.Register("SourceHeight", typeof(double), typeof(ImageView), new FrameworkPropertyMetadata(16.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    //public double SourceHeight
    //{
    //  get => (double)GetValue(SourceHeightProperty);
    //  set => SetValue(SourceHeightProperty, value);
    //}
    //#endregion

    //#region SourceWidth
    //public static DependencyProperty SourceWidthProperty = DependencyProperty.Register("SourceWidth", typeof(double), typeof(ImageView), new FrameworkPropertyMetadata(16.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    //public double SourceWidth
    //{
    //  get => (double)GetValue(SourceWidthProperty);
    //  set => SetValue(SourceWidthProperty, value);
    //}
    //#endregion

  }
}
