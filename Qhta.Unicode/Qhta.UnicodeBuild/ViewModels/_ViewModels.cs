using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.ViewModels;

namespace Qhta.UnicodeBuild.ViewModels;

public class _ViewModels: IDisposable
{
  public static _ViewModels Instance
  {
    get
    {
      if (_Instance == null)
      {
        _Instance = new _ViewModels();
      }
      return _Instance;
    }
  }

  public _DbContext DBContext
  {
    get
    {
      if (_Context == null)
      {
        _Context = new _DbContext();
      }
      return _Context;
    }
  }

  private static _ViewModels? _Instance;
  private static _DbContext? _Context = null!;


  private _ViewModels()
  {
    _Context = new _DbContext();
    {
      foreach (var ur in _Context.UcdRanges
                 //.Include(ub => ub.WritingSystem)
                 //.Include(ub => ub.UcdRanges)
                 .ToList())
      {
        UcdRanges.Add(ur);
        //Debug.WriteLine($"{ub.BlockName}.UcdRanges = {ub.UcdRanges.Count}");
      }

      foreach (var ub in _Context.UcdBlocks
                 .ToList())
      {
        UcdBlocks.Add(ub);
        //Debug.WriteLine($"{ub.BlockName}.UcdRanges = {ub.UcdRanges.Count}");
      }

      foreach (var ws in _Context.WritingSystems/*.Include(ws => ws.WritingSystemType)*/.OrderBy(ws=>ws.Name).ToList())
      {
        var vm= new WritingSystemViewModel(ws);
        AllWritingSystems.Add(vm);
        WritingSystemViewModels.Add((int)ws.Id!, vm);
        if (ws.ParentId==null)
        {
          TopWritingSystems.Add(vm);
        }
      }
      AllWritingSystems.CollectionChanged += AllWritingSystems_CollectionChanged;

      foreach (var cp in _Context.CodePoints)
      {
        var vm = new UcdCodePointViewModel(cp);
        UcdCodePoints.Add(vm);
      }
      WritingSystemTypes = _Context.WritingSystemTypes.ToList();
      WritingSystemKinds = _Context.WritingSystemKinds.ToList();

    }
  }

  private void AllWritingSystems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
    {
      foreach (WritingSystemViewModel vm in e.OldItems!)
      {
        WritingSystemViewModels.Remove((int)vm.Id!);
      }
    }
    else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
    {
      foreach (WritingSystemViewModel vm in e.NewItems!)
      {
        WritingSystemViewModels.Add((int)vm.Id!, vm);
      }
    }
  }

  public UcdBlocksCollection UcdBlocks { get; set; } = new ();
  public UcdRangeCollection UcdRanges { get; set; } = new ();
  public Dictionary<int, WritingSystemViewModel> WritingSystemViewModels { get; set; } = new();
  public WritingSystemsCollection AllWritingSystems { get; set; } = new ();
  public WritingSystemsCollection TopWritingSystems { get; set; } = new ();
  public List<WritingSystemType> WritingSystemTypes { get; set; }
  public List<WritingSystemKind> WritingSystemKinds { get; set; }
  public UcdCodePointsCollection UcdCodePoints { get; set; } = new ();

  public IEnumerable<WritingSystemViewModel> SelectableWritingSystems
  {
    get
    {
      var list = _ViewModels.Instance.AllWritingSystems
        .OrderBy(vm => vm.FullName).ToList();
      list.Insert(0, dummyWritingSystemViewModel);
      return list;
    }
  }

  readonly WritingSystemViewModel dummyWritingSystemViewModel = new WritingSystemViewModel(new WritingSystem { Name = "" });
  public void Dispose()
  {
    if (_Context!=null)
    {
      _Context.SaveChanges();
      _Context.Dispose();
      _Context = null;
    }
  }

  public int GetNewWritingSystemId()
  {
    var maxId = _Context?.WritingSystems.Max(ws => ws.Id) ?? 0;
    return maxId + 1;
  }
}