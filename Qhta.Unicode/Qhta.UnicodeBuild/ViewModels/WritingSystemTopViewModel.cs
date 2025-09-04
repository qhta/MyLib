using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.ViewModels;

/// <summary>
/// Represents a top node in a hierarchical tree structure for writing systems.
/// Represents a top-level category of writing systems, such as Area, Family, Script, Language, Notation, SymbolSet, or Subset.
/// </summary>
public class WritingSystemTopViewModel: WritingSystemViewModel
{
  /// <summary>
  /// Initializes a new instance of the <see cref="WritingSystemTopViewModel"/> class with the specified writing system type.
  /// </summary>
  /// <param name="baseCollection">A base collection from which all items of the specific type are retrieved</param>
  /// <param name="type">A type of retrieved items</param>
  public WritingSystemTopViewModel(WritingSystemsCollection baseCollection, WritingSystemType type)
  {
    BaseCollection = baseCollection ?? throw new ArgumentNullException(nameof(baseCollection));
    Type = type;
  }

  /// <summary>
  /// Reference to the base collection of writing systems that this top-level node represents.
  /// </summary>
  public WritingSystemsCollection BaseCollection { get; }
  
  /// <summary>
  /// Specifies the type of writing system this top-level node represents.
  /// </summary>
  public new WritingSystemType Type { get; private set; }

  /// <summary>
  /// New implementation of the Name property that returns the string representation of the Type property.
  /// </summary>
  public new String Name => Resources.WritingSystemTypeStrings.ResourceManager.GetString(Type.ToString()) ?? Type.ToString();

  /// <summary>
  /// Retrieves the child writing systems that belong to this top-level category.
  /// </summary>
  public new IEnumerable<WritingSystemViewModel> Children => BaseCollection.Where(item => item.Type == Type);
}
