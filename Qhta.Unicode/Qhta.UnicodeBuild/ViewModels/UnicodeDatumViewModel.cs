using System;
using System.Collections.Generic;

using Microsoft.DotNet.DesignTools.ViewModels;

using Qhta.MVVM;
using Qhta.Unicode.Models;

namespace Qhta.Unicode.ViewModels;

public partial class UnicodeDatumViewModel : ViewModel<UnicodeDatum>
{
  /// <inheritdoc />
  public UnicodeDatumViewModel(UnicodeDatum model) : base(model)
  {
    // Initialize any additional properties or collections here if needed
  }

  public int Ord { get => Model.Ord; set { if (Model.Ord!=value) { Model.Ord = value; NotifyPropertyChanged(nameof(Ord)); } } }
  public string? Glyph { get => Model.Glyph; set { if (Model.Glyph!=value) { Model.Glyph = value; NotifyPropertyChanged(nameof(Glyph)); } } }
  public string Code { get => Model.Code; set { if (Model.Code!=value) { Model.Code = value; NotifyPropertyChanged(nameof(Code)); } } }
  public string? CharName { get => Model.CharName; set { if (Model.CharName!=value) { Model.CharName = value; NotifyPropertyChanged(nameof(CharName)); } } }
  public string? Description { get => Model.Description; set { if (Model.Description!=value) { Model.Description = value; NotifyPropertyChanged(nameof(Description)); } } }
  public string? Ctg { get => Model.Ctg; set { if (Model.Ctg!=value) { Model.Ctg = value; NotifyPropertyChanged(nameof(Ctg)); } } }
  public double? Comb { get => Model.Comb; set { if (Model.Comb!=value) { Model.Comb = value; NotifyPropertyChanged(nameof(Comb)); } } }
  public string? Bidir { get => Model.Bidir; set { if (Model.Bidir!=value) { Model.Bidir = value; NotifyPropertyChanged(nameof(Bidir)); } } }
  public string? Decomposition { get => Model.Decomposition; set { if (Model.Decomposition!=value) { Model.Decomposition = value; NotifyPropertyChanged(nameof(Decomposition)); } } }
  public string? DecDigitVal { get => Model.DecDigitVal; set { if (Model.DecDigitVal!=value) { Model.DecDigitVal = value; NotifyPropertyChanged(nameof(DecDigitVal)); } } }
  public string? DigitVal { get => Model.DigitVal; set { if (Model.DigitVal!=value) { Model.DigitVal = value; NotifyPropertyChanged(nameof(DigitVal)); } } }
  public string? NumVal { get => Model.NumVal; set { if (Model.NumVal!=value) { Model.NumVal = value; NotifyPropertyChanged(nameof(NumVal)); } } }
  public string? Mirr { get => Model.Mirr; set { if (Model.Mirr!=value) { Model.Mirr = value; NotifyPropertyChanged(nameof(Mirr)); } } }
  public string? OldDescription { get => Model.OldDescription; set { if (Model.OldDescription!=value) { Model.OldDescription = value; NotifyPropertyChanged(nameof(OldDescription)); } } }
  public string? Comment { get => Model.Comment; set { if (Model.Comment!=value) { Model.Comment = value; NotifyPropertyChanged(nameof(Comment)); } } }
  public string? Upper { get => Model.Upper; set { if (Model.Upper!=value) { Model.Upper = value; NotifyPropertyChanged(nameof(Upper)); } } }
  public string? Lower { get => Model.Lower; set { if (Model.Lower!=value) { Model.Lower = value; NotifyPropertyChanged(nameof(Lower)); } } }
  public string? Title { get => Model.Title; set { if (Model.Title!=value) { Model.Title = value; NotifyPropertyChanged(nameof(Title)); } } }
  public ICollection<Alias> Aliases { get; set; } = new List<Alias>();
}
