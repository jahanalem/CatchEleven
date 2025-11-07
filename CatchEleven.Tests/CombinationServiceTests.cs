using CatchEleven.Models;
using CatchEleven.Models.Symbols;
using CatchEleven.Services;

namespace CatchEleven.Tests
{
    public class CombinationServiceTests
    {
        [Fact]
        public void ChooseBestCombination_Should_Prefer_Diamonds_For_Scoring()
        {
            // --- 1. Arrange ---

            // Define the cards currently on the table
            var tableCards = new TableCards()
            {
                CardsOnTable = new List<Card>
                {
                    new Card("7", new Spades()), // 7♠
                    new Card("2", new Spades()), // 2♠
                    new Card("3", new Clubs()),  // 3♣
                    new Card("6", new Spades())  // 6♠
                }
            };

            // Define the cards in the player's hand
            var humanHand = new List<Card>
            {
                new Card("10", new Hearts()),   // 10♥
                new Card("5", new Spades()),    // 5♠  (Can combine with 6♠ to make 11)
                new Card("4", new Diamonds()),  // 4♦  (Can combine with 7♠ to make 11)
                new Card("10", new Clubs())    // 10♣
            };

            var expectedBestCombination = new List<Card>
            {
                new Card("4", new Diamonds()), // The card played from the hand
                new Card("7", new Spades())    // The card captured from the table
            };

            // --- 2. Act ---

            var possibleCombinations = CombinationService.FindCombinationsForTargetScore(tableCards, humanHand);
            var actualBestCombination = CombinationService.ChooseBestCombination(possibleCombinations);

            // --- 3. Assert ---

            // We sort both lists before comparing them. This ensures the test doesn't
            // fail just because the cards are in a different order.
            var sortedExpected = expectedBestCombination.OrderBy(c => c.Rank).ToList();
            var sortedActual = actualBestCombination.OrderBy(c => c.Rank).ToList();

            // Assert that the actual combination is identical to the expected one.
            Assert.Equal(sortedExpected, sortedActual);

            Assert.Equal(2, actualBestCombination.Count); // It should be a 2-card combination
            Assert.Contains(new Card("4", new Diamonds()), actualBestCombination); // It must contain the Diamond
            Assert.DoesNotContain(new Card("5", new Spades()), actualBestCombination); // It must NOT contain the 5 of Spades
        }
    }
}