using CatchEleven.Constants;
using CatchEleven.Helpers;
using CatchEleven.Models;
using CatchEleven.Models.Players;
using CatchEleven.Services.Interfaces;
using System.Text;

namespace CatchEleven.Services
{
    public class GameService : IGameService
    {
        private readonly IDeckService _deckService;

        private readonly Human _humanPlayer;
        private readonly Robot _robotPlayer;
        private readonly TableCards _tableCards;

        private IBasePlayer _currentPlayer;
        private readonly Random _random = new Random();

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
            Console.WriteLine("🎴 Welcome to Catch Eleven!\n");

            // It runs as long as no player has reached the target score
            while (_humanTotalScore < Score.TargetWinScore && _robotTotalScore < Score.TargetWinScore)
            {
                // 1. Play one full round (deals 52 cards)
                RunRound();

                // 2. Calculate and display the scores for that round
                CalculateAndDisplayScores();

                // 3. Check if we need to play another round
                if (_humanTotalScore < Score.TargetWinScore && _robotTotalScore < Score.TargetWinScore)
                {
                    Console.WriteLine("\n Press Enter to start the next round...");
                    Console.ReadLine();
                }
            }

            // The game is over, Program.cs will call StopGame()
        }

        public void RunRound()
        {
            Console.WriteLine("\n---- 🔄 Starting New Round ----");

            // 1. Reset deck and all card piles for a new round
            _deckService.ResetDeck();
            _tableCards.CardsOnTable.Clear();
            _humanPlayer.CollectedCards.Clear();
            _robotPlayer.CollectedCards.Clear();
            _robotPlayer.KnownCards.Clear();
            Console.WriteLine("\n---- 🔀 Shuffling Deck ----");
            _deckService.PerformShuffle();
            //_deckService.DisplayDeck();

            // 2. Initial Deal (as per rules)
            DistributeCardsForPlayer(_humanPlayer);
            DistributeCardsForPlayer(_robotPlayer);
            DistributeCardsForTable(_tableCards);

            // test code to show hands at start
            _humanPlayer.Hand.DisplayCards("🧑‍💻 Cards dealt to Human:");
            _robotPlayer.Hand.DisplayCards("🤖 Cards dealt to Robot:");
            _robotPlayer.AddKnownCards(_robotPlayer.Hand); // Robot knows its own hand
            _tableCards.CardsOnTable.DisplayCards("🃏 Cards on the Table:");
            _robotPlayer.AddKnownCards(_tableCards.CardsOnTable);

            // 3. Choose starting player randomly
            _currentPlayer = (_random.Next(2) == 0) ? _humanPlayer : _robotPlayer;
            Console.WriteLine($"\n🎉 {_currentPlayer.GetType().Name} will start the game!");
            Console.WriteLine("Press Enter to start...");
            Console.ReadLine();

            // 4. Main Game Loop (for this round)
            while (true)
            {
                // 4a. Check if hands are empty to deal new ones
                if (_humanPlayer.Hand.Count == 0 && _robotPlayer.Hand.Count == 0)
                {
                    // Check if the deck is also empty
                    if (_deckService.Deck.Cards.Count == 0)
                    {
                        Console.WriteLine("\nAll cards have been played!");
                        break; // Exit the main game loop (game over)
                    }

                    // Deck is not empty, deal new hands (Rule: no new cards to table)
                    Console.WriteLine("\n---- 🤲 Dealing new hands (4 cards each) ----");
                    DistributeCardsForPlayer(_humanPlayer);
                    DistributeCardsForPlayer(_robotPlayer);

                    // (Show hands again for testing)
                    _humanPlayer.Hand.DisplayCards("🧑‍💻 New Hand Human:");
                    _robotPlayer.Hand.DisplayCards("🤖 New Hand Robot:");
                    _robotPlayer.AddKnownCards(_robotPlayer.Hand);
                }

                // 4b. Play the turn
                if (_currentPlayer == _humanPlayer)
                {
                    TakeHumanTurn();
                    _currentPlayer = _robotPlayer; // Switch turn
                }
                else
                {
                    TakeRobotTurn();
                    _currentPlayer = _humanPlayer; // Switch turn
                }

                // Optional: Pause to let user read
                Console.WriteLine("\nPress Enter to continue to the next turn...");
                Console.ReadLine();
            }

            // 5. The round is over.
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

            // --- Added for testing ---
            _robotPlayer.Hand.DisplayCards("🤖 Robot Hand (Before Play):");
            _tableCards.CardsOnTable.DisplayCards("🃏 Cards on Table (Before Play):");
            // --- End of testing addition ---

            var bestCombination = CombinationService.ChooseBestCombination(_tableCards, _robotPlayer.Hand);
            Card playedCard;

            if (bestCombination.Count == 0)
            {
                // --- 1. DISCARD LOGIC ---
                // No combination found. Robot must discard a card.
                // The PlayCard(null) method will automatically choose the "worst" card.
                playedCard = _robotPlayer.PlayCard(null);
                HandleDiscard(_robotPlayer, playedCard);
                Console.WriteLine($"❌ No combinations found. Robot discards: {playedCard}");
            }
            else
            {
                // --- 2. CAPTURE LOGIC ---
                Console.WriteLine("✅ Best Combination Found:");
                Console.WriteLine(string.Join(", ", bestCombination));

                // The last element is the played card
                playedCard = _robotPlayer.PlayCard(bestCombination.LastOrDefault());

                // Use the new capture method
                CaptureCombination(_robotPlayer, bestCombination, playedCard);

                Console.WriteLine($"\n🃏 Robot played: {playedCard}");
                Console.WriteLine($"   Cards Collected by Robot: {_robotPlayer.CollectedCards.Count}");
            }

            Console.WriteLine();
            _robotPlayer.Hand.DisplayCards("🤖 Robot Hand (After Play):");

            Console.WriteLine("\n🃏 Cards remaining on the Table (After Play):");
            _tableCards.CardsOnTable.DisplayCards();
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

        private void TakeHumanTurn()
        {
            Console.WriteLine("\n---- 🧑‍💻 Your Turn ----");
            _tableCards.CardsOnTable.DisplayCards("Cards on Table:");

            // 1. Ask the user to pick a card from their hand
            Card cardToPlay = AskHumanToPlayCard();

            // 2. Play the card (removes it from hand)
            Card playedCard = _humanPlayer.PlayCard(cardToPlay);
            Console.WriteLine($"\n🧑‍💻 You played: {playedCard}");

            // 3. Handle Special Cards (J, K, Q)
            // These rules are fixed and don't require sum-to-11
            if (playedCard.IsJack())
            {
                HandleJackPlay(_humanPlayer, playedCard);
            }
            else if (playedCard.IsKingOrQueen())
            {
                HandleKingQueenPlay(_humanPlayer, playedCard);
            }
            else
            {
                var validCombinations = CombinationService.FindCombinationsForTargetScore(_tableCards, playedCard);
                if (validCombinations != null && validCombinations.Count > 0)
                {
                    if (validCombinations.Count == 1)
                    {
                        // Auto-play the only valid combination
                        Console.WriteLine("Only one valid combination found.");
                        CaptureCombination(_humanPlayer, validCombinations[0], playedCard);
                    }
                    else
                    {
                        // Show options to select one
                        AskHumanToChooseCombination(validCombinations, playedCard);
                    }
                }
                else
                {
                    // No combinations found, must discard
                    HandleDiscard(_humanPlayer, playedCard);
                }
            }

            Console.WriteLine();
            _humanPlayer.Hand.DisplayCards("Your Hand (After Play):");

            //Console.WriteLine("\n🃏 Cards remaining on the Table (After Play):");
            //_tableCards.CardsOnTable.DisplayCards();
        }

        private Card AskHumanToPlayCard()
        {
            var hand = _humanPlayer.Hand;
            var options = new StringBuilder();
            options.AppendLine("Your Hand:");

            // Display hand as a numbered list
            for (int i = 0; i < hand.Count; i++)
            {
                options.AppendLine($"{i + 1}. {hand[i]}");
            }
            Console.Write(options.ToString());

            int choice = -1;
            while (choice < 1 || choice > hand.Count)
            {
                Console.Write($"Which card do you want to play? (1-{hand.Count}): ");
                var input = Console.ReadLine();
                if (!int.TryParse(input, out choice))
                {
                    choice = -1; // Invalid input, reset choice
                }
            }

            // Return the chosen card (e.g., choice 1 is index 0)
            return hand[choice - 1];
        }

        private void CaptureCombination(IBasePlayer player, IList<Card> combination, Card playedCard)
        {
            var capturedCards = combination.Where(c => c != playedCard).ToList();
            Console.WriteLine($"Capturing: {string.Join(", ", capturedCards)}");

            // Check for Basaat
            if (capturedCards.Count == _tableCards.CardsOnTable.Count)
            {
                Console.WriteLine($"💥 Basaat! {player.GetType().Name} cleared the table!");
                player.RoundScore += Score.Basaat;
            }

            // Add all cards (played + captured) to collected cards
            foreach (var card in combination)
            {
                player.CollectedCards.Add(card);
            }

            // Remove captured cards from table
            RemoveCardsFromTable(capturedCards);
        }

        private void AskHumanToChooseCombination(IList<IList<Card>> combinations, Card playedCard)
        {
            var options = new StringBuilder();
            options.AppendLine($"You have {combinations.Count} options for {playedCard}:");

            for (int i = 0; i < combinations.Count; i++)
            {
                var combo = combinations[i];
                var tableCards = combo.Take(combo.Count - 1); // All but the last (played) card
                options.AppendLine($"{i + 1}. Capture: {string.Join(", ", tableCards)}");
            }

            int discardOption = combinations.Count;
            Console.Write(options.ToString());

            int choice = -1;
            while (choice < 1 || choice > discardOption)
            {
                Console.Write($"Enter number (1-{discardOption}): ");
                var input = Console.ReadLine();
                if (!int.TryParse(input, out choice)) { choice = -1; }
            }

            // User chose a combination
            var chosenCombination = combinations[choice - 1]; // -1 for 0-based index
            CaptureCombination(_humanPlayer, chosenCombination, playedCard);
        }

        private void HandleDiscard(IBasePlayer player, Card playedCard)
        {
            Console.WriteLine($"   It doesn't capture anything and is placed on the table.");
            _tableCards.CardsOnTable.Add(playedCard);
            _robotPlayer.AddKnownCard(playedCard); // Robot learns the discarded card
        }

        private void HandleJackPlay(IBasePlayer player, Card playedCard)
        {
            if (_tableCards.CardsOnTable.Count > 0)
            {
                Console.WriteLine($"   Jack captures all cards!");
                var tableCardsCopy = new List<Card>(_tableCards.CardsOnTable);

                player.CollectedCards.Add(playedCard);
                foreach (var card in tableCardsCopy)
                {
                    player.CollectedCards.Add(card);
                }

                RemoveCardsFromTable(tableCardsCopy);
                _robotPlayer.AddKnownCard(playedCard);
                // No Basaat score for a Jack
            }
            else
            {
                // Discard
                Console.WriteLine($"Table is empty. The Jack is placed on the table.");
                HandleDiscard(player, playedCard);
            }
        }

        private void HandleKingQueenPlay(IBasePlayer player, Card playedCard)
        {
            // Find a matching King or Queen on the table
            var matchingCard = _tableCards.CardsOnTable.FirstOrDefault(c => c.Rank == playedCard.Rank);

            if (matchingCard != null)
            {
                // --- CAPTURE LOGIC ---
                Console.WriteLine($"   Successfully capturing: {matchingCard}");
                player.CollectedCards.Add(playedCard);
                player.CollectedCards.Add(matchingCard);
                _tableCards.CardsOnTable.Remove(matchingCard); // Only remove table card
                _robotPlayer.AddKnownCard(playedCard);
                // Check for Basaat
                if (_tableCards.CardsOnTable.Count == 0)
                {
                    Console.WriteLine("💥 Basaat! You cleared the table!");
                    player.RoundScore += Score.Basaat;
                    Console.WriteLine($"💥 You scored a Basaat! +{Score.Basaat} point!");
                }
            }
            else
            {
                // Discard
                Console.WriteLine($"It doesn't capture anything and is placed on the table.");
                HandleDiscard(player, playedCard);
            }
        }
    }
}
