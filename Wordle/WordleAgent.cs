using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fleck;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace Wordle
{

    internal class WordleAgent
    {
        public int id;
        public string name;
        private Algorithm algorithm;
        public List<string> possibleWords;
        private string currentGuess;
        public string targetWord;
        public bool firstGuess = true;
        public List<LetterResult> feedbackHistory;
        public int guessCount = 0;
        public const int guessLimit = 5; // Added guess limit
        public int Wins;

        public readonly HubConnection _connection;

        public WordleAgent(HubConnection connection, int id, string name)
        {
            _connection = connection;
            algorithm = new Algorithm();
            possibleWords = File.ReadAllLines("words.txt").ToList();
            targetWord = "";
            feedbackHistory = new List<LetterResult>();
            this.id = id;
            this.name = name;
            Wins = 0;
        }   
        public async Task<string> GenerateGuess()
        {
            if (firstGuess == true)
            {
                firstGuess = false;
                currentGuess = possibleWords[new Random().Next(possibleWords.Count)];
            }
            else
            {
                // Update algorithm with guess count and limit
                currentGuess = algorithm.GuessWord(feedbackHistory, guessLimit - guessCount);
            }
            guessCount++; // Increment guess count after generating a guess

            await _connection.InvokeAsync("ReceiveGuess", currentGuess);

            return currentGuess;
        }
        public async Task<List<LetterResult>> ReceiveFeedback()
        {

            // Retrieve the feedback from the server
            string feedbackString = await _connection.InvokeAsync<string>("ProvideFeedback");

            // Deserialize the feedback string into a list of LetterResult objects
            List<LetterResult> feedback = JsonConvert.DeserializeObject<List<LetterResult>>(feedbackString);

            foreach(LetterResult result in feedback)
            {
                feedbackHistory.Add(result);
            }

            return feedback;
        }

        private List<LetterResult> ParseFeedback(string feedback)
        {
            // Split the feedback string into an array of strings
            string[] feedbackStrings = feedback.Split(',');

            List<LetterResult> feedbackResults = new List<LetterResult>();

            // For each string in the array
            foreach (string feedbackString in feedbackStrings)
            {
                // Split the string into an array of property values
                string[] propertyValues = feedbackString.Split('|');

                // Create a new LetterResult object
                LetterResult result = new LetterResult
                {
                    Letter = propertyValues[0][0],
                    Position = bool.Parse(propertyValues[1]),
                    Exists = bool.Parse(propertyValues[2]),
                    LetterPosition = int.Parse(propertyValues[3]),
                    ConfirmedExistence = bool.Parse(propertyValues[4])
                };

                // Add the LetterResult object to the list
                feedbackResults.Add(result);
            }

            return feedbackResults;
        }
    }
}
