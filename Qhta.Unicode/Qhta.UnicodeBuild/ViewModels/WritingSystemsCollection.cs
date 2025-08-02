using System.Diagnostics;

using Qhta.Collections;
using Qhta.ObservableObjects;
using Qhta.UndoManager;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Actions;
using Qhta.UnicodeBuild.Helpers;

using Syncfusion.Data.Extensions;

namespace Qhta.UnicodeBuild.ViewModels;

/// <summary>
/// Specialized collection for managing writing systems.
/// </summary>
public sealed class WritingSystemsCollection : EntityCollection<WritingSystemViewModel>
{

  private BiDiDictionary<int, WritingSystemViewModel> IntDictionary { [DebuggerStepThrough] get; } = new();
  private BiDiDictionary<string, WritingSystemViewModel> StringDictionary { [DebuggerStepThrough] get; } = new();

  /// <summary>
  /// Parent writing system view model, if this collection is a child of another writing system.
  /// </summary>
  public WritingSystemViewModel? Parent { [DebuggerStepThrough] get; }
  
  /// <summary>
  /// Default constructor for the <see cref="WritingSystemsCollection"/> class.
  /// </summary>
  public WritingSystemsCollection() : base((item) => item.Name!)
  {
    CollectionChanged += WritingSystemsCollection_CollectionChanged;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="WritingSystemsCollection"/> class using the specified parent view model.
  /// </summary>
  /// <param name="parent"></param>
  public WritingSystemsCollection(WritingSystemViewModel parent) : this()
  {
    Parent = parent;
    CollectionChanged += WritingSystemsCollection_CollectionChanged;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="WritingSystemsCollection"/> class using the specified parent view model and a collection of writing system models.
  /// </summary>
  /// <param name="parent">Parent view model which contains this collection of children</param>
  /// <param name="models">Children items enumeration</param>
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
      vm.Collection = this; // Set the collection reference for the view model
    }
    CollectionChanged += WritingSystemsCollection_CollectionChanged;
  }

  /// <summary>
  /// Handles changes to the writing systems collection by updating internal dictionaries.
  /// </summary>
  /// <remarks>This method updates the <c>IntDictionary</c> and <c>StringDictionary</c> based on the action
  /// performed on the collection. If items are removed, their identifiers and names are removed from the dictionaries.
  /// If items are added, they are added to the dictionaries, provided their names do not start with "&lt;".</remarks>
  /// <param name="sender">The source of the event, typically the collection that has changed.</param>
  /// <param name="e">The event data containing information about the change.</param>
  private void WritingSystemsCollection_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
    {
      var shouldNotifyTopWritingSystemsChanged = false;
      foreach (WritingSystemViewModel item in e.OldItems!)
      {
        if (item.Id != null)
          IntDictionary.Remove((int)item.Id!);
        if (item.Name != null && !item.Name.StartsWith("<"))
          StringDictionary.Remove(item.Name);
        if (!IsLoaded)
          continue;
        if (item.ParentId == null)
          shouldNotifyTopWritingSystemsChanged = true;
        //else if (item.Parent?.Children != null && item.Parent.Children != this)
        //  item.Parent.Children.Remove(item);
      }
      if (shouldNotifyTopWritingSystemsChanged)
        NotifyPropertyChanged(nameof(TopWritingSystems));
    }
    else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
    {
      var shouldNotifyTopWritingSystemsChanged = false;
      foreach (WritingSystemViewModel item in e.NewItems!)
      {
        if (item.Id != null)
          IntDictionary.TryAdd((int)item.Id, item);
        if (!string.IsNullOrEmpty(item.Name) && !item.Name.StartsWith("<"))
          StringDictionary.TryAdd(item.Name, item);
        if (!IsLoaded)
          continue;
        if (item.ParentId == null)
          shouldNotifyTopWritingSystemsChanged = true;
        //else
        //{
        //  var parent = item.Parent;
        //  var parentChildren = parent?.Children;
        //  if (parentChildren != null && parentChildren != this)
        //  {
        //    parentChildren.Add(item);
        //    parent!.NotifyPropertyChanged(nameof(WritingSystem.Children));
        //  }
        //}
      }
      if (shouldNotifyTopWritingSystemsChanged)
        NotifyPropertyChanged(nameof(TopWritingSystems));
    }
  }

  private void WritingSystemViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (sender is not WritingSystemViewModel vm) return;
    if (e.PropertyName == nameof(WritingSystemViewModel.Id))
    {
      if (vm.Id != null) IntDictionary.TryAdd((int)vm.Id, vm);
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

      if (oldValue != null)
        StringDictionary.Remove(oldValue.ToString()!);
      if (newValue is string newName && !String.IsNullOrEmpty(newName) && !newName.StartsWith('<'))
        StringDictionary.Add(newName, vm);
      // Notify that the collection has changed, so that the UI can update
      //OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
    }
    if (e.PropertyName == nameof(WritingSystemViewModel.ParentId))
    {
      NotifyPropertyChanged(nameof(TopWritingSystems));
    }
  }

  /// <summary>
  /// Finds a writing system by its identifier.
  /// </summary>
  /// <param name="Id"></param>
  /// <returns></returns>
  public WritingSystemViewModel? FindById(int Id)
    => IntDictionary.GetValueOrDefault(Id);

  /// <summary>
  /// Finds a writing system by its name.
  /// </summary>
  /// <param name="name"></param>
  /// <returns></returns>
  public WritingSystemViewModel? FindByName(string name)
  {
    if (name.EndsWith(" Script", StringComparison.InvariantCultureIgnoreCase))
      name = name.Substring(0, name.Length - 7);
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
  /// Adds a writing system to the collection, creating a new view model if it does not already exist.
  /// </summary>
  /// <param name="ws"></param>
  /// <returns></returns>
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

  /// <summary>
  /// Adds a writing system view model to the collection, ensuring it has a valid name and is not already present.
  /// </summary>
  /// <param name="vm"></param>
  public new void Add(WritingSystemViewModel vm)
  {
    //ObservableCollection<WritingSystemViewModel>
    if (String.IsNullOrEmpty(vm.Name))
      return;
    if (vm.Id != null) IntDictionary.TryAdd((int)vm.Id, vm);
    if (!String.IsNullOrEmpty(vm.Name) && !vm.Name.StartsWith("<")) StringDictionary.TryAdd(vm.Name, vm);
    vm.PropertyChanged += WritingSystemViewModel_PropertyChanged;
    base.Add(vm);
  }

  /// <summary>
  /// Removes the first occurrence of a specific object from the collection.
  /// </summary>
  /// <param name="item">The object to remove from the collection. The value can be <see langword="null"/> for reference types.</param>
  /// <returns><see langword="true"/> if the item was successfully removed from the collection;  otherwise, <see
  /// langword="false"/>. This method also returns <see langword="false"/> if the item is not found in the collection.</returns>
  public override bool Remove(WritingSystemViewModel item)
  {
    var count0 = Count;
    var index = IndexOf(item);
    var ok = base.Remove(item);
    if (ok)
    {
      if (Parent == null)
      {
        var dbContext = _ViewModels.Instance.DbContext;
        var entry = dbContext.Entry(item.Model);
        var oldState = entry.State;
        var parent = item.Parent;
        if (parent != null && parent.Children != null && parent.Children != this)
        {
          var childIndex = parent.Children.IndexOf(item);
          parent.Children.Remove(item);
          parent.NotifyPropertyChanged(nameof(WritingSystem.Children));
          UndoMgr.Record(new RemoveEntityAction(), new RemoveEntityArgs(parent.Children, item, childIndex));
        }
        _ViewModels.Instance.DbContext.WritingSystems.Remove(item.Model);
        var newState = entry.State;
        UndoMgr.Record(new RemoveEntityAction(), new RemoveEntityArgs(this, item, index));
        UndoMgr.Record(new DeleteDataEntityAction(), new DeleteDataEntityArgs(item.Model, oldState, newState));
      }
    }
    var count1 = base.Count;
    return ok;
  }

  /// <summary>
  /// Gets the top-level writing systems in the collection, which are those without a parent.
  /// </summary>
  public IEnumerable<WritingSystemViewModel> TopWritingSystems => base.Items.Where(item => item.ParentId == null);

  /// <summary>
  /// Column width for the long text in the UI.
  /// </summary>
  public double LongTextColumnWidth
  {
    [DebuggerStepThrough]
    get => _longTextColumnWidth;
    set
    {
      if (double.IsNaN(value))
        return;
      if (_longTextColumnWidth != value)
      {
        _longTextColumnWidth = value;
        NotifyPropertyChanged(nameof(LongTextColumnWidth));
      }
    }
  }
  private double _longTextColumnWidth = 420; // Default width
}