using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

using Qhta.MVVM;
using Qhta.SF.WPF.Tools;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.Resources;

#pragma warning disable CA1416
namespace Qhta.UnicodeBuild.ViewModels;

/// <summary>
/// ViewModel for a Unicode code point, providing properties and methods to interact with the underlying model.
/// </summary>
public partial class UcdCodePointViewModel : EntityViewModel<UcdCodePoint>, IRowHeightProvider, IErrorMessageProvider
{
  /// <summary>
  /// Initializes a new instance of the <see cref="UcdCodePointViewModel"/> class with the specified model.
  /// </summary>
  /// <param name="model"></param>
  public UcdCodePointViewModel(UcdCodePoint model) : base(model)
  {
  }

  /// <summary>
  /// Identifier for the code point, typically a unique integer value.
  /// It is a read-only property that retrieves the Id from the underlying model.
  /// </summary>
  public int Id => Model.Id!;

  /// <summary>
  /// Code point value.
  /// It is a read-only property.
  /// </summary>
  public CodePoint CP => Model.Code!;

  /// <summary>
  /// Visual representation of the Unicode code point, which can be a single character or two characters for diacritical marks.
  /// It is a read-only property.
  /// </summary>
  public string? Glyph
  {
    get
    {
      var str = Model.Glyph;
      if (str is null)
        return null;
      if (str.Length == 2)
        return str;
      if (str.Length == 1)
      {
        var ch = str[0];
        if (char.IsSurrogate(ch) || char.IsControl(ch) || char.IsWhiteSpace(ch) ||
            ch == '"' /*|| (ch>='\u007F' && ch <= '\u00A0')*/)
          return null;
        else
          return str;
      }
      return null;
    }
  }

  /// <summary>
  /// Size of the glyph in points. This is used for rendering the glyph in a UI.
  /// it is a read/write property.
  /// </summary>
  public int GlyphSize { get => _GlyphSize; set { if (_GlyphSize != value) { _GlyphSize = value; NotifyPropertyChanged(nameof(GlyphSize)); } } }
  private int _GlyphSize = 12;

  /// <summary>
  /// Short name of the Unicode code point, which may be used to identify the character in various contexts.
  /// It is a read/write property.
  /// </summary>
  public string? CharName
  {
    get => (!string.IsNullOrEmpty(Model.CharName)) ? Model.CharName : null;
    set => ChangeViewModelProperty(nameof(CharName), nameof(Model.CharName), value);
  }

  /// <summary>
  /// Descriptive name of the Unicode code point.
  /// It is a read/write property.
  /// </summary>
  public string? Description => Model.Description;

  /// <summary>
  /// Unicode category of the code point, represented as a two-letter code.
  /// It is a read-only property.
  /// </summary>
  public UcdCategory? Ctg => Enum.Parse<UcdCategory>(Model.Ctg!);

  /// <summary>
  /// Category of the Unicode code point, represented as a view model.
  /// </summary>
  public UnicodeCategoryViewModel? Category
  {
    get
    {
      var result = Model.Ctg is null ? null : _ViewModels.Instance.UnicodeCategoriesList[(int)Convert.ToByte(Ctg)];
      return result;
    }
  }

  /// <summary>
  /// Combining class of the Unicode code point, which indicates how the character combines with other characters.
  /// It is a read-only property that returns a nullable enum value.
  /// </summary>
  public UcdCombination? Comb => Model.Comb;

  /// <summary>
  /// Bidirectional category of the Unicode code point, which indicates how the character behaves in bidirectional text.
  /// It is a read-only property.
  /// </summary>
  public UcdBidir? Bidir => Enum.Parse<UcdBidir>(Model.Bidir!);

  /// <summary>
  /// Decomposition of the Unicode code point, if applicable.
  /// It is a read-only property.
  /// </summary>
  public string? Decomposition => Model.Decomposition;

  /// <summary>
  /// Decimal digit value of the Unicode code point, if applicable.
  /// It is a read-only property that returns a nullable byte value.
  /// </summary>
  public byte? DecDigitVal => (Model.DecDigitVal is null) ? null : byte.Parse(Model.DecDigitVal);

  /// <summary>
  /// Digit value of the Unicode code point, if applicable.
  /// It is a read-only property that returns a nullable byte value.
  /// </summary>
  public byte? DigitVal => (Model.DigitVal is null) ? null : byte.Parse(Model.DigitVal);

  /// <summary>
  /// Number value of the Unicode code point, if applicable.
  /// It is a read-only property that returns a nullable string value.
  /// </summary>
  public string? NumVal => Model.NumVal;

  /// <summary>
  /// Mirror property indicating whether the code point is mirrored in bidirectional text.
  /// It is a read-only property that returns a boolean value.
  /// </summary>
  public bool Mirr => (Model.Mirr == "Y") ? true : false;

  /// <summary>
  /// Old description of the Unicode code point, if applicable.
  /// It is a read-only property that returns a nullable string value.
  /// </summary>
  public string? OldDescription => Model.OldDescription;

  /// <summary>
  /// Comment associated with the Unicode code point, if applicable.
  /// It is a read-only property that returns a nullable string value.
  /// </summary>
  public string? Comment => Model.Comment;

  /// <summary>
  /// Uppercase representation of the Unicode code point, if applicable.
  /// It is a read-only property that returns a nullable CodePoint value.
  /// </summary>
  public CodePoint? Upper => Model.Upper;

  /// <summary>
  /// Lowercase representation of the Unicode code point, if applicable.
  /// It is a read-only property that returns a nullable CodePoint value.
  /// </summary>
  public CodePoint? Lower => Model.Lower;

  /// <summary>
  /// Title of the Unicode code point, if applicable.
  /// It is a read-only property that returns a nullable string value.
  /// </summary>
  public CodePoint? Title => Model.Title;

  /// <summary>
  /// Alias collection for the Unicode code point, which contains alternative names of the code point.
  /// </summary>
  public AliasCollection Aliases { [DebuggerStepThrough] get; } = new ();

  /// <summary>
  /// Identifier of the Unicode code point block, which is used to group code points into blocks.
  /// </summary>
  public int? BlockId
  {
    [DebuggerStepThrough]
    get => Model.BlockId;
    set => ChangeViewModelProperty(nameof(BlockId), nameof(Model.BlockId), value, nameof(Block));
  }

  /// <summary>
  /// Exposes the Unicode code point block as a view model.
  /// </summary>
  public UcdBlockViewModel? Block
  {
    get
    {
      var result = Model.BlockId is null ? null : _ViewModels.Instance.UcdBlocks.FindById((int)Model.BlockId);
      return result;
    }
    set => ChangeViewModelProperty(nameof(BlockId), nameof(Model.BlockId), value?.Id, nameof(Block));
  }


  /// <summary>
  /// Identifier of the script writing system associated with this Unicode code point, if applicable.
  /// </summary>
  public int? WritingSystemId
  {
    get => Model.WritingSystemId; 
    set => ChangeViewModelProperty(nameof(WritingSystemId), nameof(Model.WritingSystemId), value, nameof(WritingSystem));
  }

  /// <summary>
  /// Exposes the script writing system as a view model.
  /// </summary>
  public WritingSystemViewModel? WritingSystem
  {
    get => Model.WritingSystemId is null ? null : _ViewModels.Instance.WritingSystems.FindById((int)Model.WritingSystemId);
    set => ChangeViewModelProperty(nameof(WritingSystemId), nameof(Model.WritingSystemId), value?.Id, nameof(WritingSystem));
  }

  /// <summary>
  /// Identifier of the symbol set writing system associated with this Unicode code point, if applicable.
  /// </summary>
  public int? SymbolSetId
  {
    get => Model.SymbolSetId; 
    set => ChangeViewModelProperty(nameof(SymbolSetId), nameof(Model.SymbolSetId), value, nameof(SymbolSet));
  }
  /// <summary>
  /// Exposes the symbol set writing system as a view model.
  /// </summary>
  public WritingSystemViewModel? SymbolSet
  {
    get => Model.SymbolSetId is null ? null : _ViewModels.Instance.WritingSystems.FindById((int)Model.SymbolSetId);
    set => ChangeViewModelProperty(nameof(SymbolSetId), nameof(Model.SymbolSetId), value?.Id, nameof(SymbolSet));
  }

  /// <summary>
  /// Identifier of the subset writing system associated with this Unicode code point, if applicable.
  /// </summary>
  public int? SubsetId
  {
    get => Model.SubsetId; 
    set => ChangeViewModelProperty(nameof(SubsetId), nameof(Model.SubsetId), value, nameof(Subset));
  }
  /// <summary>
  /// Exposes the subset writing system as a view model.
  /// </summary>
  public WritingSystemViewModel? Subset
  {
    get => Model.SubsetId is null ? null : _ViewModels.Instance.WritingSystems.FindById((int)Model.SubsetId);
    set => ChangeViewModelProperty(nameof(SubsetId), nameof(Model.SubsetId), value?.Id, nameof(Subset));
  }

  /// <summary>
  /// Exposes the collection of all writing systems that are used by this code point.
  /// All non-null writing system properties are returned.
  /// </summary>
  public IEnumerable<WritingSystemViewModel> GetWritingSystems()
  {
    List<WritingSystemViewModel> result = new List<WritingSystemViewModel>(); 
    if (WritingSystem!=null) result.Add(WritingSystem);
    if (SymbolSet!=null) result.Add(SymbolSet);
    if (Subset!=null) result.Add(Subset);
    return result;
  }

  #region IRowHeightProvider implementation
  /// <summary>
  /// Simple property to provide the height of the row in a UI.
  /// </summary>
  public double RowHeight
  {
    [DebuggerStepThrough]
    get => _RowHeight;
    set
    {
      if (_RowHeight != value)
      {
        _RowHeight = value;
        NotifyPropertyChanged(nameof(RowHeight));
      }
    }
  }
  private double _RowHeight = 24;
  #endregion

  #region IErrorMessageProvider implementation

  /// <summary>
  /// Gets or sets an error message associated with the view model.
  /// </summary>
  public string? ErrorMessage { [DebuggerStepThrough] get; set; }

  #endregion
}
