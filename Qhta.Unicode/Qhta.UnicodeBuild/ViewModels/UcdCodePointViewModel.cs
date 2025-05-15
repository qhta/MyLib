using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.DotNet.DesignTools.Protocol.Values;
using Microsoft.DotNet.DesignTools.ViewModels;
using PropertyTools.DataAnnotations;
using Qhta.MVVM;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;

using BrowsableAttribute = System.ComponentModel.BrowsableAttribute;
using ReadOnlyAttribute = System.ComponentModel.ReadOnlyAttribute;
namespace Qhta.UnicodeBuild.ViewModels;

public partial class UcdCodePointViewModel : ViewModel<UcdCodePoint>
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
}
