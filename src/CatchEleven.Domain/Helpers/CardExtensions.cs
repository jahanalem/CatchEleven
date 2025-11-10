using CatchEleven.Domain.Constants;
using CatchEleven.Domain.Models;

namespace CatchEleven.Domain.Helpers
{
    public static class CardExtensions
    {
        public static int Value(this Card card)
        {
            if (int.TryParse(card.Rank, out int numericValue))
            {
                return numericValue;
            }
            else if (card.Rank == "A")
            {
                return 1;
            }
            else
            {
                return 0; // For J, Q, K
            }
        }

        public static int Value(this string rank)
        {
            if (int.TryParse(rank, out int numericValue))
            {
                return numericValue;
            }
            else if (rank == "A")
            {
                return 1;
            }
            else
            {
                return 0; // For J, Q, K
            }
        }

        public static bool IsFaceCard(this Card card)
        {
            return card.Rank == "J" || card.Rank == "Q" || card.Rank == "K";
        }

        public static bool IsJack(this Card card)
        {
            return card.Rank == "J";
        }

        public static bool IsKingOrQueen(this Card card)
        {
            return card.Rank == "K" || card.Rank == "Q";
        }

        public static Card? FindJackOrDefault(this IEnumerable<Card> cards)
        {
            return cards.FirstOrDefault(card => card.IsJack());
        }

        public static int CountCardsBySymbol(this IList<Card> cards, string targetSymbol = Symbol.Diamonds)
        {
            int count = 0;
            foreach (var card in cards)
            {
                if (card.Suit.Symbol == targetSymbol)
                {
                    count++;
                }
            }

            return count;
        }

        public static bool IsThere2Diamond(this IList<Card> cards)
        {
            foreach (var card in cards)
            {
                if (card.Rank == "2" && card.Suit.Symbol == Symbol.Diamonds)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ContainsTwoOfDiamonds(this IList<Card> cards)
        {
            foreach (var card in cards)
            {
                if (card.Rank == "2" && card.Suit.Symbol == Symbol.Diamonds)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ContainsJackOfDiamonds(this IList<Card> cards)
        {
            foreach (var card in cards)
            {
                if (card.Rank == "J" && card.Suit.Symbol == Symbol.Diamonds)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
