using System.ComponentModel;
using System.Windows;

using Qhta.MVVM;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;
using Qhta.DeepCopy;
using Qhta.ObservableObjects;

namespace Qhta.UnicodeBuild.ViewModels;

public sealed class WritingSystemsCollection() : OrderedObservableCollection<WritingSystemViewModel>((item) => item.Name!)
{
  

  private Dictionary<int, WritingSystemViewModel> IntDictionary { get; set; } = new();
  private Dictionary<string, WritingSystemViewModel> StringDictionary { get; set; } = new();

  public WritingSystemsCollection(WritingSystemViewModel parent, IEnumerable<WritingSystem> models) : this()
  {
    Parent = parent;
    foreach (var model in models)
    {
      var vm = _ViewModels.Instance.WritingSystems.FirstOrDefault(item => item.Id == model.Id);
      if (vm == null)
      {
        vm = new WritingSystemViewModel(model);
      }
      Add(vm);
    }
    CollectionChanged += WritingSystemsCollection_CollectionChanged;
  }

  private void WritingSystemsCollection_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
    {
      foreach (WritingSystemViewModel vm in e.OldItems!)
      {
        if (vm.Id!=null)
          IntDictionary.Remove((int)vm.Id!);
        if (vm.Name != null && !vm.Name.StartsWith("<"))
          StringDictionary.Remove(vm.Name);
      }
    }
    else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
    {
      foreach (WritingSystemViewModel vm in e.NewItems!)
      {
        IntDictionary.TryAdd((int)vm.Id!, vm);
        if (!string.IsNullOrEmpty(vm.Name) && !vm.Name.StartsWith("<"))
        {
          StringDictionary.TryAdd(vm.Name, vm);
        }
      }
    }
  }

  private void WritingSystemViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (sender is not WritingSystemViewModel vm) return;
    if (e.PropertyName == nameof(WritingSystemViewModel.Id))
    {
      if (vm.Id != null) IntDictionary.Add((int)vm.Id, vm);
      //OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
    }
    if (e.PropertyName == nameof(WritingSystemViewModel.Name))
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

      if (oldValue!=null)
        StringDictionary.Remove(oldValue.ToString()!);
      if (newValue is string newName && !String.IsNullOrEmpty(newName) && !newName.StartsWith('<')) StringDictionary.Add(newName, vm);
      // Notify that the collection has changed, so that the UI can update
      //OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
    }
    if (e.PropertyName == nameof(WritingSystemViewModel.ParentId))
    {
      OnPropertyChanged(new PropertyChangedEventArgs(nameof(TopWritingSystems)));
    }
  }


  public WritingSystemViewModel? FindById(int Id)
    => IntDictionary.GetValueOrDefault(Id);

  public WritingSystemViewModel? FindByName(string name)
  {
    if (name.EndsWith(" Script", StringComparison.InvariantCultureIgnoreCase))
      name = name.Substring(0, name.Length - 7);
    var result = StringDictionary.GetValueOrDefault(name);
    if (result == null)
    {
      if (name.Contains(' '))
        result = StringDictionary.GetValueOrDefault(name.Replace(' ','-'));
      else if (name.Contains('-'))
        result = StringDictionary.GetValueOrDefault(name.Replace('-', ' '));
    }
    return result;
  }



  public WritingSystemViewModel? Parent { get; }

  public WritingSystemViewModel Add(WritingSystem ws)
  {
    var vm = (!IsLoaded) ? null : _ViewModels.Instance.WritingSystems.FindById((int)ws.Id!);
    if (vm == null)
    {
      vm = new WritingSystemViewModel(ws);
      Add(vm);
    }
    return vm;
  }

  public new void Add(WritingSystemViewModel vm)
  {
    if (String.IsNullOrEmpty(vm.Name))
      return;
    vm.PropertyChanged += WritingSystemViewModel_PropertyChanged;
    base.Add(vm);
    if (vm.Id != null) IntDictionary.Add((int)vm.Id, vm);
    if (!String.IsNullOrEmpty(vm.Name) && !vm.Name.StartsWith("<")) StringDictionary.Add(vm.Name, vm);
  }

  //public RelayCommand EvaluateIsUsedCommand { get; }

  //private void EvaluateIsUsed()
  //{
  //  foreach (var vm in this)
  //  {
  //    vm.IsUsed = vm.Model.UcdBlocks?.Count > 0 || vm.Model.Children?.Count > 0 || vm.Model.UcdRanges?.Count > 0;
  //  }
  //}

  public IEnumerable<WritingSystemViewModel> TopWritingSystems => Items.Where(item => item.ParentId == null);


}