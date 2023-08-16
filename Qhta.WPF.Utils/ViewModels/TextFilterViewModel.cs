namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// View model shown in TextFilterWindow
/// </summary>
public class TextFilterViewModel: ViewModel
{
  /// <summary>
  /// Specifies if other filters should be cleared.
  /// </summary>
  public bool ClearAllFilters { get; set; }

  /// <summary>
  /// Simple filter text.
  /// </summary>
  public string? FilterText { get; set; }

  /// <summary>
  /// Selected predicate function.
  /// </summary>
  public TextPredicateFunction Function { get; set; }

  /// <summary>
  /// Specifies whether letter case should be ignored.
  /// </summary>
  public bool IgnoreCase { get; set; }

  #region Individual boolea properties used in view.

  /// <summary>
  /// Specifies whether predicate function is IsEqual.
  /// </summary>
  public bool IsEqual { get => Function==TextPredicateFunction.IsEqual; set { if (value) Function = TextPredicateFunction.IsEqual; } }

  /// <summary>
  /// Specifies whether predicate function is NotEqual.
  /// </summary>
  public bool NotEqual { get => Function==TextPredicateFunction.NotEqual; set { if (value) Function = TextPredicateFunction.NotEqual; } }

  /// <summary>
  /// Specifies whether predicate function is StartsWith.
  /// </summary>
  public bool StartsWith { get => Function==TextPredicateFunction.StartsWith; set { if (value) Function = TextPredicateFunction.StartsWith; } }

  /// <summary>
  /// Specifies whether predicate function is EndsWith.
  /// </summary>
  public bool EndsWith { get => Function==TextPredicateFunction.EndsWith; set { if (value) Function = TextPredicateFunction.EndsWith; } }

  /// <summary>
  /// Specifies whether predicate function is Contains.
  /// </summary>
  public bool Contains { get => Function==TextPredicateFunction.Contains; set { if (value) Function = TextPredicateFunction.Contains; } }

  /// <summary>
  /// Specifies whether predicate function is RegExpr.
  /// </summary>
  public bool RegExpr { get => Function==TextPredicateFunction.RegExpr; set { if (value) Function = TextPredicateFunction.RegExpr; } }

  #endregion
}
