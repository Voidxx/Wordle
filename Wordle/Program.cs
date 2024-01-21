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

        Console.Write("Do you want to participate in the tournament? (yes/no): ");
        string userResponse = Console.ReadLine().ToLower();
        bool userParticipates = userResponse == "yes";

        Console.Write("Enter the number of agents: ");
        int numConnections = int.Parse(Console.ReadLine());



        List<HubConnection> connections = new List<HubConnection>();
        List<WordleAgent> agents = new List<WordleAgent>();
        if (userParticipates)
        {
            HubConnection userConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/gamehub")
                .Build();

            await userConnection.StartAsync();

            string userName = "User";
            WordleUser user = new WordleUser(userConnection, agents.Count + 1, userName);
            connections.Add(userConnection);
            agents.Add(user);
        }
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

        int[] agentWins = new int[numConnections];
        for (int i = 0; i < numConnections; i++)
        {
            agentWins[i] = 0;
        }

        Tournament tournament = new Tournament(agents, 5);
        tournament.RunTournament();

        // Stop the WebSocket server
        await serverTask;
    }


}