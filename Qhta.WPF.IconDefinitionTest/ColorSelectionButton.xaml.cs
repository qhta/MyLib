using System.Windows.Controls;
using System.Windows.Media;
//using Qhta.Drawing;

namespace Qhta.WPF.IconDefinitionTest
{
  /// <summary>
  /// Interaction logic for ColorSelectionDrawingTest.xaml
  /// </summary>
  public partial class ColorSelectionButton : UserControl
  {
    public ColorSelectionButton()
    {
      InitializeComponent();
    }

    private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      IconDef icon = this.FindResource("ColorIndicatorIconDef") as IconDef;
      var primaryColor = icon.Resources["PrimaryColor"] as Parameter;
      primaryColor.Value = Colors.Red;
      icon.Invalidate();
      var source = this.Image.Source;
      this.Image.InvalidateProperty(Image.SourceProperty);
      var binding = this.Image.GetBindingExpression(Image.SourceProperty);
      binding.UpdateTarget();
    }
  }
}
