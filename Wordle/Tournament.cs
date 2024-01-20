using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wordle
{
    public class Tournament
    {
        private List<WordleAgent> agents;
        private int numRounds;

        internal Tournament(List<WordleAgent> agents, int numRounds)
        {
            this.agents = agents;
            this.numRounds = numRounds;
        }

        public async void RunTournament()
        {
            Dictionary<string, string> matchResults = new Dictionary<string, string>();
            List<WordleAgent> remainingAgents = new List<WordleAgent>(agents);

            for (int round = 0; round < numRounds; round++)
            {
                if (remainingAgents.Count == 1)
                {
                    Console.WriteLine($"{remainingAgents[0].name} wins the tournament!");
                    break;
                }
                else if (remainingAgents.Count % 2 != 0)
                {
                    throw new Exception("Number of remaining agents must be even.");
                }

                List<WordleAgent> nextRound = new List<WordleAgent>();
                List<Task<WordleAgent>> matchTasks = new List<Task<WordleAgent>>();
                IOutputStrategy strategy = new AgentOnlyOutputStrategy();

                foreach (WordleAgent agent in remainingAgents)
                {
                    if (agent is WordleUser)
                        strategy = new UserAndAgentOutputStrategy();
                }

                for (int i = 0; i < remainingAgents.Count; i += 2)
                {
                    WordleAgent agent1 = remainingAgents[i];
                    WordleAgent agent2 = remainingAgents[i + 1];

                    string matchId = $"{agent1.id}-{agent2.id}"; // Unique match ID

                    Console.WriteLine($"{agent1.name} vs. {agent2.name}");


                    matchTasks.Add(PlayMatch(agent1, agent2, matchId, strategy));
                }
                // Wait for all matches to complete
                WordleAgent[] winners = await Task.WhenAll(matchTasks);

                foreach (var winner in winners)
                {
                    nextRound.Add(winner);
                }
                // Print the tournament bracket
                Console.WriteLine("Tournament Bracket:");
                foreach (var match in matchResults)
                {
                    Console.WriteLine($"{match.Key}: {match.Value}");
                }
                Console.WriteLine();
                remainingAgents = nextRound;
            }

            // Display the winner
            WordleAgent finalWinner = remainingAgents[0];
            Console.WriteLine($"{finalWinner.name} wins the tournament!");
        }

        private async Task<WordleAgent> PlayMatch(WordleAgent agent1, WordleAgent agent2, string matchId, IOutputStrategy outputStrategy)
        {
            // Get the SharedState instance for this match
            SharedState sharedState = SharedState.GetInstance(matchId);

            // Play best-of-three matches between agent1 and agent2
            bool switchAgents = false;

            while (agent1.Wins < 2 && agent2.Wins < 2)
            {
                WordleAgent currentAgent = switchAgents ? agent1 : agent2;
                WordleAgent opponentAgent = switchAgents ? agent2 : agent1;
                string wordToGuess = "";

                if (currentAgent is WordleUser)
                {
                    while (true)
                    {
                        Console.Write("Enter your 5 letter word for the agent to guess: ");
                        wordToGuess = Console.ReadLine();

                        if (wordToGuess.Length != 5 || wordToGuess.Any(char.IsDigit))
                        {
                            Console.WriteLine("Invalid input. Please enter a 5 letter word.");
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                // Select a word for the current agent
                else
                    wordToGuess = currentAgent.possibleWords[new Random().Next(currentAgent.possibleWords.Count)];
                currentAgent.targetWord = wordToGuess;
                sharedState.TargetWord = wordToGuess;
                await currentAgent._connection.InvokeAsync("SetTargetWord", wordToGuess, matchId);
                outputStrategy.DisplayMessage(currentAgent, opponentAgent, wordToGuess);

                string guess;
                do
                {
                    if (currentAgent is not WordleUser && opponentAgent is WordleUser)
                    {
                        currentAgent.isOpponentUser = true;
                    }
                    // Current agent tries to guess the word
                    guess = await opponentAgent.GenerateGuess(matchId);
                    List<LetterResult> feedback = await currentAgent.ReceiveFeedback(matchId);
                    foreach (LetterResult letterResult in feedback)
                    {
                        opponentAgent.feedbackHistory.Add(letterResult);
                    }

                    outputStrategy.DisplayGuessMessage(currentAgent, opponentAgent, guess);

                    // Check if the current agent has guessed the word
                    if (guess == wordToGuess)
                    {
                        outputStrategy.DisplayWinningMessage(currentAgent, opponentAgent);
                        if (switchAgents)
                        {
                            agent1.Wins++;
                        }
                        else
                        {
                            agent2.Wins++;
                        }
                    }

                } while (guess != wordToGuess && opponentAgent.guessCount < WordleAgent.guessLimit);

                cleanUpHistoryOfAgents(currentAgent, opponentAgent);
                // Switch roles
                switchAgents = !switchAgents;
            }

            // Determine the final winner
            WordleAgent winner = agent2.Wins > agent1.Wins ? agent1 : agent2;
            WordleAgent loser = agent2.Wins < agent1.Wins ? agent1 : agent2;
            outputStrategy.DisplayFinalMessage(winner, loser);
            winner.Wins = 0;
            winner.isOpponentUser = false;
            return winner;
        }
        private static void cleanUpHistoryOfAgents(WordleAgent agent1, WordleAgent agent2)
        {
            agent1.guessCount = 0;
            agent2.guessCount = 0;
            agent1.feedbackHistory.Clear();
            agent2.feedbackHistory.Clear();
            agent1.firstGuess = true;
            agent2.firstGuess = true;


        }


    }



}