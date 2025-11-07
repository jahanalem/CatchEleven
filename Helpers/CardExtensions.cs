using CatchEleven.Constants;
using CatchEleven.Models;

namespace CatchEleven.Helpers
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

        public static int CountCardsByRank(this IList<Card> cards, string targetRank = Symbol.Diamonds)
        {
            int count = 0;
            foreach (var card in cards)
            {
                if (card.Rank == targetRank)
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

        public static int CalculateHandWeight(this IList<Card> cards)
        {
            int totalWeight = 0;
            int bonusWeight = 0;
            var diamondCount = cards.CountCardsByRank(Symbol.Diamonds);
            var totalCards = cards.Count;
            if (cards.ContainsTwoOfDiamonds())
            {
                bonusWeight = 2;
            }
            totalWeight = diamondCount * 2 + bonusWeight + totalCards;

            return totalWeight;
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

        public static void DisplayCards(this IList<Card> cards, string message = "Cards:")
        {
            Console.WriteLine(message);
            foreach (var card in cards)
            {
                Console.Write($"{card}  ");
            }
            Console.WriteLine();
        }
    }
}
