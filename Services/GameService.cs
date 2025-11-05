using CatchEleven.Helpers;
using CatchEleven.Models;
using CatchEleven.Models.Players;

namespace CatchEleven.Services
{
    public class GameService
    {
        public GameService() { }

        public void StartGame()
        {
            Console.WriteLine("Game started!\n");
        }

        public void StopGame()
        {
            Console.WriteLine("\n Game stopped.");
        }

        // Distribute cards to a player
        public IList<Card> DistributeCardsForPlayer(IBasePlayer player, Deck deck)
        {
            Console.WriteLine($"Distributing cards to {player.GetType().Name}...");

            var dealtCards = new List<Card>();

            for (int i = 0; i < 4; i++)
            {
                var drawnCard = deck.Cards.DrawCard();
                if (drawnCard != null)
                {
                    dealtCards.Add(drawnCard);
                }
            }

            player.Hand = dealtCards;
            return dealtCards;
        }

        // Distribute cards to the table
        public IList<Card> DistributeCardsForTable(TableCards tableCards, Deck deck)
        {
            Console.WriteLine("Distributing cards to the table...");

            var tableDealtCards = new List<Card>();

            for (int i = 0; i < 4; i++)
            {
                var drawnCard = deck.Cards.DrawCard();
                if (drawnCard != null)
                {
                    tableDealtCards.Add(drawnCard);
                    tableCards.CardsOnTable.Add(drawnCard);
                }
            }

            return tableDealtCards;
        }

        // Test: Find and display best combination
        public void Test()
        {
            var table = new Models.TableCards
            {
                CardsOnTable = new List<Models.Card>
                {
                    new("2", new Models.Symbols.Hearts()),
                    new("3", new Models.Symbols.Spades()),
                    new("4", new Models.Symbols.Diamonds()),
                    new("Q", new Models.Symbols.Spades()),
                    new("K", new Models.Symbols.Clubs()),
                    new("10", new Models.Symbols.Diamonds()),
                    new("9", new Models.Symbols.Diamonds()),
                    new("J", new Models.Symbols.Clubs())
                }
            };

            var targetCard = new Models.Card("2", new Models.Symbols.Hearts());

            var combinations = CombinationService.FindCombinationsForTargetScore(table, targetCard);
            var bestCombination = CombinationService.ChooseBestCombination(combinations);

            Console.WriteLine("\n----  Best Combination Test ----");
            Console.WriteLine(bestCombination.Count == 0
                ? "No valid combinations found."
                : $"Best combination:\n{string.Join(", ", bestCombination)}");
        }
    }
}
