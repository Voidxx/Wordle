using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Wordle;

public class GameHub : Hub
{

    public async Task SetTargetWord(string targetWord, string matchId)
    {
        SharedState sharedState = SharedState.GetInstance(matchId);

        sharedState.TargetWord = targetWord;
    }

    public async Task ReceiveGuess(string guess, string matchId)
    {
        SharedState sharedState = SharedState.GetInstance(matchId);

        sharedState.Guess = guess;
    }
    public async Task<string> ProvideFeedback(string matchId)
    {
        SharedState sharedState = SharedState.GetInstance(matchId);

        string guess = sharedState.Guess;

        List<LetterResult> feedback = GenerateFeedback(guess, matchId);

        string feedbackString = JsonConvert.SerializeObject(feedback);
        return feedbackString;
    }
    private List<LetterResult> GenerateFeedback(string guess, string matchId)
    {
        SharedState sharedState = SharedState.GetInstance(matchId);

        string targetWord = sharedState.TargetWord;

        List<LetterResult> feedback = new List<LetterResult>();

        for (int i = 0; i < guess.Length; i++)
        {
            char guessChar = guess[i];
            char targetChar = targetWord[i];

            LetterResult result = new LetterResult
            {
                Letter = guessChar,
                Position = guessChar == targetChar,
                Exists = targetWord.Contains(guessChar),
                LetterPosition = targetWord.IndexOf(guessChar),
                ConfirmedExistence = targetWord.IndexOf(guessChar) >= 0
            };

            feedback.Add(result);
        }

        return feedback;
    }
}