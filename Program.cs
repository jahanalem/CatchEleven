using CatchEleven.Helpers;
using CatchEleven.Models.Players;
using CatchEleven.Services;
using System.Text;

namespace CatchEleven
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("🎴 Welcome to Catch Eleven!\n");

            var deckService = new DeckService();

            Console.WriteLine("🂡 Initial Deck:");
            deckService.DisplayDeck();

            Console.WriteLine("\n---- 🔀 Shuffling Deck ----");
            deckService.PerformShuffle();

            Console.WriteLine("\n🂱 Shuffled Deck:");
            deckService.DisplayDeck();

            // Start the game
            var gameService = new GameService();
            gameService.StartGame();

            var humanPlayer = new Human();
            var robotPlayer = new Robot();
            var tableCards = new Models.TableCards();

            // Distribute cards to human
            var humanHand = gameService.DistributeCardsForPlayer(humanPlayer, deckService.Deck);
            humanHand.DisplayCards("🧑‍💻 Cards dealt to Human:");

            // Distribute cards to robot
            var robotHand = gameService.DistributeCardsForPlayer(robotPlayer, deckService.Deck);
            robotHand.DisplayCards("🤖 Cards dealt to Robot:");
            robotPlayer.AddKnownCards(robotHand);

            // Distribute cards to the table
            var tableInitialCards = gameService.DistributeCardsForTable(tableCards, deckService.Deck);
            tableInitialCards.DisplayCards("🃏 Cards on the Table:");
            robotPlayer.AddKnownCards(tableInitialCards);

            // Display remaining deck
            Console.WriteLine("\n🂦 Remaining cards in deck:");
            deckService.DisplayDeck();

            // Find the best combination for the human player
            var possibleCombinations = CombinationService.FindCombinationsForTargetScore(tableCards, humanHand);
            var bestCombination = CombinationService.ChooseBestCombination(possibleCombinations);

            Console.WriteLine("\n---- 🎯 Combination Analysis ----");
            if (bestCombination.Count == 0)
            {
                Console.WriteLine("❌ No valid combinations found.");
                // TODO: Handle when no valid combination exists
            }
            else
            {
                Console.WriteLine("✅ Best Combination:");
                Console.WriteLine(string.Join(", ", bestCombination));
            }

            // End the game
            gameService.StopGame();

            Console.WriteLine("\n🏁 Game session finished.");
        }
    }
}
