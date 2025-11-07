using CatchEleven.Constants;
using CatchEleven.Helpers;

namespace CatchEleven.Models.Players
{
    public abstract class BasePlayer : IBasePlayer
    {
        public IList<Card> Hand { get; set; } = new List<Card>(4);
        public IList<Card> CollectedCards { get; set; } = new List<Card>();
        public int RoundScore { get; set; }

        /// <summary>
        /// Plays a card from the player's hand.
        /// If a specific card is provided, it plays that card.
        /// If not (cardToPlay is null), it automatically chooses the 'worst' card to discard.
        /// </summary>
        /// <param name="cardToPlay">A specific card the player wants to play. If null, a card will be chosen automatically.</param>
        /// <returns>The card that was played from the hand.</returns>
        public Card PlayCard(Card? cardToPlay)
        {
            Card selectedCard;

            // 1. A specific card was chosen (e.g., a Human's choice or a Robot's best combination)
            if (cardToPlay != null && Hand.Contains(cardToPlay))
            {
                selectedCard = cardToPlay;
            }
            else
            {
                // 2. No card was provided. This is a "discard" scenario.
                // We must automatically choose the "worst" card to play.
                selectedCard = ChooseCardToDiscard();
            }

            // 3. Remove the selected card from the hand and return it.
            Hand.Remove(selectedCard);
            return selectedCard;
        }

        /// <summary>
        /// Selects the "worst" card to discard from the hand, based on game strategy
        /// (e.g., when no capturing combination is possible).
        /// </summary>
        /// <returns>The chosen card to discard.</returns>
        private Card ChooseCardToDiscard()
        {
            // Get all non-Diamond cards
            var nonDiamondCards = Hand.Where(c => c.Suit.Symbol != Symbol.Diamonds).ToList();

            // --- 1. Try to discard a non-Diamond 'Q' or 'K' ---
            var kingOrQueen = nonDiamondCards.FirstOrDefault(c => c.Rank == "Q" || c.Rank == "K");
            if (kingOrQueen != null)
            {
                return kingOrQueen;
            }

            // --- 2. Try to discard the highest-value non-Diamond, non-Jack card ---
            var highestNonDiamond = nonDiamondCards
                .Where(c => !c.IsJack())
                .OrderByDescending(c => c.Value())
                .FirstOrDefault();

            if (highestNonDiamond != null)
            {
                return highestNonDiamond;
            }

            // --- 3. If we are here, the hand is only Diamonds or Jacks ---
            // Discard the highest-value Diamond (that isn't a Jack)
            var highestDiamond = Hand
                .Where(c => c.Suit.Symbol == Symbol.Diamonds && !c.IsJack())
                .OrderByDescending(c => c.IsFaceCard()) // Prioritize Q, K
                .ThenByDescending(c => c.Value()) // Then 10, 9, etc.
                .FirstOrDefault();

            if (highestDiamond != null)
            {
                return highestDiamond;
            }

            // --- 4. If we are still here, the hand must be *only* Jacks ---
            // Just play the first card.
            // First() will throw an error if the hand is empty, which is correct.
            // A player cannot play a card from an empty hand.
            return Hand.First();
        }
    }
}