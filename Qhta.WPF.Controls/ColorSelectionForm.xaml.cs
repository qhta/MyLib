using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Qhta.WPF.Controls
{
  public partial class ColorSelectionForm : UserControl
  {
    public ColorSelectionForm()
    {
      InitializeComponent();
      DefinedColorsPicker.SelectionChanged+=DefinedColorsPicker_SelectionChanged;
      CustomColorForm.SelectedColorChanged+=CustomColorForm_SelectedColorChanged;
      CustomColorForm.CloseFormRequest+=CustomColorForm_CloseFormRequest;
    }

    #region SelectedColor property
    public Color SelectedColor
    {
      get => (Color)GetValue(SelectedColorProperty);
      set => SetValue(SelectedColorProperty, value);
    }

    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register
      ("SelectedColor", typeof(Color), typeof(ColorSelectionForm),
       new FrameworkPropertyMetadata(Colors.Transparent,
         FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    #endregion

    public event ValueChangedEventHandler<Color> SelectedColorChanged;
    public event EventHandler CloseFormRequest;

    private void DefinedColorsPicker_SelectionChanged(object sender, ValueChangedEventArgs<KnownColor> args)
    {
      SelectedColor = args.NewValue.Color;
      SelectedColorChanged?.Invoke(this, new ValueChangedEventArgs<Color>(SelectedColor));
      CloseFormRequest?.Invoke(this, new EventArgs());
    }

    private void CustomColorForm_SelectedColorChanged(object sender, ValueChangedEventArgs<Color> args)
    {
      SelectedColor = args.NewValue;
      if (KnownColors.Instance.FirstOrDefault(item => item.Color.Equals(args.NewValue))==null)
      {
        var knownColors = KnownColors.Instance;
        knownColors.CustomColors.Add(new KnownColor { Name=args.NewValue.ToString(), Color=args.NewValue, IsSelected=true });
      }
      SelectedColorChanged?.Invoke(this, args);
    }

    public class AddressDetails
    {
      public int HouseNo { get; set; }
      public string StreetName { get; set; }
      public string City { get; set; }
      private string PoAddress { get; set; }
    }

    public void SaveCustomColors(string path)
    {
      AddressDetails details = new AddressDetails();
      details.HouseNo = 4;
      details.StreetName = "Rohini";
      details.City = "Delhi";
      //Serialize(details);

      //CustomColors colors = new CustomColors();
      //colors.AddRange(this.CustomColors.Select(item => item.Color));
      XmlSerializer xmlSerializer = new XmlSerializer(typeof(AddressDetails));
      using (TextWriter textWriter = new StreamWriter(@"d:\temp\Xml.xml"))
      {
        //textWriter.Write("ala");
        xmlSerializer.Serialize(textWriter, details);
      }
      //byte[] buffer = new byte[] { 1, 2, 3, 4 };
      //aStream.Write(buffer, 0, 4);
    }

    private void CustomColorForm_CloseFormRequest(object sender, EventArgs e)
    {
      CloseFormRequest?.Invoke(this, new EventArgs());
    }

    
  }
}
