using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordle
{
    internal interface IWordleAlgorithm
    {
        string GuessWord(List<LetterResult> feedback);
    }
}
