using System.ComponentModel.DataAnnotations;

using Qhta.MVVM;
using Qhta.SF.Tools;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;
using Qhta.UndoManager;
using Qhta.UnicodeBuild.Actions;

#pragma warning disable CA1416
namespace Qhta.UnicodeBuild.ViewModels;

/// <summary>
/// ViewModel for a Unicode code point, providing properties and methods to interact with the underlying model.
/// </summary>
public partial class UcdCodePointViewModel : ViewModel<UcdCodePoint>, IRowHeightProvider, IErrorMessageProvider
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
  public string? CharName { get => Model.CharName; set { if (Model.CharName != value) { Model.CharName = value; NotifyPropertyChanged(nameof(CharName)); } } }

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
  public UcdCombination? Comb =>  Model.Comb;

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
  public ICollection<Alias> Aliases { get; } = new List<Alias>();

  /// <summary>
  /// Identifier of the Unicode code point block, which is used to group code points into blocks.
  /// </summary>
  public int? UcdBlockId
  {
    get => Model.Block; 
    set 
    {
      if (Model.Block != value)
      {
        UndoMgr.Record(new ChangePropertyAction(), new ChangePropertyArgs(this, nameof(UcdBlockId), UcdBlockId, value));
        Model.Block = value; 
        NotifyPropertyChanged(nameof(UcdBlockId));
        NotifyPropertyChanged(nameof(UcdBlock));
      }
    }
  }
  /// <summary>
  /// Exposes the Unicode code point block as a view model.
  /// </summary>
  public UcdBlockViewModel? UcdBlock
  {
    get
    {
      var result = Model.Block is null ? null : _ViewModels.Instance.UcdBlocks.FindById((int)Model.Block);
      return result;
    }
    set
    {
      if (value is not null)
      {
        if (value.Id != UcdBlockId)
        {
          UcdBlockId = value?.Id;
          NotifyPropertyChanged(nameof(UcdBlock));
        }
      }
      else
      {
        if (UcdBlockId is not null)
        {
          UcdBlockId = null;
          NotifyPropertyChanged(nameof(UcdBlock));
        }
      }
    }
  }

  /// <summary>
  /// Identifier of the area writing system associated with this Unicode code point, if applicable.
  /// </summary>
  public int? AreaId { get => Model.Area; set { if (Model.Area != value) { Model.Area = value; NotifyPropertyChanged(nameof(AreaId)); } } }
  /// <summary>
  /// Exposes the area writing system as a view model.
  /// </summary>
  public WritingSystemViewModel? Area
  {
    get
    {
      var result = Model.Area is null ? null : _ViewModels.Instance.WritingSystems.FindById((int)Model.Area);
      return result;
    }
    set
    {
      if (value is not null)
      {
        if (value.Id != AreaId)
        {
          AreaId = value?.Id;
          NotifyPropertyChanged(nameof(Area));
        }
      }
      else
      {
        if (AreaId is not null)
        {
          AreaId = null;
          NotifyPropertyChanged(nameof(Area));
        }
      }
    }
  }

  /// <summary>
  /// Identifier of the script writing system associated with this Unicode code point, if applicable.
  /// </summary>
  public int? ScriptId { get => Model.Script; set { if (Model.Script != value) { Model.Script = value; NotifyPropertyChanged(nameof(ScriptId)); } } }
  /// <summary>
  /// Exposes the script writing system as a view model.
  /// </summary>
  public WritingSystemViewModel? Script
  {
    get
    {
      var result = Model.Script is null ? null : _ViewModels.Instance.WritingSystems.FindById((int)Model.Script);
      return result;
    }
    set
    {
      if (value is not null)
      {
        if (value.Id != ScriptId)
        {
          ScriptId = value?.Id;
          NotifyPropertyChanged(nameof(Script));
        }
      }
      else
      {
        if (ScriptId is not null)
        {
          ScriptId = null;
          NotifyPropertyChanged(nameof(Script));
        }
      }
    }
  }
  /// <summary>
  /// Identifier of the language writing system associated with this Unicode code point, if applicable.
  /// </summary>
  public int? LanguageId { get => Model.Language; set { if (Model.Language != value) { Model.Language = value; NotifyPropertyChanged(nameof(LanguageId)); } } }
  /// <summary>
  /// Exposes the language writing system as a view model.
  /// </summary>
  public WritingSystemViewModel? Language
  {
    get
    {
      var result = Model.Language is null ? null : _ViewModels.Instance.WritingSystems.FindById((int)Model.Language);
      return result;
    }
    set
    {
      if (value is not null)
      {
        if (value.Id != LanguageId)
        {
          LanguageId = value?.Id;
          NotifyPropertyChanged(nameof(Language));
        }
      }
      else
      {
        if (LanguageId is not null)
        {
          LanguageId = null;
          NotifyPropertyChanged(nameof(Language));
        }
      }
    }
  }

  /// <summary>
  /// Identifier of the notation writing system associated with this Unicode code point, if applicable.
  /// </summary>
  public int? NotationId { get => Model.Notation; set { if (Model.Notation != value) { Model.Notation = value; NotifyPropertyChanged(nameof(NotationId)); } } }
  /// <summary>
  /// Exposes the notation writing system as a view model.
  /// </summary>
  public WritingSystemViewModel? Notation
  {
    get
    {
      var result = Model.Notation is null ? null : _ViewModels.Instance.WritingSystems.FindById((int)Model.Notation);
      return result;
    }
    set
    {
      if (value is not null)
      {
        if (value.Id != NotationId)
        {
          NotationId = value?.Id;
          NotifyPropertyChanged(nameof(Notation));
        }
      }
      else
      {
        if (NotationId is not null)
        {
          NotationId = null;
          NotifyPropertyChanged(nameof(Notation));
        }
      }
    }
  }

  /// <summary>
  /// Identifier of the symbol set writing system associated with this Unicode code point, if applicable.
  /// </summary>
  public int? SymbolSetId { get => Model.SymbolSet; set { if (Model.SymbolSet != value) { Model.SymbolSet = value; NotifyPropertyChanged(nameof(SymbolSetId)); } } }
  /// <summary>
  /// Exposes the symbol set writing system as a view model.
  /// </summary>
  public WritingSystemViewModel? SymbolSet
  {
    get
    {
      var result = Model.SymbolSet is null ? null : _ViewModels.Instance.WritingSystems.FindById((int)Model.SymbolSet);
      return result;
    }
    set
    {
      if (value is not null)
      {
        if (value.Id != SymbolSetId)
        {
          SymbolSetId = value?.Id;
          NotifyPropertyChanged(nameof(SymbolSet));
        }
      }
      else
      {
        if (SymbolSetId is not null)
        {
          SymbolSetId = null;
          NotifyPropertyChanged(nameof(SymbolSet));
        }
      }
    }
  }

  /// <summary>
  /// Identifier of the subset writing system associated with this Unicode code point, if applicable.
  /// </summary>
  public int? SubsetId { get => Model.Subset; set { if (Model.Subset != value) { Model.Subset = value; NotifyPropertyChanged(nameof(SubsetId)); } } }
  /// <summary>
  /// Exposes the subset writing system as a view model.
  /// </summary>
  public WritingSystemViewModel? Subset
  {
    get
    {
      var result = Model.Subset is null ? null : _ViewModels.Instance.WritingSystems.FindById((int)Model.Subset);
      return result;
    }
    set
    {
      if (value is not null)
      {
        if (value.Id != SubsetId)
        {
          SubsetId = value?.Id;
          NotifyPropertyChanged(nameof(Subset));
        }
      }
      else
      {
        if (SubsetId is not null)
        {
          SubsetId = null;
          NotifyPropertyChanged(nameof(Subset));
        }
      }
    }
  }

  /// <summary>
  /// Identifier of the artefact writing system associated with this Unicode code point, if applicable.
  /// </summary>
  public int? ArtefactId { get => Model.Artefact; set { if (Model.Artefact != value) { Model.Artefact = value; NotifyPropertyChanged(nameof(ArtefactId)); } } }
  /// <summary>
  /// Exposes the artefact writing system as a view model.
  /// </summary>
  public WritingSystemViewModel? Artefact
  {
    get
    {
      var result = Model.Artefact is null ? null : _ViewModels.Instance.WritingSystems.FindById((int)Model.Artefact);
      return result;
    }
    set
    {
      if (value is not null)
      {
        if (value.Id != ArtefactId)
        {
          ArtefactId = value?.Id;
          NotifyPropertyChanged(nameof(Artefact));
        }
      }
      else
      {
        if (ArtefactId is not null)
        {
          ArtefactId = null;
          NotifyPropertyChanged(nameof(Artefact));
        }
      }
    }
  }


  #region IRowHeightProvider implementation
  /// <summary>
  /// Simple property to provide the height of the row in a UI.
  /// </summary>
  public double RowHeight
  {
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
  public string ErrorMessage { get; set; }

  #endregion
}
