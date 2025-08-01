using System.Diagnostics;
using Qhta.MVVM;
using Qhta.UnicodeBuild.ViewModels;

namespace Qhta.UnicodeBuild.Commands;


/// <summary>
/// Command to mark unused writing systems.
/// All writing systems that:
/// <list type="bullet">
/// <item>are not used in any code point</item>
/// <item>are not used in any Unicode block</item>
/// <item>have no children</item>
/// </list>
///  will be marked as unused.
/// </summary>
public class MarkUnusedWritingSystemsCommand : Command
{
  /// <inheritdoc/>
  public override bool CanExecute(object? parameter)
  {
    var result = _ViewModels.Instance.UcdCodePoints.IsLoaded;
    //Debug.WriteLine($"MarkUnusedWritingSystemsCommand.CanExecute({parameter})={result}");
    return result;
  }

  /// <inheritdoc/>
  public override void Execute(object? parameter)
  {
    foreach (var writingSystem in _ViewModels.Instance.WritingSystems)
    {
      var isUnused = true;
      // Check if the writing system has children
      if (writingSystem.Children?.Any() == true)
        isUnused = false;
      else
        // Check if the writing system is not used in any Unicode block
      if (_ViewModels.Instance.UcdBlocks.Any(block => block.WritingSystem == writingSystem)) isUnused = false;
      else
      // Check if the writing system is not used in any code point
      if (_ViewModels.Instance.UcdCodePoints.Any(cp => cp.GetWritingSystems().Contains(writingSystem)))
        isUnused = false;
      writingSystem.IsMarked = isUnused;
    }
  }
}