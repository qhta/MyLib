namespace Qhta.WPF.Utils;

/// <summary>
/// ShaderEffect class to adjust colors of the brush.
/// </summary>
public class ColorAdjustEffect : ShaderEffect
{
  private static PixelShader _pixelShader = new PixelShader() { UriSource = new Uri("pack://application:,,,/" + Assembly.GetExecutingAssembly() + ";component/Effects/ColorAdjust.ps") };

  /// <summary>Dependency property to store Input property.</summary>
  public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(ColorAdjustEffect), 0);
  /// <summary>Dependency property to store Saturation property.</summary>
  public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register("Saturation", typeof(double), typeof(ColorAdjustEffect), new UIPropertyMetadata(1.0, PixelShaderConstantCallback(0), CoerceFactor));
  /// <summary>Dependency property to store Gamma property.</summary>
  public static readonly DependencyProperty GammaProperty = DependencyProperty.Register("Gamma", typeof(double), typeof(ColorAdjustEffect), new UIPropertyMetadata(0.5, PixelShaderConstantCallback(1), CoerceFactor));
  /// <summary>Dependency property to store BrightnessAdjustment property.</summary>
  public static readonly DependencyProperty BrightnessAdjustmentProperty = DependencyProperty.Register("BrightnessAdjustment", typeof(double), typeof(ColorAdjustEffect), new UIPropertyMetadata(0.0, PixelShaderConstantCallback(2), CoerceBrightnessAdjustment));
  /// <summary>Dependency property to store RedAdjustment property.</summary>
  public static readonly DependencyProperty RedAdjustmentProperty = DependencyProperty.Register("RedAdjustment", typeof(double), typeof(ColorAdjustEffect), new UIPropertyMetadata(0.0, PixelShaderConstantCallback(3), CoerceBrightnessAdjustment));
  /// <summary>Dependency property to store GreenAdjustment property.</summary>
  public static readonly DependencyProperty GreenAdjustmentProperty = DependencyProperty.Register("GreenAdjustment", typeof(double), typeof(ColorAdjustEffect), new UIPropertyMetadata(0.0, PixelShaderConstantCallback(4), CoerceBrightnessAdjustment));
  /// <summary>Dependency property to store BlueAdjustment property.</summary>
  public static readonly DependencyProperty BlueAdjustmentProperty = DependencyProperty.Register("BlueAdjustment", typeof(double), typeof(ColorAdjustEffect), new UIPropertyMetadata(0.0, PixelShaderConstantCallback(5), CoerceBrightnessAdjustment));

  /// <summary>
  /// Default constructor that updates shader values.
  /// </summary>
  public ColorAdjustEffect()
  {
    PixelShader = _pixelShader;

    UpdateShaderValue(InputProperty);
    UpdateShaderValue(SaturationProperty);
    UpdateShaderValue(GammaProperty);
    UpdateShaderValue(BrightnessAdjustmentProperty);
    UpdateShaderValue(RedAdjustmentProperty);
    UpdateShaderValue(GreenAdjustmentProperty);
    UpdateShaderValue(BlueAdjustmentProperty);
  }

  /// <summary>
  /// Input brush property.
  /// </summary>
  public Brush Input
  {
    get { return (Brush)GetValue(InputProperty); }
    set { SetValue(InputProperty, value); }
  }

  /// <summary>
  /// A value between 0 and 1 to alter the amount of colour left in the image. 0 is entirely greyscale, and 1 is unaffected. Default is 1.
  /// </summary>
  public double Saturation
  {
    get { return (double)GetValue(SaturationProperty); }
    set { SetValue(SaturationProperty, value); }
  }

  /// <summary>
  /// A value between 0 and 1 to alter the lightness of the greyscale without altering true black or true white. 
  /// 0 shifts shades closer to true black, and 1 shifts shades closer to true white. Default is 0.5.
  /// </summary>
  public double Gamma
  {
    get { return (double)GetValue(GammaProperty); }
    set { SetValue(GammaProperty, value); }
  }

  /// <summary>
  /// A value between -1 and 1 to linearly move the end result closer to true black or true white respectively.
  /// -1 will result in an entirely black image, +1 will result in an entirely white image. Default is 0.
  /// </summary>
  public double BrightnessAdjustment
  {
    get { return (double)GetValue(BrightnessAdjustmentProperty); }
    set { SetValue(BrightnessAdjustmentProperty, value); }
  }

  /// <summary>
  /// A value between -1 and 1 to linearly increase the Red component of the result.
  /// -1 will remove all Red from the image, +1 will maximize all Red in the image. Default is 0.
  /// </summary>
  public double RedAdjustment
  {
    get { return (double)GetValue(RedAdjustmentProperty); }
    set { SetValue(RedAdjustmentProperty, value); }
  }
  /// <summary>
  /// A value between -1 and 1 to linearly increase the Green component of the result.
  /// -1 will remove all Green from the image, +1 will maximize all Green in the image. Default is 0.
  /// </summary>
  public double GreenAdjustment
  {
    get { return (double)GetValue(GreenAdjustmentProperty); }
    set { SetValue(GreenAdjustmentProperty, value); }
  }
  /// <summary>
  /// A value between -1 and 1 to linearly increase the Blue component of the result.
  /// -1 will remove all Blue from the image, +1 will maximize all Blue in the image. Default is 0.
  /// </summary>
  public double BlueAdjustment
  {
    get { return (double)GetValue(BlueAdjustmentProperty); }
    set { SetValue(BlueAdjustmentProperty, value); }
  }

  /// <summary>
  /// Coerces factor to the range of 0.0 to 1.0.
  /// </summary>
  /// <param name="d"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  private static object CoerceFactor(DependencyObject d, object value)
  {
    double newFactor = (double)value;

    if (newFactor < 0.0) return 0.0;
    if (newFactor > 1.0) return 1.0;
    return newFactor;
  }

  private static object CoerceBrightnessAdjustment(DependencyObject d, object value)
  {
    double newFactor = (double)value;

    if (newFactor < -1.0) return -1.0;
    if (newFactor > 1.0) return 1.0;
    return newFactor;
  }
}
