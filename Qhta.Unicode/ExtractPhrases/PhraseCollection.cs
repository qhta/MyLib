using System.Diagnostics;

namespace ExtractPhrases;

public class PhraseCollection: Dictionary<Phrase, int>
{
  /// <summary>
  /// Remove phrases that occur only once.
  /// </summary>
  /// <returns></returns>
  public int RemoveSingleCountPhrases()
  {
    var phrasesToRemove = new HashSet<Phrase>();
    foreach (var keyValuePair in this)
    {
      if (keyValuePair.Value == 1)
      {
        phrasesToRemove.Add(keyValuePair.Key);
      }
    }
    foreach (var phrase in phrasesToRemove)
      this.Remove(phrase);
    return phrasesToRemove.Count;
  }

  /// <summary>
  /// Remove phrases that are contained in another phrase with the same count
  /// </summary>
  /// <returns></returns>
  public int RemoveDuplicatedFragmentPhrases()
  {
    var phrasesToRemove = new HashSet<Phrase>();
    foreach (var keyValuePair in this)
    {
      foreach (var keyValuePair2 in this)
      {
        if (!keyValuePair.Key.Equals(keyValuePair2.Key) && keyValuePair.Value == keyValuePair2.Value && keyValuePair2.Key.Contains(keyValuePair.Key))
        {
          phrasesToRemove.Add(keyValuePair.Key);
          break;
        }
      }
    }
    foreach (var phrase in phrasesToRemove)
      this.Remove(phrase);
    return phrasesToRemove.Count;
  }

  /// <summary>
  ///  Remove single-word or two-word phrases from other phrases
  /// </summary>
  /// <returns></returns>
  public int RemoveShortPhrasesFromOthers(int length)
  {
    var singleWordPhrases = new Dictionary<Phrase, List<Phrase>>();
    foreach (var phrase in this)
      if (phrase.Key.Count == length)
        singleWordPhrases.Add(phrase.Key, new List<Phrase>());

    foreach (var phrase1 in singleWordPhrases.Keys)
    {
      foreach (var keyValuePair in this)
      {
        if (phrase1.Equals("ABBREVIATION") && keyValuePair.Key.Equals("ABBREVIATION MARK"))
          Debug.Assert(true);
        if (!phrase1.Equals(keyValuePair.Key) && keyValuePair.Key.Contains(phrase1))
        {
          singleWordPhrases[phrase1].Add(keyValuePair.Key);
        }
      }
    }
    var changesCount = 0;
    foreach (var item in singleWordPhrases)
    {
      var singleWord = item.Key;
      foreach (var phrase in item.Value)
      {
        var newPhrase = phrase.Remove(singleWord);
        if (newPhrase.IsEmpty()) continue;
        if (newPhrase.Equals(singleWord))
          continue;
        var count = this[phrase];
        if (!this.TryAdd(newPhrase, count))
          this[newPhrase] += count;
        this[singleWord] += count;
        changesCount++;
      }
    }

    foreach (var item in singleWordPhrases)
    {
      var singleWord = item.Key;
      foreach (var phrase in item.Value)
      {
        this.Remove(phrase);
      }
    }
    return changesCount;
  }

}