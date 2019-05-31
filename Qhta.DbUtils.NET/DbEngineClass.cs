using System;
using System.ComponentModel;

namespace Qhta.DbUtils
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
      get
      {
        if (_Instance==null)
          _Instance = CreateInstance();
        return _Instance;
      }
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
    /// Tworzy instancję silnika danych
    /// </summary>
    /// <returns></returns>
    public DbEngine CreateInstance()
    {
      var instance = (DbEngine)Type.GetConstructor(new Type[0]).Invoke(new object[0]);
      return instance;
    }

  }
}
