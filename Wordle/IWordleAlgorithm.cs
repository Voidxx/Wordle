using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordle
{
    internal interface IWordleAlgorithm
    {
        string EliminateImpossibleWords(List<LetterResult> feedback);

        string FocusOnConfirmingLetters(List<LetterResult> feedback);

        string LastChanceGuess(List<LetterResult> feedback);
    }
}
