using System.ComponentModel.DataAnnotations;

namespace Qhta.Unicode.Models;

public enum WritingSystemTypeEnum: byte
{
  Area = 1, // a range of code points that are not defined individually
  Family = 2, // a set of scripts or other writing systems
  Language = 3 , // a structured system of communication that consists of grammar and vocabulary
  Script = 4, // a set of symbols used to write a language
  Notation = 5, // a system of symbols used to represent information in some convention
  Set = 6, // a set of symbols that can be scattered across different ranges
  Subset = 7, // a subset of symbols belonging to a set
}
