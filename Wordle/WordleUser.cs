using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Data.Common;
using Wordle;

public class WordleUser : WordleAgent
{
    public WordleUser(HubConnection connection, int id, string name)
        : base(connection, id, name)
    {
    }

    public override async Task<string> GenerateGuess(string matchId)
    {
        Console.Write("Enter your guess: ");
        string guess = Console.ReadLine();
        await _connection.InvokeAsync("ReceiveGuess", guess, matchId);
        guessCount++;
        return guess;
    }

    public override async Task<List<LetterResult>> ReceiveFeedback(string matchId)
    {
        // Retrieve the feedback from the server
        string feedbackString = await _connection.InvokeAsync<string>("ProvideFeedback", matchId);

        // Deserialize the feedback string into a list of LetterResult objects
        List<LetterResult> feedback = JsonConvert.DeserializeObject<List<LetterResult>>(feedbackString);

        foreach (LetterResult result in feedback)
        {
            feedbackHistory.Add(result);
        }



        return feedback;
    }
}