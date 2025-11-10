using CatchEleven.Domain.Models.Symbols;

namespace CatchEleven.Domain.Models
{
    public record Card(string Rank, ISuit Suit)
    {
        /// <summary>
        /// Provides a string representation of the card, e.g., "K♠".
        /// </summary>
        public override string ToString()
        {
            return $"{Rank}{Suit.Symbol}";
        }
    }
}
