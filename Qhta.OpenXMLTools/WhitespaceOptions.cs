using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Mode for handling whitespaces.
/// </summary>
public enum WsMode
{
  /// <summary>
  /// Do not change the white spaces.
  /// </summary>
  NoChange,
  /// <summary>
  /// Remove white spaces.
  /// </summary>
  Remove,
  /// <summary>
  /// Remove white spaces and replace with a single space.
  /// </summary>
  Reduce,
  /// <summary>
  /// Change white spaces to tabs.
  /// </summary>
  ChangeToTabs,
  /// <summary>
  /// Change tabs to whitespaces.
  /// </summary>
  ChangeToSpaces
}

/// <summary>
/// Options for handling whitespaces in text.
/// </summary>
public record WhitespaceOptions
{
  /// <summary>
  /// Default constructor.
  /// </summary>
  public WhitespaceOptions()
  {
    Mode = WsMode.NoChange;
    Start = WsMode.NoChange;
    End = WsMode.NoChange;
  }

  /// <summary>
  /// Constructor with all options.
  /// </summary>
  /// <param name="mode"></param>
  /// <param name="start"></param>
  /// <param name="end"></param>
  public WhitespaceOptions(WsMode mode, WsMode start, WsMode end)
  {
    Mode = mode;
    Start = start;
    End = end;
  }

  /// <summary>
  /// Constructor with the same mode for all options.
  /// </summary>
  /// <param name="mode"></param>
  public WhitespaceOptions(WsMode mode) : this(mode, mode, mode)
  {
  }

  /// <summary>
  /// Constructor with mode and trim options.
  /// </summary>
  /// <param name="mode"></param>
  /// <param name="trimOptions"></param>
  public WhitespaceOptions(WsMode mode, TrimOptions trimOptions)
  {
    Mode = mode;
    Start = mode;
    End = mode;
    if (trimOptions.HasFlag(TrimOptions.TrimStart))
      Start = WsMode.Remove;
    if (trimOptions.HasFlag(TrimOptions.TrimEnd))
      End = WsMode.Remove;
  }

  /// <summary>
  /// What to do with whitespaces at the main part of the text.
  /// </summary>
  public WsMode Mode { get; set; }

  /// <summary>
  /// What to do with whitespaces at the start of the text.
  /// </summary>
  public WsMode Start { get; set; }


  /// <summary>
  /// What to do with whitespaces at the end of the text.
  /// </summary>
  public WsMode End { get; set; }

  /// <summary>
  /// How many spaces to use for a tab.
  /// </summary>
  public int TabSize { get; set; } = 4;

}