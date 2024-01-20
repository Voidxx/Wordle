using Fleck;
using Newtonsoft.Json;
using Wordle;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System.Data.Common;
using System.Runtime.CompilerServices;


internal class Program
{
    public static async Task Main(string[] args)
    {
        // Start the WebSocket server
        Task serverTask = Task.Run(() => GameServer.main(args));

        // Prompt user for input
        Console.Write("Enter the number of agents: ");
        int numConnections = int.Parse(Console.ReadLine());


        // Create the desired number of connections and agents
        List<HubConnection> connections = new List<HubConnection>();
        List<WordleAgent> agents = new List<WordleAgent>();

        for (int i = 0; i < numConnections; i++)
        {
            HubConnection connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/gamehub")
                .Build();

            await connection.StartAsync();

            connections.Add(connection);

            string agentName = "Agent " + (i + 1);
            WordleAgent agent = new WordleAgent(connection, agents.Count + 1, agentName);
            agents.Add(agent);
        }

        // Initialize counters for each agent
        int[] agentWins = new int[numConnections];
        for (int i = 0; i < numConnections; i++)
        {
            agentWins[i] = 0;
        }

        // Start the tournament
        Tournament tournament = new Tournament(agents, 5);
        tournament.RunTournament();

        // Stop the WebSocket server
        await serverTask;
    }

    //static async Task gameLoop(WordleAgent agent1, WordleAgent agent2, int agent1Wins, int agent2Wins, bool switchAgents)
    //{
    //    bool winner = false;
    //    while ((agent1Wins < 2) && (agent2Wins < 2))
    //    {
    //        WordleAgent currentAgent = switchAgents ? agent1 : agent2;
    //        WordleAgent opponentAgent = switchAgents ? agent2 : agent1;

    //        // Select a word for the current agent
    //        string wordToGuess = currentAgent.possibleWords[new Random().Next(currentAgent.possibleWords.Count)];
    //        currentAgent.targetWord = wordToGuess;
    //        await currentAgent._connection.InvokeAsync("SetTargetWord", wordToGuess);
    //        Console.WriteLine("Agent " + (currentAgent.id) +" selected word: " + wordToGuess);

    //        // Switch roles
    //        switchAgents = !switchAgents;

    //        string guess;
    //        do
    //        {
    //            Thread.Sleep(1000);
    //            // Current agent tries to guess the word
    //            guess = await currentAgent.GenerateGuess();
    //            List<LetterResult> feedback = await opponentAgent.ReceiveFeedback();
    //            foreach (LetterResult letterResult in feedback)
    //            {
    //                currentAgent.feedbackHistory.Add(letterResult);
    //            }
    //            Console.WriteLine("Agent " + (opponentAgent.id) +" guesses: " + guess);
    //            Console.WriteLine("-------------------------");
    //            foreach (LetterResult item in feedback)
    //            {
    //                if (item.Exists == true && item.Position == true)
    //                {
    //                    Console.ForegroundColor = ConsoleColor.Green;
    //                    Console.Write("[" + item.Letter + "] ");
    //                    Console.ResetColor();
    //                }
    //                else if (item.Exists == true && item.Position == false)
    //                {
    //                    Console.ForegroundColor = ConsoleColor.Yellow;
    //                    Console.Write("[" + item.Letter + "] ");
    //                    Console.ResetColor();
    //                }
    //                else
    //                {
    //                    Console.ForegroundColor = ConsoleColor.Red;
    //                    Console.Write("[" + item.Letter + "] ");
    //                    Console.ResetColor();
    //                }
    //            }
    //            Console.WriteLine("");

    //            // Check if the current agent has guessed the word
    //            if (guess == wordToGuess)
    //            {
    //                winner = true;
    //                // If the current agent is agent1, increment agent1Wins
    //                // Otherwise, increment agent2Wins
    //                if (switchAgents)
    //          {
    //                    agent1Wins++;
    //                }
    //          else
    //                {
    //                    agent2Wins++;
    //                }
    //            }

    //        } while (guess != wordToGuess && currentAgent.guessCount < WordleAgent.guessLimit);
    //        if (winner == false)
    //        {
    //            Console.WriteLine("Agent " + (opponentAgent.id) + "loses round.");
    //        }
    //        else
    //        {
    //            Console.WriteLine("Agent " + (opponentAgent.id) + "wins round!.");
    //            winner = false;
    //        }
    //        cleanUpHistoryOfAgents(currentAgent, opponentAgent);
    //    }

    //    // Determine the final winner
    //    if (agent1Wins > agent2Wins)
    //    {
    //        Console.WriteLine("Agent 1 wins!");

    //    }
    //    else
    //    {
    //        Console.WriteLine("Agent 2 wins!");
    //    }
    //}


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