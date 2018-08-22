using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.DbUtils
{
  /// <summary>
  /// Klasa przechowująca typ i instancję silnika danych. Rozszerza informacje o silniku
  /// </summary>
  public class DbEngineClass: DbEngineInfo, INotifyPropertyChanged
  {
    /// <summary>
    /// Typ silnika danych
    /// </summary>
    public Type Type
    {
      get => _Type;
      set
      {
        if (_Type!=value)
        {
          _Type = value;
          NotifyPropertyChanged(nameof(Type));
        }
      }
    }
    private Type _Type;

    /// <summary>
    /// Instancja utworzonego silnika
    /// </summary>
    public DbEngine Instance
    {
      get => _Instance;
      set
      {
        if (_Instance!=value)
        {
          _Instance = value;
          NotifyPropertyChanged(nameof(Instance));
        }
      }
    }
    private DbEngine _Instance;

    /// <summary>
    /// Zdarzenie zmiany właściwości.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Powiadomienie o zmianie właściwości
    /// </summary>
    /// <param name="propertyName"></param>
    public void NotifyPropertyChanged(string propertyName)
    {
      if (PropertyChanged!=null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Podaje (a wcześniej ewentualnie tworzy) instancję silnika danych
    /// </summary>
    /// <returns></returns>
    public DbEngine GetInstance()
    {
      if (Instance==null)
        Instance = (DbEngine)Type.GetConstructor(new Type[0]).Invoke(new object[0]);
      return Instance;
    }
  }
}
