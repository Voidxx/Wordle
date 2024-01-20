using Wordle;

public interface IOutputStrategy
{
    void DisplayMessage(WordleAgent currentAgent, WordleAgent opponentAgent, string wordToGuess);
    void DisplayWinningMessage(WordleAgent currentAgent, WordleAgent opponentAgent);
    void DisplayFinalMessage(WordleAgent winner, WordleAgent loser);

    void DisplayGuessMessage(WordleAgent currentAgent, WordleAgent opponentAgent, string guess);
}