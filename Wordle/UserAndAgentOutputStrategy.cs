using Wordle;

public class UserAndAgentOutputStrategy : IOutputStrategy
{
    public void DisplayMessage(WordleAgent currentAgent, WordleAgent opponentAgent, string wordToGuess)
    {
        if (currentAgent is WordleUser)
        {
            Console.WriteLine("You have selected the word: " + wordToGuess);
        }
        else if(opponentAgent is WordleUser)
        {
            Console.WriteLine(currentAgent.name + " has selected a word for you to guess. ");
        }
    }

    public void DisplayWinningMessage(WordleAgent currentAgent, WordleAgent opponentAgent)
    {
        if (opponentAgent is WordleUser)
        {
            Console.WriteLine("You win the round!");
        }
        else if (currentAgent is WordleUser)
        {
            Console.WriteLine(opponentAgent.name + " wins the round!");
        }
    }

    public void DisplayFinalMessage(WordleAgent winner, WordleAgent loser)
    {
        if (winner is WordleUser)
        {
            Console.WriteLine("You win the match!");
        }
        else if(loser is WordleUser)
        {
            Console.WriteLine(winner.name + " wins the match!");
        }
    }

    public void DisplayGuessMessage(WordleAgent opponentAgent, WordleAgent guessingAgent, string guess)
    {
        if(guessingAgent is not WordleUser && opponentAgent is WordleUser)
        {
            Console.WriteLine("Your opponent guessed with: " + guess);
        }
    }
}