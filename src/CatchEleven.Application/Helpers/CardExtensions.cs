using CatchEleven.Domain.Constants;
using CatchEleven.Domain.Helpers;
using CatchEleven.Domain.Models;

namespace CatchEleven.Application.Helpers
{
    public static class CardExtensions
    {
        public static int CalculateHandWeight(this IList<Card> cards)
        {
            int totalWeight = 0;
            int bonusWeight = 0;
            var diamondCount = cards.CountCardsBySymbol(Symbol.Diamonds);
            var totalCards = cards.Count;
            if (cards.ContainsTwoOfDiamonds())
            {
                bonusWeight = 2;
            }
            totalWeight = diamondCount * 2 + bonusWeight + totalCards;

            return totalWeight;
        }
    }
}
