using System.Diagnostics;
using Qhta.ObservableObjects;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;

namespace Qhta.UnicodeBuild.ViewModels;

/// <summary>
/// Specialized collection for UCD blocks, providing methods to find blocks by ID or name.
/// </summary>
public sealed class AliasCollection : EntityCollection<AliasViewModel>
{

  private Dictionary<int, AliasViewModel> IntDictionary { [DebuggerStepThrough] get; set; } = new();
  private Dictionary<string, AliasViewModel> StringDictionary { [DebuggerStepThrough] get; set; } = new();

  /// <summary>
  /// Default constructor for the <see cref="AliasCollection"/> class, initializing an empty collection.
  /// </summary>
  public AliasCollection() : base(((item) => item.Id))
  {
    _IsReadOnly = true; // Make the collection read-only
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="AliasCollection"/> class built from an existing collection of UcdBlock models.
  /// </summary>
  /// <param name="models"></param>
  public AliasCollection(IEnumerable<Alias> models) : this()
  {
    foreach (var model in models)
    {
      var vm = _ViewModels.Instance.Aliases.FirstOrDefault(item => item.Id == model.Id);
      if (vm == null)
      {
        vm = new AliasViewModel(model);
      }
      Add(vm);
    }
    CollectionChanged += AliasCollection_CollectionChanged;
  }

  private void AliasCollection_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
    {
      foreach (WritingSystemViewModel vm in e.OldItems!)
      {
        if (vm.Id != null)
          IntDictionary.Remove((int)vm.Id!);
        if (vm.Name != null)
          StringDictionary.Remove(vm.Name);
      }
    }
    else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
    {
      foreach (AliasViewModel vm in e.NewItems!)
      {
        IntDictionary.TryAdd((int)vm.Id!, vm);
        if (!string.IsNullOrEmpty(vm.Name))
        {
          StringDictionary.TryAdd(vm.Name, vm);
        }
      }
    }
  }


  /// <summary>
  /// Finds a AliasViewModel by its ID.
  /// </summary>
  /// <param name="Id"></param>
  /// <returns></returns>
  public AliasViewModel? FindById(int Id)
    => IntDictionary.GetValueOrDefault(Id);

  /// <summary>
  /// Finds a AliasViewModel by its name.
  /// </summary>
  /// <param name="name"></param>
  /// <returns></returns>
  public AliasViewModel? FindByName(string name)
  {
    var result = StringDictionary.GetValueOrDefault(name);
    if (result == null)
    {
      if (name.Contains(' '))
        result = StringDictionary.GetValueOrDefault(name.Replace(' ', '-'));
      else if (name.Contains('-'))
        result = StringDictionary.GetValueOrDefault(name.Replace('-', ' '));
    }
    return result;
  }

  /// <summary>
  /// Adds a new AliasViewModel on the existing UcdBlock entity to the collection if it does not already exist.
  /// </summary>
  /// <param name="alias"></param>
  /// <returns></returns>
  public AliasViewModel Add(Alias alias)
  {
    var vm = (!IsLoaded) ? null : _ViewModels.Instance.Aliases.FindById((int)alias.Id!);
    if (vm == null)
    {
      vm = new AliasViewModel(alias);
      Add(vm);
    }
    return vm;
  }

  /// <summary>
  /// Adds a new AliasViewModel to the collection.
  /// </summary>
  /// <param name="vm"></param>
  public new void Add(AliasViewModel vm)
  {
    if (String.IsNullOrEmpty(vm.Name))
      return;
    base.Add(vm);
    IntDictionary.Add((int)vm.Id, vm);
    if (!String.IsNullOrEmpty(vm.Name)) StringDictionary.Add(vm.Name, vm);
  }

}
