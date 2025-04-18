using System;
using System.Collections.Generic;
using Qhta.MVVM;
using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class AliasViewModel(Alias model) : ViewModel<Alias>(model)
{
  // Initialize any additional properties or collections here if needed

  public int Ord { get => Model.Ord; set { if (Model.Ord != value) { Model.Ord = value; NotifyPropertyChanged(nameof(Ord)); } } }
  public string Alias1 { get => Model.Alias1; set { if (Model.Alias1 != value) { Model.Alias1 = value; NotifyPropertyChanged(nameof(Alias1)); } } }
  public AliasType? Type { get => ConvertByteToAliasType(Model.Type); 
    set { if (ConvertByteToAliasType(Model.Type) != value) { Model.Type = ConvertAliasTypeToByte(value); NotifyPropertyChanged(nameof(Type)); } } }

  public static AliasType? ConvertByteToAliasType(byte? value)
  {
    if (value.HasValue && Enum.IsDefined(typeof(AliasType), value.Value))
    {
      return (AliasType)value.Value;
    }
    return null; // Return null if the value is not valid
  }

  public static byte? ConvertAliasTypeToByte(AliasType? aliasType)
  {
    if (aliasType.HasValue)
    {
      return (byte)aliasType.Value; // Cast the enum to a byte
    }
    return null; // Return null if the input is null
  }

}

