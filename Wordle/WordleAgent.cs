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
        public async Task<string> GenerateGuess(string matchId)
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

            await _connection.InvokeAsync("ReceiveGuess", currentGuess, matchId);

            return currentGuess;
        }
        public async Task<List<LetterResult>> ReceiveFeedback(string matchId)
        {

            // Retrieve the feedback from the server
            string feedbackString = await _connection.InvokeAsync<string>("ProvideFeedback", matchId);

            // Deserialize the feedback string into a list of LetterResult objects
            List<LetterResult> feedback = JsonConvert.DeserializeObject<List<LetterResult>>(feedbackString);

            foreach(LetterResult result in feedback)
            {
                feedbackHistory.Add(result);
            }

            return feedback;
        }

    }
}
