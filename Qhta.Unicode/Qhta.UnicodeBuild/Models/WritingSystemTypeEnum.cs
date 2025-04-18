using System.ComponentModel.DataAnnotations;

namespace Qhta.Unicode.Models;

public enum WritingSystemTypeEnum
{
  area = 1, // a range of code points that are not defined individually
  family = 2, // a set of scripts or other writing systems
  language = 3 , // a structured system of communication that consists of grammar and vocabulary
  script = 4, // a set of symbols used to write a language
  notation = 5, // a system of symbols used to represent information in some convention
  set = 6, // a set of symbols that can be scattered across different ranges
  subset = 7, // a subset of symbols belonging to a set
}
