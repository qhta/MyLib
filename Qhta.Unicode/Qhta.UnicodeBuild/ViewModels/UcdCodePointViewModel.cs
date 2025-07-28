using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Qhta.MVVM;
using Qhta.SF.Tools;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;


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
  public string? CharName { get => Model.CharName; set => ChangeProperty(nameof(CharName), value); }

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
  public ICollection<Alias> Aliases { [DebuggerStepThrough] get; } = new List<Alias>();

  /// <summary>
  /// Identifier of the Unicode code point block, which is used to group code points into blocks.
  /// </summary>
  public int? UcdBlockId
  {
    [DebuggerStepThrough]
    get => Model.BlockId;
    set => ChangeModelProperty(nameof(UcdBlockId), value, nameof(UcdBlock));
  }

  /// <summary>
  /// Exposes the Unicode code point block as a view model.
  /// </summary>
  public UcdBlockViewModel? UcdBlock
  {
    get
    {
      var result = Model.BlockId is null ? null : _ViewModels.Instance.UcdBlocks.FindById((int)Model.BlockId);
      return result;
    }
    set => ChangeProperty(nameof(UcdBlockId), value?.Id);
  }

  /// <summary>
  /// Identifier of the area writing system associated with this Unicode code point, if applicable.
  /// </summary>
  public int? AreaId { get => Model.AreaId; set => ChangeModelProperty(nameof(AreaId), value, nameof(Area)); }

  /// <summary>
  /// Exposes the area writing system as a view model.
  /// </summary>
  public WritingSystemViewModel? Area
  {
    get
    {
      var result = Model.AreaId is null ? null : _ViewModels.Instance.WritingSystems.FindById((int)Model.AreaId);
      return result;
    }
    set => ChangeProperty(nameof(AreaId), value?.Id);
  }

  /// <summary>
  /// Identifier of the script writing system associated with this Unicode code point, if applicable.
  /// </summary>
  public int? ScriptId { get => Model.ScriptId; set => ChangeModelProperty(nameof(ScriptId), value, nameof(Script)); }
  /// <summary>
  /// Exposes the script writing system as a view model.
  /// </summary>
  public WritingSystemViewModel? Script
  {
    get
    {
      var result = Model.ScriptId is null ? null : _ViewModels.Instance.WritingSystems.FindById((int)Model.ScriptId);
      return result;
    }
    set => ChangeProperty(nameof(ScriptId), value?.Id); 
  }
  /// <summary>
  /// Identifier of the language writing system associated with this Unicode code point, if applicable.
  /// </summary>
  public int? LanguageId { get => Model.LanguageId; set => ChangeModelProperty(nameof(LanguageId), value, nameof(Language)); }
  /// <summary>
  /// Exposes the language writing system as a view model.
  /// </summary>
  public WritingSystemViewModel? Language
  {
    get
    {
      var result = Model.LanguageId is null ? null : _ViewModels.Instance.WritingSystems.FindById((int)Model.LanguageId);
      return result;
    }
    set => ChangeProperty(nameof(LanguageId), value?.Id);
  }

  /// <summary>
  /// Identifier of the notation writing system associated with this Unicode code point, if applicable.
  /// </summary>
  public int? NotationId { get => Model.NotationId; set => ChangeModelProperty(nameof(NotationId), value, nameof(Notation)); }
  /// <summary>
  /// Exposes the notation writing system as a view model.
  /// </summary>
  public WritingSystemViewModel? Notation
  {
    get
    {
      var result = Model.NotationId is null ? null : _ViewModels.Instance.WritingSystems.FindById((int)Model.NotationId);
      return result;
    }
    set => ChangeProperty(nameof(NotationId), value?.Id);
  }

  /// <summary>
  /// Identifier of the symbol set writing system associated with this Unicode code point, if applicable.
  /// </summary>
  public int? SymbolSetId { get => Model.SymbolSetId; set => ChangeModelProperty(nameof(SymbolSetId), value, nameof(SymbolSet)); }
  /// <summary>
  /// Exposes the symbol set writing system as a view model.
  /// </summary>
  public WritingSystemViewModel? SymbolSet
  {
    get
    {
      var result = Model.SymbolSetId is null ? null : _ViewModels.Instance.WritingSystems.FindById((int)Model.SymbolSetId);
      return result;
    }
    set => ChangeProperty(nameof(SymbolSetId), value?.Id);
  }

  /// <summary>
  /// Identifier of the subset writing system associated with this Unicode code point, if applicable.
  /// </summary>
  public int? SubsetId { get => Model.SubsetId; set => ChangeModelProperty(nameof(SubsetId), value, nameof(Subset)); }
  /// <summary>
  /// Exposes the subset writing system as a view model.
  /// </summary>
  public WritingSystemViewModel? Subset
  {
    get
    {
      var result = Model.SubsetId is null ? null : _ViewModels.Instance.WritingSystems.FindById((int)Model.SubsetId);
      return result;
    }
    set => ChangeProperty(nameof(SubsetId), value?.Id);
  }

  /// <summary>
  /// Exposes the writing system which can be one of all the above writing systems.
  /// Returned value is taken from the following references: <see cref="Subset"/>, <see cref="SymbolSet"/>, <see cref="Notation"/>, <see cref="Language"/>, <see cref="Script"/>, <see cref="Area"/>.
  /// First non-null writing system is returned.
  /// In a set method, the proper reference is set based on <see cref="WritingSystemType"/> of the provided value.
  /// </summary>
  public WritingSystemViewModel? WritingSystem
  {
    get
    {
      var writingSystemId = Model.SubsetId ?? Model.SymbolSetId ?? Model.NotationId ?? Model.LanguageId ?? Model.ScriptId ?? Model.AreaId;
      WritingSystemViewModel? result = null;
      if (writingSystemId != null)
        result = _ViewModels.Instance.WritingSystems.FindById((int)writingSystemId);
      return result;
    }
    set
    {
      if (value is not null)
      {
        switch (value.Type)
        {
          case WritingSystemType.Area:
            Area = value;
            break;
          case WritingSystemType.Script:
            Script = value;
            break;
          case WritingSystemType.Language:
            Language = value;
            break;
          case WritingSystemType.Notation:
            Notation = value;
            break;
          case WritingSystemType.SymbolSet:
            SymbolSet = value;
            break;
          case WritingSystemType.Subset:
            Subset = value;
            break;
          default:
            Debug.WriteLine($"Unsupported WritingSystem type {value.Type} for {value.Name}");
            break;
        }
      }
    }
  }

  /// <summary>
  /// Exposes the collection of writing systems which types are specified by <paramref name="allowedTypes"/>
  /// All non-null writing system is returned in the order of types in <paramref name="allowedTypes"/>.
  /// </summary>
  /// <param name="allowedTypes">Specifies which types of writing system are allowed.</param>
  public IEnumerable<WritingSystemViewModel>? GetWritingSystems(WritingSystemType[] allowedTypes)
  {
    List<WritingSystemViewModel> result = new List<WritingSystemViewModel>(); ;
    WritingSystemViewModel? resultItem = null;
    foreach (var type in allowedTypes)
    {
      switch (type)
      {
        case WritingSystemType.Area:
          resultItem = Area;
          break;
        case WritingSystemType.Script:
          resultItem = Script;
          break;
        case WritingSystemType.Language:
          resultItem = Language;
          break;
        case WritingSystemType.Notation:
          resultItem = Notation;
          break;
        case WritingSystemType.SymbolSet:
          resultItem = SymbolSet;
          break;
        case WritingSystemType.Subset:
          resultItem = Subset;
          break;
      }
      if (resultItem is not null)
      {
        result.Add(resultItem);
      }
    }
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
