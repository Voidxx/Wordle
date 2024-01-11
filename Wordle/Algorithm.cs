using Wordle;

public class Algorithm : IWordleAlgorithm
{
    public List<string> possibleWords = File.ReadAllLines("words.txt").ToList();

    public string GuessWord(List<LetterResult> feedback)
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
}