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

            TakeRobotTurn();
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

        private void TakeRobotTurn()
        {
            Console.WriteLine("\n---- 🤖 Robot's Turn ----");

            var bestCombination = CombinationService.ChooseBestCombination(_tableCards, _robotPlayer.Hand);
            Card playedCard;

            if (bestCombination.Count == 0)
            {
                // --- 1. DISCARD LOGIC ---
                // No combination found. Robot must discard a card.
                // The PlayCard(null) method will automatically choose the "worst" card.
                playedCard = _robotPlayer.PlayCard(null);
                _tableCards.CardsOnTable.Add(playedCard); 
                _robotPlayer.AddKnownCard(playedCard);
                Console.WriteLine($"❌ No combinations found. Robot discards: {playedCard}");
            }
            else
            {
                // --- 2. CAPTURE LOGIC ---
                // A combination was found.

                // The last element is the played card
                playedCard = _robotPlayer.PlayCard(bestCombination.LastOrDefault());

                Console.WriteLine($"\n🃏 Robot played: {playedCard}");

                Console.WriteLine();
                Console.WriteLine("Robot Hand after playing the card:");
                _robotPlayer.Hand.DisplayCards();

                var capturedCards = bestCombination.Where(c => c != playedCard).ToList();
                RemoveCardsFromTable(capturedCards);

                foreach (var card in bestCombination)
                {
                    _robotPlayer.CollectedCards.Add(card);
                }

                Console.WriteLine("\n🃏 Cards remaining on the Table after Robot's play:");
                _tableCards.CardsOnTable.DisplayCards();

                Console.WriteLine("✅ Best Combination:");
                Console.WriteLine(string.Join(", ", bestCombination));
            }
        }

        private void RemoveCardsFromTable(IList<Card> cardsToRemove)
        {
            foreach (var card in cardsToRemove)
            {
                _tableCards.CardsOnTable.Remove(card);
            }
        }
    }
}
