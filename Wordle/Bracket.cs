using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordle
{
    class Bracket
    {
        public List<Match> Matches { get; set; }
    }

    class Match
    {
        public WordleAgent Agent1 { get; set; }
        public WordleAgent Agent2 { get; set; }
        public WordleAgent Winner { get; set; }
    }
}
