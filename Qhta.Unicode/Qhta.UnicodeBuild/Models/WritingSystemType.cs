using System.ComponentModel.DataAnnotations;

namespace Qhta.Unicode.Models;

public enum WritingSystemType: byte
{
  Area = 1, // a range of code points that are not defined individually
  Family = 2, // a set of scripts or other writing systems
  Script = 3, // a set of symbols used to write a language
  Language = 4, // a structured system of communication that consists of grammar and vocabulary
  Notation = 5, // a system of symbols used to represent information in some convention
  SymbolSet = 6, // a set of symbols that can be scattered across different ranges
  Subset = 7, // a subset of symbols belonging to a set
  Artefact = 8, // an artefact of human culture
}
