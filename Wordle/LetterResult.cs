using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordle
{
    public class LetterResult
    {
        public char Letter { get; set; }
        public int LetterPosition { get; set; }
        public bool Position { get; set; }
        public bool Exists { get; set; }
        public bool ConfirmedExistence { get; set; }
    }
}
