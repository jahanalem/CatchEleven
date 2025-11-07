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

            var actualBestCombination = CombinationService.ChooseBestCombination(tableCards, humanHand);

            // --- 3. Assert ---

            // We sort both lists before comparing them. This ensures the test doesn't
            // fail just because the cards are in a different order.
            var sortedExpected = expectedBestCombination.OrderBy(c => c.ToString()).ToList();
            var sortedActual = actualBestCombination.OrderBy(c => c.ToString()).ToList();

            // Assert that the actual combination is identical to the expected one.
            Assert.Equal(sortedExpected, sortedActual);

            Assert.Equal(2, actualBestCombination.Count); // It should be a 2-card combination
            Assert.Contains(new Card("4", new Diamonds()), actualBestCombination); // It must contain the Diamond
            Assert.DoesNotContain(new Card("5", new Spades()), actualBestCombination); // It must NOT contain the 5 of Spades
        }

        [Fact]
        public void ChooseBestCombination_Should_Select_Jack_To_Take_All_Table_Cards()
        {
            // --- 1. Arrange ---

            // Define the card suits needed for the test
            var hearts = new Hearts();
            var diamonds = new Diamonds();
            var spades = new Spades();

            // Scenario:
            // Hand: 5H, AD, 6H, JS
            // Table: 10D, 4D, QH, 8S
            var tableCards = new TableCards()
            {
                CardsOnTable = new List<Card>
                {
                    new Card("10", diamonds), // 10♦
                    new Card("4", diamonds),  // 4♦
                    new Card("Q", hearts),    // Q♥
                    new Card("8", spades)    // 8♠
                }
            };

            var humanHand = new List<Card>
            {
                new Card("5", hearts),    // 5♥
                new Card("A", diamonds),  // A♦
                new Card("6", hearts),    // 6♥
                new Card("J", spades)     // J♠ (The card that should be played)
            };

            // The expected combination is the Jack + ALL cards from the table.
            var expectedBestCombination = new List<Card>
            {
                new Card("10", diamonds), // All table cards
                new Card("4", diamonds),
                new Card("Q", hearts),
                new Card("8", spades),
                new Card("J", spades)     // The Jack from the hand
            };

            // --- 2. Act ---

            // Select the best one
            var actualBestCombination = CombinationService.ChooseBestCombination(tableCards, humanHand);

            // --- 3. Assert ---

            // Sort both lists to ensure the order doesn't cause a test failure.
            // We sort by the card's string representation (e.g., "10♦", "J♠").
            var sortedExpected = expectedBestCombination.OrderBy(c => c.ToString()).ToList();
            var sortedActual = actualBestCombination.OrderBy(c => c.ToString()).ToList();

            // Assert that the actual combination is identical to the expected one.
            Assert.Equal(sortedExpected, sortedActual);

            // Add more specific asserts to be extra clear
            Assert.Equal(5, actualBestCombination.Count); // 1 Jack + 4 table cards
            Assert.Contains(new Card("J", spades), actualBestCombination); // It must contain the Jack
            Assert.Contains(new Card("Q", hearts), actualBestCombination); // It must contain the Queen from the table
            Assert.Contains(new Card("10", diamonds), actualBestCombination); // It must contain the 10 from the table
        }
    }
}