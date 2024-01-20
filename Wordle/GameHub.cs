using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Wordle;

public class GameHub : Hub
{

    public async Task SetTargetWord(string targetWord, string matchId)
    {
        // Get the SharedState instance for this match
        SharedState sharedState = SharedState.GetInstance(matchId);

        // Store the target word in memory until it's needed
        sharedState.TargetWord = targetWord;
    }

    public async Task ReceiveGuess(string guess, string matchId)
    {
        // Get the SharedState instance for this match
        SharedState sharedState = SharedState.GetInstance(matchId);

        sharedState.Guess = guess;
    }
    public async Task<string> ProvideFeedback(string matchId)
    {
        // Get the SharedState instance for this match
        SharedState sharedState = SharedState.GetInstance(matchId);

        string guess = sharedState.Guess;

        // Process the guess and generate feedback
        List<LetterResult> feedback = GenerateFeedback(guess, matchId);

        // Convert the feedback to a string and return it
        string feedbackString = JsonConvert.SerializeObject(feedback);
        return feedbackString;
    }
    private List<LetterResult> GenerateFeedback(string guess, string matchId)
    {
        // Get the SharedState instance for this match
        SharedState sharedState = SharedState.GetInstance(matchId);

        // Retrieve the target word from memory
        string targetWord = sharedState.TargetWord;
        // Initialize an empty list to hold the feedback
        List<LetterResult> feedback = new List<LetterResult>();

        // Loop over each character in the guess
        for (int i = 0; i < guess.Length; i++)
        {
            char guessChar = guess[i];
            char targetChar = targetWord[i];

            // Create a new LetterResult object
            LetterResult result = new LetterResult
            {
                Letter = guessChar,
                Position = guessChar == targetChar,
                Exists = targetWord.Contains(guessChar),
                LetterPosition = targetWord.IndexOf(guessChar),
                ConfirmedExistence = targetWord.IndexOf(guessChar) >= 0
            };

            // Add the LetterResult object to the feedback list
            feedback.Add(result);
        }

        return feedback;
    }
}