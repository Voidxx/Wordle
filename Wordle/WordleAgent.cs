using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordle
{
    internal class WordleAgent
    {
        private Algorithm algorithm;
        public List<string> possibleWords;
        private string currentGuess;
        public string targetWord;
        public bool firstGuess = true;
        public List<LetterResult> feedbackHistory;


        public WordleAgent()
        {
            algorithm = new Algorithm();
            possibleWords = File.ReadAllLines("words.txt").ToList();
            targetWord = "";
            feedbackHistory = new List<LetterResult>();
        }

        public string GenerateGuess()
        {
            if (firstGuess == true)
            {
                firstGuess = false;
                currentGuess = possibleWords[new Random().Next(possibleWords.Count)];
            }
            else
            {
                currentGuess = algorithm.GuessWord(feedbackHistory);
            }
            return currentGuess;
        }

        public List<LetterResult> ReceiveFeedback(string guess)
        {
            List<LetterResult> feedback = new List<LetterResult>();

            for (int i = 0; i < guess.Length; i++)
            {
                char letter = guess[i];
                LetterResult result = new LetterResult
                {
                    Letter = letter,
                    Position = targetWord.Length > 0 ? letter == targetWord[i] : false,
                    Exists = targetWord.Contains(letter),
                    LetterPosition = i,
                    ConfirmedExistence = feedbackHistory.All(fr => fr.Letter == letter ? fr.Exists : true)
                };

                feedback.Add(result);
                feedbackHistory.Add(result);
            }


            return feedback;
        }
    }
}
