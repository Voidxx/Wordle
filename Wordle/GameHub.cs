using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Wordle;

public class GameHub : Hub
{

    public async Task SetTargetWord(string targetWord)
    {
        // Store the target word in memory until it's needed
        SharedState.Instance.targetWord = targetWord;
    }
    public async Task ReceiveGuess(string guess)
    {
        SharedState.Instance.Guess = guess;

    }

    public async Task<string> ProvideFeedback()
    {
        string guess = SharedState.Instance.Guess;

        // Process the guess and generate feedback
        List<LetterResult> feedback = GenerateFeedback(guess);

        // Convert the feedback to a string and return it
        string feedbackString = JsonConvert.SerializeObject(feedback);
        return feedbackString;
    }
    private List<LetterResult> GenerateFeedback(string guess)
    {
        // Retrieve the target word from memory
        string targetWord = SharedState.Instance.targetWord;
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