using System.Diagnostics;
using System.Windows.Input;

using Qhta.MVVM;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class WritingSystemViewModel(WritingSystem model)
  : ViewModel<WritingSystem>(model), ILongTextViewModel, IEquatable<WritingSystemViewModel>
{
  public int? Id
  {
    get => Model.Id;
    set
    {
      if (Model.Id != value)
      {
        Model.Id = value;
        NotifyPropertyChanged(nameof(Id));
      }
    }
  }

  public string Name
  {
    get => Model.Name;
    set
    {
      if (Model.Name != value)
      {
        Model.Name = value;
        NotifyPropertyChanged(nameof(Name));
      }
    }
  }

  public string? Type
  {
    get => Model.Type;
    set
    {
      if (Model.Type != value)
      {
        Model.Type = value;
        NotifyPropertyChanged(nameof(Type));
      }
    }
  }

  public int? ParentId
  {
    get => Model.ParentId;
    set
    {
      if (Model.ParentId != value)
      {
        Model.ParentId = value;
        NotifyPropertyChanged(nameof(ParentId));
      }
    }
  }

  public string? Kind
  {
    get => Model.Kind;
    set
    {
      if (Model.Kind != value)
      {
        Model.Kind = value;
        NotifyPropertyChanged(nameof(Kind));
      }
    }
  }

  public string? KeyPhrase
  {
    get => Model.KeyPhrase;
    set
    {
      if (Model.KeyPhrase != value)
      {
        Model.KeyPhrase = value;
        NotifyPropertyChanged(nameof(KeyPhrase));
      }
    }
  }

  public string? Ctg
  {
    get => Model.Ctg;
    set
    {
      if (Model.Ctg != value)
      {
        Model.Ctg = value;
        NotifyPropertyChanged(nameof(Ctg));
      }
    }
  }

  public string? Iso
  {
    get => Model.Iso;
    set
    {
      if (Model.Iso != value)
      {
        Model.Iso = value;
        NotifyPropertyChanged(nameof(Iso));
      }
    }
  }

  public string? Abbr
  {
    get => Model.Abbr;
    set
    {
      if (Model.Abbr != value)
      {
        Model.Abbr = value;
        NotifyPropertyChanged(nameof(Abbr));
      }
    }
  }

  public string? Ext
  {
    get => Model.Ext;
    set
    {
      if (Model.Ext != value)
      {
        Model.Ext = value;
        NotifyPropertyChanged(nameof(Ext));
      }
    }
  }

  public string? Description
  {
    get => Model.Description;
    set
    {
      if (Model.Description != value)
      {
        Model.Description = value;
        NotifyPropertyChanged(nameof(Description));
      }
    }
  }

  public string? Aliases
  {
    get => Model.Aliases;
    set
    {
      if (Model.Aliases != value)
      {
        Model.Aliases = value;
        NotifyPropertyChanged(nameof(Aliases));
      }
    }
  }

  public virtual WritingSystemType? WritingSystemType
  {
    get => Model.WritingSystemType;
    set
    {
      if (Model.WritingSystemType != value)
      {
        Model.WritingSystemType = value;
        NotifyPropertyChanged(nameof(WritingSystemType));
      }
    }
  }

  public virtual WritingSystemKind? WritingSystemKind
  {
    get => Model.WritingSystemKind;
    set
    {
      if (Model.WritingSystemKind != value)
      {
        Model.WritingSystemKind = value;
        NotifyPropertyChanged(nameof(WritingSystemKind));
      }
    }
  }

  public virtual WritingSystem? Parent
  {
    get => Model.Parent;
    set
    {
      if (Model.Parent != value)
      {
        Model.Parent = value;
        NotifyPropertyChanged(nameof(Parent));
      }
    }
  }

  private bool _IsUsed;
  public virtual bool IsUsed
  {
    get => _IsUsed;
    set
    {
      if (_IsUsed != value)
      {
        _IsUsed = value;
        NotifyPropertyChanged(nameof(IsUsed));
      }
    }
  }

  public virtual IEnumerable<WritingSystem>? SelectableWritingSystemParents => _ViewModels.Instance.SelectableWritingSystemParents;

  public virtual WritingSystemsCollection? Children
  {
    get
    {
      if (_Children == null)
      {
        if (Model.Children == null)
          return null;
        _Children = new WritingSystemsCollection(Model.Children.ToList());
      }
      return _Children;
    }
  }

  private WritingSystemsCollection? _Children;

  public int ChildrenCount => Children?.Count ?? 0;

  private bool _isExpanded;
  public bool IsExpanded
  {
    get => _isExpanded;
    set
    {
      if (_isExpanded != value)
      {
        _isExpanded = value;
        NotifyPropertyChanged(nameof(IsExpanded));
      }
    }
  }

  public bool Equals(WritingSystemViewModel? other)
  {
    if (other == null)
      return false;
    return Id == other.Id;
  }

  public override string ToString()
  {
    return Name;
  }

  public string? LongText { get => Description; set => Description = value; }
  public bool CanExpandLongText => !string.IsNullOrEmpty(Model.Description);

  private bool _IsLongTextExpanded = false;
  public bool IsLongTextExpanded
  {
    get => _IsLongTextExpanded;
    set
    {
      if (_IsLongTextExpanded != value)
      {
        _IsLongTextExpanded = value;
        //Debug.WriteLine($"IsWrapped changed to {IsRowHeightExpanded}");
        NotifyPropertyChanged(nameof(IsLongTextExpanded));
      }
    }
  }

}
