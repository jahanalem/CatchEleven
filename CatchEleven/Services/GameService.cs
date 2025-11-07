using CatchEleven.Constants;
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

        private int _humanTotalScore = 0;
        private int _robotTotalScore = 0;

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

            // Calculate and show the final scores for the round
            CalculateAndDisplayScores();
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

                // --- Basat Logic ---
                if (capturedCards.Count == _tableCards.CardsOnTable.Count && !playedCard.IsFaceCard())
                {
                    Console.WriteLine("🤖 Robot made a Basaat! (captured all table cards)");
                    _robotPlayer.RoundScore += Score.Basaat;
                    Console.WriteLine($"💥 Robot scored a Basaat! +{Score.Basaat} point!");
                }

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

        private void CalculateAndDisplayScores()
        {
            Console.WriteLine("\n---- 🧮 Round Over: Calculating Scores ----");

            var (humanScore, robotScore) = ScoreService.CalculateRoundScores(_humanPlayer, _robotPlayer);

            _humanTotalScore += humanScore;
            _robotTotalScore += robotScore;

            Console.WriteLine($"🧑‍💻 Human Score this round: {humanScore}");
            Console.WriteLine($"🤖 Robot Score this round: {robotScore}");
            Console.WriteLine("---------------------------------");
            Console.WriteLine($"🧑‍💻 Human Total Score: {_humanTotalScore}");
            Console.WriteLine($"🤖 Robot Total Score: {_robotTotalScore}");
            Console.WriteLine("---------------------------------");

            // Reset round scores for next round
            _humanPlayer.RoundScore = 0;
            _robotPlayer.RoundScore = 0;
        }
    }
}
