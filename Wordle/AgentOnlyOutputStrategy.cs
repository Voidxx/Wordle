using Wordle;

public class AgentOnlyOutputStrategy : IOutputStrategy
{
    public void DisplayMessage(WordleAgent currentAgent, WordleAgent opponentAgent, string wordToGuess)
    {
        Console.WriteLine(currentAgent.name + " has selected the word: " + wordToGuess);
    }

    public void DisplayWinningMessage(WordleAgent currentAgent, WordleAgent opponentAgent)
    {
        Console.WriteLine(currentAgent.name + " wins the round!");
    }

    public void DisplayFinalMessage(WordleAgent winner, WordleAgent loser)
    {
        Console.WriteLine(winner.name + " wins the match!");
    }

    public void DisplayGuessMessage(WordleAgent currentAgent, WordleAgent opponentAgent, string guess)
    {
        Console.WriteLine(opponentAgent.name + " guessed with: " + guess);
    }
}