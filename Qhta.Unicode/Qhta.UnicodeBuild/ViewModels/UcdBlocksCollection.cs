using Qhta.ObservableObjects;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;

namespace Qhta.UnicodeBuild.ViewModels;

/// <summary>
/// Specialized collection for UCD blocks, providing methods to find blocks by ID or name.
/// </summary>
public sealed class UcdBlocksCollection() : OrderedObservableCollection<UcdBlockViewModel>((item) => item.Range!.Start!)
{

  private Dictionary<int, UcdBlockViewModel> IntDictionary { get; set; } = new();
  private Dictionary<string, UcdBlockViewModel> StringDictionary { get; set; } = new();

  /// <summary>
  /// Initializes a new instance of the <see cref="UcdBlocksCollection"/> class built from an existing collection of UcdBlock models.
  /// </summary>
  /// <param name="models"></param>
  public UcdBlocksCollection(IEnumerable<UcdBlock> models) : this()
  {
    foreach (var model in models)
    {
      var vm = _ViewModels.Instance.UcdBlocks.FirstOrDefault(item => item.Id == model.Id);
      if (vm == null)
      {
        vm = new UcdBlockViewModel(model);
      }
      Add(vm);
    }
    CollectionChanged += UcdBlocksCollection_CollectionChanged;
  }

  private void UcdBlocksCollection_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
    {
      foreach (WritingSystemViewModel vm in e.OldItems!)
      {
        if (vm.Id != null)
          IntDictionary.Remove((int)vm.Id!);
        if (vm.Name != null && !vm.Name.StartsWith("<"))
          StringDictionary.Remove(vm.Name);
      }
    }
    else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
    {
      foreach (UcdBlockViewModel vm in e.NewItems!)
      {
        IntDictionary.TryAdd((int)vm.Id!, vm);
        if (!string.IsNullOrEmpty(vm.Name))
        {
          StringDictionary.TryAdd(vm.Name, vm);
        }
      }
    }
  }

  private void WritingSystemViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (sender is not UcdBlockViewModel vm) return;
    if (e.PropertyName == nameof(UcdBlockViewModel.Id))
    {
      if (vm.Id != null) IntDictionary.Add((int)vm.Id, vm);
      //OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
    }
    if (e.PropertyName == nameof(UcdBlockViewModel.Name))
    {
      object? oldValue = null;
      object? newValue = null;
      if (e is PropertyChanged2EventArgs e2)
      {
        oldValue = e2.OldValue;
        newValue = e2.NewValue;
      }
      else
        newValue = vm.Name;

      if (oldValue != null)
        StringDictionary.Remove(oldValue.ToString()!);
      if (newValue is string newName && !String.IsNullOrEmpty(newName) && !newName.StartsWith('<')) StringDictionary.Add(newName, vm);
      // Notify that the collection has changed, so that the UI can update
      //OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
    }
  }

  /// <summary>
  /// Finds a UcdBlockViewModel by its ID.
  /// </summary>
  /// <param name="Id"></param>
  /// <returns></returns>
  public UcdBlockViewModel? FindById(int Id)
    => IntDictionary.GetValueOrDefault(Id);

  /// <summary>
  /// Finds a UcdBlockViewModel by its name.
  /// </summary>
  /// <param name="name"></param>
  /// <returns></returns>
  public UcdBlockViewModel? FindByName(string name)
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
  /// Adds a new UcdBlockViewModel on the existing UcdBlock entity to the collection if it does not already exist.
  /// </summary>
  /// <param name="ws"></param>
  /// <returns></returns>
  public UcdBlockViewModel Add(UcdBlock ws)
  {
    var vm = (!IsLoaded) ? null : _ViewModels.Instance.UcdBlocks.FindById((int)ws.Id!);
    if (vm == null)
    {
      vm = new UcdBlockViewModel(ws);
      Add(vm);
    }
    return vm;
  }

  /// <summary>
  /// Adds a new UcdBlockViewModel to the collection, ensuring it has a valid name.
  /// </summary>
  /// <param name="vm"></param>
  public new void Add(UcdBlockViewModel vm)
  {
    if (String.IsNullOrEmpty(vm.Name))
      return;
    vm.PropertyChanged += WritingSystemViewModel_PropertyChanged;
    base.Add(vm);
    if (vm.Id != null) IntDictionary.Add((int)vm.Id, vm);
    if (!String.IsNullOrEmpty(vm.Name)) StringDictionary.Add(vm.Name, vm);
  }

}
