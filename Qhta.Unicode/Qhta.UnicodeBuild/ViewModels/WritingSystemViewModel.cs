﻿using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Qhta.MVVM;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;


namespace Qhta.UnicodeBuild.ViewModels;

public class WritingSystemViewModel(WritingSystem model)
  : ViewModel<WritingSystem>(model), ILongTextViewModel, IEquatable<WritingSystemViewModel>, IComparable<WritingSystemViewModel>, INotifyDataErrorInfo
{
  public WritingSystemViewModel() : this(new WritingSystem())
  {
    //Debug.WriteLine($"WritingSystemViewModel() {this}");
  }

  [Browsable(false)]
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


  [Required]
  public string? Name
  {
    get => Model.Name;
    set
    {
      if (Model.Name != value)
      {
        var oldValue = Model.Name;
        Model.Name = value;
        NotifyPropertyChanged(nameof(Name), oldValue, value);
      }
    }
  }

  [Browsable(false)]
  public string FullName => Model.Name + " " + Type.ToString()?.ToLower();


  [Required]
  public WritingSystemType? Type
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

  public WritingSystemKind? Kind
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

  [DataType(DataType.MultilineText)]
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

  [Browsable(false)]
  public virtual WritingSystemViewModel? Parent
  {
    get => _ViewModels.Instance.WritingSystems.FirstOrDefault(vm => vm.Id == Model.ParentId);
    set
    {
      var parentId = value?.Id;
      if (parentId == 0)
        parentId = null;

      if (Parent != null)
      {
        Parent.Children?.Remove(this);
      }
      Model.ParentId = parentId;
      NotifyPropertyChanged(nameof(Parent));
      if (Parent?.Children != null)
      {
        Parent.Children?.Add(this);
        Parent.NotifyPropertyChanged(nameof(Children));
      }
      NotifyPropertyChanged(nameof(ParentId));
    }
  }

  [Browsable(false)]
  public virtual bool IsUsed => Model.UcdBlocks?.Count > 0 || Model.Children?.Count > 0;

  [Browsable(false)]
  public virtual WritingSystemsCollection? Children
  {
    get
    {
      if (_Children == null)
      {
        if (Model.Children == null)
          return null;
        _Children = new WritingSystemsCollection(this, Model.Children.OrderBy(ws => ws.Name).ToList());
        _Children.CollectionChanged += Children_CollectionChanged;
      }
      return _Children;
    }
    //set => _Children = value;
  }


  private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
  {
    //Debug.WriteLine($"Children_CollectionChanged({sender}, {e.Action}, {e.NewItems?.Cast<WritingSystemViewModel>().FirstOrDefault()}, {e.NewStartingIndex})");
  }

  private WritingSystemsCollection? _Children;
  [Browsable(false)]
  public int ChildrenCount => Children?.Count ?? 0;

  private bool _isExpanded;
  [Browsable(false)]
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

  public static bool LogEquals;
  public bool Equals(WritingSystemViewModel? other)
  {
    if (LogEquals)
      Debug.WriteLine($"Compare with other {other}");
    if (other == null)
      return false;
    if (this.Name==null || other.Name == null)
    {
      if (LogEquals)
        Debug.WriteLine($"WritingSystemViewModel.Equals: Name is empty for {this} or {other}");
      return true;
    }
    return Id == other.Id;
  }

  //public override bool Equals(object? other)
  //{
  //  if (LogEquals)
  //    Debug.WriteLine($"Compare {this} with other object {other}");
  //  if (other is string str)
  //  {
  //    if (str==null)
  //    {
  //      Debug.WriteLine($"WritingSystemViewModel.Equals: Name is empty for {other}");
  //      return true;
  //    }
  //  }
  //  else
  //  if (other is WritingSystemViewModel otherVM)
  //    return Equals(otherVM);
  //  return false;
  //}

  [Browsable(false)] public string? LongText { get => Description; set => Description = value; }
  [Browsable(false)] public bool CanExpandLongText => !string.IsNullOrEmpty(Model.Description);

  private bool _IsLongTextExpanded;
  [Browsable(false)]
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

  private readonly Dictionary<string, ValidationResultEx> errors = new();

  public bool Validate(string propertyName)
  {
    bool ok = true;
    if (propertyName == nameof(Name))
    {
      errors.Remove(propertyName);
      if (string.IsNullOrEmpty(Name))
        errors.Add(propertyName, new ValidationResultEx("Name cannot be empty", Severity.Error));
      else
      if (!Regex.IsMatch(Name, "^[a-zA-Z]+( [a-zA-Z0-9]+)*$"))
        errors.Add(propertyName, new ValidationResultEx("Invalid name string", Severity.Error));
      ok = false;
    }
    else
    {
      errors.Remove(propertyName);
      ok = true;
    }

    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    return ok;
  }

  IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName)
  {
    if (propertyName != null && errors.TryGetValue(propertyName, out var error))
    {
      yield return error;
    }
    yield return null;
  }

  public bool HasErrors => errors.Count > 0;

  public void AddError(string propertyName, string message, Severity severity = Severity.Error)
  {
    errors.Remove(propertyName);
    errors.Add(propertyName, new ValidationResultEx(message, severity));
    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

  }
  public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

  public int CompareTo(WritingSystemViewModel? other)
  {
    if (other is null) return 1;
    if (Id==null && other.Id == null)
      return 0;
    if (Id == null)
      return -1;
    if (other.Id == null)
      return 1;
    return  ((int)Id).CompareTo((int)(other.Id));
  }

  public override String ToString()
  {
    if (Name == null)
      return "<null>";
    if (Name == "")
      return "<empty>";
    return Name + " " + Type?.ToString()?.ToLower();
  }

}
