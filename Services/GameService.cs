using CatchEleven.Helpers;
using CatchEleven.Models;
using CatchEleven.Models.Players;
using CatchEleven.Services.Interfaces;

namespace CatchEleven.Services
{
    public class GameService : IGameService
    {
        private readonly IDeckService _deckService;

        private readonly Human _humanPlayer;
        private readonly Robot _robotPlayer;
        private readonly TableCards _tableCards;

        public GameService(IDeckService deckService)
        {
            _deckService = deckService;

            _humanPlayer = new Human();
            _robotPlayer = new Robot();
            _tableCards = new TableCards();
        }

        public void StartGame()
        {
            Console.WriteLine("Game started!\n");

            Console.WriteLine("\n---- 🔀 Shuffling Deck ----");
            _deckService.PerformShuffle();
            _deckService.DisplayDeck();

            var humanHand = DistributeCardsForPlayer(_humanPlayer);
            humanHand.DisplayCards("🧑‍💻 Cards dealt to Human:");

            var robotHand = DistributeCardsForPlayer(_robotPlayer);
            robotHand.DisplayCards("🤖 Cards dealt to Robot:");
            _robotPlayer.AddKnownCards(robotHand);

            var tableInitialCards = DistributeCardsForTable(_tableCards);
            tableInitialCards.DisplayCards("🃏 Cards on the Table:");
            _robotPlayer.AddKnownCards(tableInitialCards);

            Console.WriteLine("\n🂦 Remaining cards in deck:");
            _deckService.DisplayDeck();

            RunCombinationTest();
        }

        public void StopGame()
        {
            Console.WriteLine("\n Game stopped.");
        }

        // Distribute cards to a player
        private IList<Card> DistributeCardsForPlayer(IBasePlayer player)
        {
            Console.WriteLine($"Distributing cards to {player.GetType().Name}...");

            var dealtCards = new List<Card>();

            for (int i = 0; i < 4; i++)
            {
                var drawnCard = _deckService.DrawCard();
                if (drawnCard != null)
                {
                    dealtCards.Add(drawnCard);
                }
            }

            player.Hand = dealtCards;

            return dealtCards;
        }

        // Distribute cards to the table
        private IList<Card> DistributeCardsForTable(TableCards tableCards)
        {
            Console.WriteLine("Distributing cards to the table...");

            var tableDealtCards = new List<Card>();

            for (int i = 0; i < 4; i++)
            {
                var drawnCard = _deckService.DrawCard();
                if (drawnCard != null)
                {
                    tableDealtCards.Add(drawnCard);
                    tableCards.CardsOnTable.Add(drawnCard);
                }
            }

            return tableDealtCards;
        }

        private void RunCombinationTest()
        {
            Console.WriteLine("\n---- 🎯 Combination Analysis ----");
            var possibleCombinations = CombinationService.FindCombinationsForTargetScore(_tableCards, _humanPlayer.Hand);
            var bestCombination = CombinationService.ChooseBestCombination(possibleCombinations);

            if (bestCombination.Count == 0)
            {
                Console.WriteLine("❌ No valid combinations found.");
            }
            else
            {
                Console.WriteLine("✅ Best Combination:");
                Console.WriteLine(string.Join(", ", bestCombination));
            }
        }

        // Test: Find and display best combination
        private void Test()
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
