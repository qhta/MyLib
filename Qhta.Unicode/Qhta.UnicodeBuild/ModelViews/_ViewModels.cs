using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
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

  private static _ViewModels? _Instance;
  private static _DbContext _Context = null!;

  private _ViewModels()
  {
    UcdBlocks = new UcdBlocksCollection();
    WritingSystems = new WritingSystemsCollection();
    _Context = new _DbContext();
    {
      foreach (var ub in _Context.UcdBlocks.Include(ub => ub.WritingSystem).ToList())
      {
        UcdBlocks.Add(ub);
      }
      foreach (var ws in _Context.WritingSystems.Include(ws => ws.WritingSystemType).ToList())
      {
        WritingSystems.Add(ws);
      }
      WritingSystemTypes = _Context.WritingSystemTypes.ToList();
      WritingSystemKinds = _Context.WritingSystemKinds.ToList();

    }
  }


  public UcdBlocksCollection UcdBlocks { get; set; }
  public WritingSystemsCollection WritingSystems { get; set; }
  public List<WritingSystemType> WritingSystemTypes { get; set; }
  public List<WritingSystemKind> WritingSystemKinds { get; set; }

  public void Dispose()
  {
    _Context.SaveChanges();
    _Context.Dispose();
  }
}