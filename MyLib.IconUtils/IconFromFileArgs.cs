using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.IconUtils
{

  /// <summary>
  /// Argumenty dla funkcji <c>GetIconFromFile</c>
  /// </summary>
  [DataContract]
  public class IconFromFileArgs
  {
    public IconFromFileArgs()
    {
      BWTreshold = 0.5;
      Gamma = 1.0;
    }

    /// <summary>
    /// Żądane wielkości ikon
    /// </summary>
    [DataMember]
    public int[] Sizes { get; set; }

    /// <summary>
    /// Czy brakujące wielkości ikon mają być utworzone.
    /// Istotne tylko przy określeniu <see cref="Sizes"/>
    /// </summary>
    [DataMember]
    public bool CreateMissingSizes { get; set; }

    /// <summary>
    /// Żądane rozdzielczości barwne (bits per pixel)
    /// </summary>
    [DataMember]
    public ColorDepth[] Depths { get; set; }

    /// <summary>
    /// Czy brakujące rozdzielczości barwne mają być utworzone.
    /// Istotne tylko przy określeniu <see cref="Depths"/>
    /// </summary>
    [DataMember]
    public bool CreateMissingDepths { get; set; }

    /// <summary>
    /// Próg rozgraniczenia jasności przy konwersji obrazu kolorowego na czarno-biały
    /// Istotne tylko przy ustawieniu <see cref="CreateMissingColorDepth"/>
    /// </summary>
    [DataMember]
    [DefaultValue(0.5)]
    public double BWTreshold { get; set; }

    /// <summary>
    /// Współczynnik korekcji cieni i rozjaśnień przy konwersji obrazu kolorowego na odcienie szarości i czarno-biały
    /// Istotne tylko przy ustawieniu <see cref="CreateMissingColorDepth"/>
    /// </summary>
    [DataMember]
    [DefaultValue(1.0)]
    public double Gamma { get; set; }  
  }

  /// <summary>
  /// Argumenty dla funkcji <c>GetIconFromFont</c>
  /// </summary>
  public class IconFromFontArgs
  {

    /// <summary>
    /// Wypełnienie czcionki
    /// </summary>
    public Brush FontBrush { get; set; }

    /// <summary>
    /// Funkcja skalująca rozmiar czcionki względem rozmiaru ikony.
    /// Domyślne skalowanie zapewnia, aby napis zmieścił się w obszarze ikony
    /// </summary>
    public Func<float, float> FontSizeScaling { get; set; }

  }


}
