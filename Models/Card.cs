using CatchEleven.Models.Symbols;

namespace CatchEleven.Models
{
    public class Card
    {
        public string Rank { get; }
        public ISuit Suit { get; }

        public Card(string rank, ISuit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        /// <summary>
        /// Provides a string representation of the card, e.g., "K♠".
        /// </summary>
        public override string ToString()
        {
            return $"{Rank}{Suit.Symbol}";
        }
    }
}
