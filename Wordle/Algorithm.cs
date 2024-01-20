using System.Diagnostics.Metrics;
using Wordle;

public class Algorithm
{
    public List<string> possibleWords = File.ReadAllLines("words.txt").ToList();


    public string GuessWord(List<LetterResult> feedback, int guessesLeft)
    {
        // Different strategies based on the number of guesses left
        if (guessesLeft > 3)
        {
            return EliminateImpossibleWords(feedback);
        }
        else if (guessesLeft > 1)
        {
            return AvoidRepeatedLetters(feedback);
        }
        else
        {
            return LastChanceGuess(feedback);
        }
    }

    private string EliminateImpossibleWords(List<LetterResult> feedback)
    {
        var filteredWords = new List<string>(possibleWords);

        foreach (LetterResult letterResult in feedback)
        {
            if (!letterResult.Exists)
            {
                filteredWords = filteredWords.Where(word => !word.Contains(letterResult.Letter)).ToList();
            }
            else if (!letterResult.Position)
            {
                filteredWords = filteredWords.Where(word => word.Contains(letterResult.Letter)).ToList();
            }
            else
            {
                filteredWords = filteredWords.Where(word => word[letterResult.LetterPosition] == letterResult.Letter).ToList();
            }
        }
        return filteredWords[new Random().Next(filteredWords.Count)];
    }


    private string AvoidRepeatedLetters(List<LetterResult> feedback)
    {
        var potentialWords = new List<string>(possibleWords);
        Dictionary<string, int> wordScores = new Dictionary<string, int>();

        // Get the letters used in the current round
        var usedLetters = feedback.Select(fr => fr.Letter).Distinct();

        foreach (var word in potentialWords)
        {
            int score = 0;
            foreach (var result in feedback)
            {
                if (result.Exists && result.Position && word[result.LetterPosition] == result.Letter && !word.Substring(0, result.LetterPosition).Contains(result.Letter))
                {
                    score++;
                }
            }

            // Exclude words that contain any of the used letters
            bool containsUsedLetters = usedLetters.Any(letter => word.Contains(letter));
            if (containsUsedLetters) continue;

            wordScores[word] = score;
        }

        string bestGuess = wordScores.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;

        if (string.IsNullOrEmpty(bestGuess))
        {
            bestGuess = EliminateImpossibleWords(feedback);
        }

        return bestGuess;
    }
    private string LastChanceGuess(List<LetterResult> feedback)
    {
        var filteredWords = possibleWords.Where(word =>
            feedback.All(fr => !fr.Position || (fr.Position && word[fr.LetterPosition] == fr.Letter))).ToList();

        foreach (var fr in feedback.Where(fr => fr.Exists && !fr.Position && fr.ConfirmedExistence))
        {
            filteredWords = filteredWords.Where(word => word.Contains(fr.Letter) && word[fr.LetterPosition] != fr.Letter && word[fr.LetterPosition] != fr.Letter).ToList();
        }

        Dictionary<string, int> wordScores = new Dictionary<string, int>();
        foreach (var word in filteredWords)
        {
            int score = feedback.Where(fr => fr.Exists && !fr.Position && fr.ConfirmedExistence).Count(fr => word.Contains(fr.Letter));
            wordScores[word] = score;
        }

        string bestGuess = wordScores.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;

        if (string.IsNullOrEmpty(bestGuess))
        {
            bestGuess = EliminateImpossibleWords(feedback);
        }

        return bestGuess;
    }


}