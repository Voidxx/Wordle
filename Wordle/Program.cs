using Wordle;


internal class Program
{
    private static void Main(string[] args)
    {
        WordleAgent agent1 = new WordleAgent();
        WordleAgent agent2 = new WordleAgent();

        gameLoop(agent1, agent2);
    }


    static void gameLoop(WordleAgent agent1, WordleAgent agent2)
    {
        string winner = null;
        while (winner == null)
        {
            // Agent 1 selects a word
            string wordToGuess = agent1.possibleWords[new Random().Next(agent1.possibleWords.Count)];
            agent1.targetWord = wordToGuess;
            Console.WriteLine("agent 1 je odabrao riječ: " + wordToGuess);
            string guess;
            do
            {
                Thread.Sleep(1000);
                // Agent 2 tries to guess the word
                guess = agent2.GenerateGuess();
                List<LetterResult> feedback = new List<LetterResult>();
                feedback = agent1.ReceiveFeedback(guess);
                foreach(LetterResult letterResult in feedback)
                {
                    agent2.feedbackHistory.Add(letterResult);
                }
                Console.WriteLine("agent 2 pogađa sa riječi: " + guess);
                Console.WriteLine("-------------------------");
                foreach (LetterResult item in feedback)
                {
  

                    if (item.Exists == true && item.Position == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("[" + item.Letter + "] ");
                        Console.ResetColor();
                    }
                    else if(item.Exists == true && item.Position == false) 
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("[" + item.Letter + "] ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("[" + item.Letter + "] ");
                        Console.ResetColor();
                    }
                }
                Console.WriteLine("");

                // Check if Agent 2 has guessed the word
                if (guess == wordToGuess)
                {
                    winner = "Agent 2";
                }
            } while (guess != wordToGuess);

            // Switch roles
            WordleAgent temp = agent1;
            agent1 = agent2;
            agent2 = temp;
        }

        Console.WriteLine($"{winner} wins!");
    }
}