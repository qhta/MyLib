using Microsoft.VisualBasic;
using Qhta.Unicode.Models;

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
      foreach (var wst in _Context.WritingSystemTypes.ToList())
      {
        WritingSystemTypesList.Add(new WritingSystemTypeViewModel(wst));
      }

      foreach (var wsk in _Context.WritingSystemKinds.ToList())
      {
        WritingSystemKindsList.Add(new WritingSystemKindViewModel(wsk));
      }


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
  public List<WritingSystemTypeViewModel> WritingSystemTypesList { get; } = new();
  public List<WritingSystemKindViewModel> WritingSystemKindsList { get; } = new();
  public Array WritingSystemTypes { get; } = Enum.GetValues(typeof(WritingSystemType));
  public Array WritingSystemKinds { get; } = Enum.GetValues(typeof(WritingSystemKind));
  public Array Categories { get; } = Enum.GetNames(typeof(UcdCategory));

  public UcdCodePointsCollection UcdCodePoints { get; set; } = new ();

  public IEnumerable<WritingSystemViewModel> SelectableWritingSystems
  {
    get
    {
      var list = _ViewModels.Instance.AllWritingSystems.Where(item=> !item.Name.StartsWith("<")) // items with special names are not selectable
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
    var lastItem = _Context?.WritingSystems.OrderByDescending(ws => ws.Id).FirstOrDefault();
    if (lastItem == null) return 1; // If no writing systems exist, start with ID 1.
    var maxId = lastItem.Id ?? 0;
    if (lastItem.Name=="" || lastItem.Name.StartsWith("<")) 
    {
      // If the last item has no name, or it has a special name we can use its ID directly.
      return maxId;
    }
    return maxId+1;
  }
}