using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.DotNet.DesignTools.Protocol.Values;
using Microsoft.DotNet.DesignTools.ViewModels;
using PropertyTools.DataAnnotations;
using Qhta.MVVM;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;

using BrowsableAttribute = System.ComponentModel.BrowsableAttribute;
using ReadOnlyAttribute = System.ComponentModel.ReadOnlyAttribute;
namespace Qhta.UnicodeBuild.ViewModels;

public partial class UcdCodePointViewModel : ViewModel<UcdCodePoint>, IRowHeightProvider
{
  /// <inheritdoc />
  public UcdCodePointViewModel(UcdCodePoint model) : base(model)
  {
    // Initialize any additional properties or collections here if needed
  }
  [Browsable(false)]
  public int Id { get => Model.Id!; set { if (Model.Id != value) { Model.Id = value!; NotifyPropertyChanged(nameof(Id)); } } }

  //[RegularExpression("[0-9a-fA-F]+")]
  //[Range(typeof(int),"0","0xFFFFFF")]
  //[ReadOnly(true)]
  public CodePoint CP { get => Model.Code!; set { if (Model.Code !=value) { Model.Code = value!; NotifyPropertyChanged(nameof(CP)); } } }
  public string? Glyph { get => Model.Glyph; set { if (Model.Glyph!=value) { Model.Glyph = value; NotifyPropertyChanged(nameof(Glyph)); } } }
  public int GlyphSize { get => _GlyphSize; set { if (_GlyphSize != value) { _GlyphSize = value; NotifyPropertyChanged(nameof(GlyphSize)); } } }
  private int _GlyphSize = 12;
  public string? CharName { get => Model.CharName; set { if (Model.CharName!=value) { Model.CharName = value; NotifyPropertyChanged(nameof(CharName)); } } }
  public string? Description { get => Model.Description; set { if (Model.Description!=value) { Model.Description = value; NotifyPropertyChanged(nameof(Description)); } } }

  public UcdCategory? Ctg
  {
    get => Enum.Parse<UcdCategory>(Model.Ctg!);
    set
    {
      var val = value?.ToString();
      if (Model.Ctg != val)
      {
        Model.Ctg = val;
        NotifyPropertyChanged(nameof(Ctg));
      }
    }
  }

  public UnicodeCategoryViewModel? Category
  {
    get
    {
      var result = Model.Ctg is null ? null : _ViewModels.Instance.UnicodeCategoriesList[(int)Convert.ToByte(Ctg)];
      return result;
    }
    set
    {
      if (value is not null)
      {
        if (value.Id != Ctg)
        {
          Ctg = value?.Id;
          NotifyPropertyChanged(nameof(Category));
        }
      }
      else
      {
        if (Ctg is not null)
        {
          Ctg = null;
          NotifyPropertyChanged(nameof(Category));
        }
      }
    }
  }

  public UcdCombination? Comb
  {
    get => (Model.Comb is null) ? null : (UcdCombination)Enum.ToObject(typeof(UcdCombination), Model.Comb);
    set
    {
      byte? val = (value is null) ? null: Convert.ToByte(value);
      if (Model.Comb != val)
      {
        Model.Comb = val;
        NotifyPropertyChanged(nameof(Comb));
      }
    }
  }

  public UcdBidir? Bidir
  {
    get => Enum.Parse<UcdBidir>(Model.Bidir!);
    set
    {
      var val = value?.ToString();
      if (Model.Bidir != val)
      {
        Model.Bidir = val;
        NotifyPropertyChanged(nameof(Bidir));
      }
    }
  }
  public string? Decomposition { get => Model.Decomposition; set { if (Model.Decomposition!=value) { Model.Decomposition = value; NotifyPropertyChanged(nameof(Decomposition)); } } }

  [Range(typeof(byte), "0", "9")]
  public byte? DecDigitVal
  {
    get => (Model.DecDigitVal is null) ? null : byte.Parse(Model.DecDigitVal);
    set
    {
      var val = value?.ToString();
      if (Model.DecDigitVal != val)
      {
        Model.DecDigitVal = val;
        NotifyPropertyChanged(nameof(DecDigitVal));
      }
    }
  }

  [Range(typeof(byte), "0", "9")]
  public byte? DigitVal
  {
    get => (Model.DigitVal is null) ? null : byte.Parse(Model.DigitVal);
    set
    {
      var val = value?.ToString();
      if (Model.DigitVal != val)
      {
        Model.DigitVal = val;
        NotifyPropertyChanged(nameof(DigitVal));
      }
    }
  }

  [RegularExpression("[0-9]+(/[0-9]+)")]
  public string? NumVal { get => Model.NumVal; set { if (Model.NumVal!=value) { Model.NumVal = value; NotifyPropertyChanged(nameof(NumVal)); } } }

  public bool? Mirr
  {
    get => (Model.Mirr is null) ? null : (Model.Mirr == "Y") ? true : (Model.Mirr=="N") ? false  :null;
    set
    {
      var val = (value is null) ? null : (value==true) ? "Y" : "N";
      if (Model.Mirr != val)
      {
        Model.Mirr = val;
        NotifyPropertyChanged(nameof(Mirr));
      }
    }
  }
  
  public string? OldDescription { get => Model.OldDescription; set { if (Model.OldDescription!=value) { Model.OldDescription = value; NotifyPropertyChanged(nameof(OldDescription)); } } }
  
  public string? Comment { get => Model.Comment; set { if (Model.Comment!=value) { Model.Comment = value; NotifyPropertyChanged(nameof(Comment)); } } }
  [RegularExpression("^[0-9a-fA-F]{4,6}")]
  public string? Upper { get => Model.Upper; set { if (Model.Upper!=value) { Model.Upper = value; NotifyPropertyChanged(nameof(Upper)); } } }
  [RegularExpression("^[0-9a-fA-F]{4,6}")]
  public string? Lower { get => Model.Lower; set { if (Model.Lower!=value) { Model.Lower = value; NotifyPropertyChanged(nameof(Lower)); } } }
  [RegularExpression("^[0-9a-fA-F]{4,6}")]
  public string? Title { get => Model.Title; set { if (Model.Title!=value) { Model.Title = value; NotifyPropertyChanged(nameof(Title)); } } }
  //public ICollection<Alias> Aliases { get; set; } = new List<Alias>();

  public int? UcdBlockId { get => Model.Script; set { if (Model.Block != value) { Model.Block = value; NotifyPropertyChanged(nameof(UcdBlockId)); } } }
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

  public int? AreaId { get => Model.Area; set { if (Model.Area != value) { Model.Area = value; NotifyPropertyChanged(nameof(AreaId)); } } }
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

  public int? ScriptId { get => Model.Script; set { if (Model.Script != value) { Model.Script = value; NotifyPropertyChanged(nameof(ScriptId)); } } }
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

  public int? LanguageId { get => Model.Language; set { if (Model.Language != value) { Model.Language = value; NotifyPropertyChanged(nameof(LanguageId)); } } }
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

  public int? NotationId { get => Model.Notation; set { if (Model.Notation != value) { Model.Notation = value; NotifyPropertyChanged(nameof(NotationId)); } } }
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

  public int? SymbolSetId { get => Model.SymbolSet; set { if (Model.SymbolSet != value) { Model.SymbolSet = value; NotifyPropertyChanged(nameof(SymbolSetId)); } } }
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

  public int? SubsetId { get => Model.Subset; set { if (Model.Subset != value) { Model.Subset = value; NotifyPropertyChanged(nameof(SubsetId)); } } }
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

  public int? ArtefactId { get => Model.Artefact; set { if (Model.Artefact != value) { Model.Artefact = value; NotifyPropertyChanged(nameof(ArtefactId)); } } }
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

  private double _RowHeight = 24;

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
}
