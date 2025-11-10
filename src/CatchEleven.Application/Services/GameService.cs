using CatchEleven.Application.Interfaces;
using CatchEleven.Application.Services.Interfaces;
using CatchEleven.Domain.Constants;
using CatchEleven.Domain.Helpers;
using CatchEleven.Domain.Models;
using CatchEleven.Domain.Models.Players;

namespace CatchEleven.Application.Services
{
    public class GameService : IGameService
    {
        private readonly IDeckService _deckService;
        private readonly IGamePresenter _presenter;
        private readonly IUserInputService _inputService;

        private readonly Human _humanPlayer;
        private readonly Robot _robotPlayer;
        private readonly TableCards _tableCards;

        private IBasePlayer _currentPlayer;
        private readonly Random _random = new Random();

        private int _humanTotalScore = 0;
        private int _robotTotalScore = 0;

        public GameService(
            IDeckService deckService,
            IGamePresenter presenter,
            IUserInputService inputService)
        {
            _deckService = deckService;
            _presenter = presenter;
            _inputService = inputService;

            _humanPlayer = new Human();
            _robotPlayer = new Robot();
            _tableCards = new TableCards();
        }


        public void StartGame()
        {
            _presenter.DisplayMessage("🎴 Welcome to Catch Eleven!\n");

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
                    _presenter.DisplayMessage("\n Press Enter to start the next round...");
                    _inputService.WaitForEnter();
                }
            }

            // The game is over, Program.cs will call StopGame()
        }

        public void RunRound()
        {
            _presenter.DisplayMessage("\n---- 🔄 Starting New Round ----");

            // 1. Reset deck and all card piles for a new round
            _deckService.ResetDeck();
            _tableCards.CardsOnTable.Clear();
            _humanPlayer.CollectedCards.Clear();
            _robotPlayer.CollectedCards.Clear();
            _robotPlayer.KnownCards.Clear();
            _presenter.DisplayMessage("\n---- 🔀 Shuffling Deck ----");
            _deckService.PerformShuffle();
            //_deckService.DisplayDeck();

            // 2. Initial Deal (as per rules)
            DistributeCardsForPlayer(_humanPlayer);
            DistributeCardsForPlayer(_robotPlayer);
            DistributeCardsForTable(_tableCards);

            // test code to show hands at start
            _presenter.DisplayCards("Your Hand:", _humanPlayer.Hand);
            _presenter.DisplayCards("🤖 Cards dealt to Robot:", _robotPlayer.Hand);
            _robotPlayer.AddKnownCards(_robotPlayer.Hand); // Robot knows its own hand
            _presenter.DisplayCards("🃏 Cards on the Table:", _tableCards.CardsOnTable);
            _robotPlayer.AddKnownCards(_tableCards.CardsOnTable);

            // 3. Choose starting player randomly
            _currentPlayer = _random.Next(2) == 0 ? _humanPlayer : _robotPlayer;
            _presenter.DisplayMessage($"\n🎉 {_currentPlayer.GetType().Name} will start the game!");
            _presenter.DisplayMessage("Press Enter to start...");
            _inputService.WaitForEnter();

            // 4. Main Game Loop (for this round)
            while (true)
            {
                // 4a. Check if hands are empty to deal new ones
                if (_humanPlayer.Hand.Count == 0 && _robotPlayer.Hand.Count == 0)
                {
                    // Check if the deck is also empty
                    if (_deckService.Deck.Cards.Count == 0)
                    {
                        _presenter.DisplayMessage("\nAll cards have been played!");
                        break; // Exit the main game loop (game over)
                    }

                    // Deck is not empty, deal new hands (Rule: no new cards to table)
                    _presenter.DisplayMessage("\n---- 🤲 Dealing new hands (4 cards each) ----");
                    DistributeCardsForPlayer(_humanPlayer);
                    DistributeCardsForPlayer(_robotPlayer);

                    // (Show hands again for testing)
                    _presenter.DisplayCards("Your Hand:", _humanPlayer.Hand);
                    _presenter.DisplayCards("🤖 New Hand Robot:", _robotPlayer.Hand);
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
                _presenter.DisplayMessage("\nPress Enter to continue to the next turn...");
                _inputService.WaitForEnter();
            }

            // 5. The round is over.
        }

        public void StopGame()
        {
            _presenter.DisplayMessage("\n Game stopped.");

            // Calculate and show the final scores for the round
            CalculateAndDisplayScores();
        }

        // Distribute cards to a player
        private IList<Card> DistributeCardsForPlayer(IBasePlayer player)
        {
            _presenter.DisplayMessage($"Distributing cards to {player.GetType().Name}...");

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
            _presenter.DisplayMessage("Distributing cards to the table...");

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
            _presenter.DisplayMessage("\n---- 🤖 Robot's Turn ----");

            // --- Added for testing ---
            _presenter.DisplayCards("🤖 Robot Hand (Before Play):", _robotPlayer.Hand);
            _presenter.DisplayCards("🃏 Cards on Table (Before Play):", _tableCards.CardsOnTable);
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
                _presenter.DisplayMessage($"❌ No combinations found. Robot discards: {playedCard}");
            }
            else
            {
                // --- 2. CAPTURE LOGIC ---
                _presenter.DisplayMessage("✅ Best Combination Found:");
                _presenter.DisplayMessage(string.Join(", ", bestCombination));

                // The last element is the played card
                playedCard = _robotPlayer.PlayCard(bestCombination.LastOrDefault());

                // Use the new capture method
                CaptureCombination(_robotPlayer, bestCombination, playedCard);

                _presenter.DisplayMessage($"\n🃏 Robot played: {playedCard}");
                _presenter.DisplayMessage($"   Cards Collected by Robot: {_robotPlayer.CollectedCards.Count}");
            }

            _presenter.DisplayMessage("");
            _presenter.DisplayCards("🤖 Robot Hand (After Play):", _robotPlayer.Hand);

            _presenter.DisplayMessage("\n🃏 Cards remaining on the Table (After Play):");
            _presenter.DisplayCards("", _tableCards.CardsOnTable);
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
            _presenter.DisplayMessage("\n---- 🧮 Round Over: Calculating Scores ----");

            var (humanScore, robotScore) = ScoreService.CalculateRoundScores(_humanPlayer, _robotPlayer);

            _humanTotalScore += humanScore;
            _robotTotalScore += robotScore;

            _presenter.DisplayMessage($"🧑‍💻 Human Score this round: {humanScore}");
            _presenter.DisplayMessage($"🤖 Robot Score this round: {robotScore}");
            _presenter.DisplayMessage("---------------------------------");
            _presenter.DisplayMessage($"🧑‍💻 Human Total Score: {_humanTotalScore}");
            _presenter.DisplayMessage($"🤖 Robot Total Score: {_robotTotalScore}");
            _presenter.DisplayMessage("---------------------------------");

            // Reset round scores for next round
            _humanPlayer.RoundScore = 0;
            _robotPlayer.RoundScore = 0;
        }

        private void TakeHumanTurn()
        {
            _presenter.DisplayMessage("\n---- 🧑‍💻 Your Turn ----");
            _presenter.DisplayCards("Cards on Table:", _tableCards.CardsOnTable);

            // 1. Get the card to play from the input service
            Card cardToPlay = _inputService.AskHumanToPlayCard(_humanPlayer.Hand);

            // 2. Play the card
            Card playedCard = _humanPlayer.PlayCard(cardToPlay);
            _presenter.DisplayMessage($"\n🧑‍💻 You played: {playedCard}");

            // 3. Handle game logic
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
                        _presenter.DisplayMessage("Only one valid combination found.");
                        CaptureCombination(_humanPlayer, validCombinations[0], playedCard);
                    }
                    else
                    {
                        int choice = _inputService.AskHumanToChooseCombination(validCombinations, playedCard);
                        var chosenCombination = validCombinations[choice - 1];
                        CaptureCombination(_humanPlayer, chosenCombination, playedCard);
                    }
                }
                else
                {
                    HandleDiscard(_humanPlayer, playedCard);
                }
            }
            _presenter.DisplayMessage("");
            _presenter.DisplayCards("Your Hand:", _humanPlayer.Hand);
        }

        private void CaptureCombination(IBasePlayer player, IList<Card> combination, Card playedCard)
        {
            var capturedCards = combination.Where(c => c != playedCard).ToList();
            _presenter.DisplayMessage($"Capturing: {string.Join(", ", capturedCards)}");

            // Check for Basaat
            if (capturedCards.Count == _tableCards.CardsOnTable.Count)
            {
                _presenter.DisplayMessage($"💥 Basaat! {player.GetType().Name} cleared the table!");
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

        private void HandleDiscard(IBasePlayer player, Card playedCard)
        {
            _presenter.DisplayMessage($"It doesn't capture anything and is placed on the table.");
            _tableCards.CardsOnTable.Add(playedCard);
            _robotPlayer.AddKnownCard(playedCard); // Robot learns the discarded card
        }

        private void HandleJackPlay(IBasePlayer player, Card playedCard)
        {
            if (_tableCards.CardsOnTable.Count > 0)
            {
                _presenter.DisplayMessage($"   Jack captures all cards!");
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
                _presenter.DisplayMessage($"Table is empty. The Jack is placed on the table.");
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
                _presenter.DisplayMessage($"   Successfully capturing: {matchingCard}");
                player.CollectedCards.Add(playedCard);
                player.CollectedCards.Add(matchingCard);
                _tableCards.CardsOnTable.Remove(matchingCard); // Only remove table card
                _robotPlayer.AddKnownCard(playedCard);
                // Check for Basaat
                if (_tableCards.CardsOnTable.Count == 0)
                {
                    _presenter.DisplayMessage("💥 Basaat! You cleared the table!");
                    player.RoundScore += Score.Basaat;
                    _presenter.DisplayMessage($"💥 You scored a Basaat! +{Score.Basaat} point!");
                }
            }
            else
            {
                // Discard
                HandleDiscard(player, playedCard);
            }
        }
    }
}
