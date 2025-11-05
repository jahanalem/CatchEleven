using CatchEleven.Constants;
using CatchEleven.Helpers;
using CatchEleven.Models;

namespace CatchEleven.Services
{
    public static class CombinationService
    {
        public const int TargetScore = 11;

        public static IList<IList<Card>> FindCombinationsForTargetScore(TableCards tableCards, IList<Card> cards)
        {
            IList<IList<Card>> allCombinationsFromCurrentHand = new List<IList<Card>>();

            foreach (var card in cards)
            {
                if (card.IsFaceCard())
                {
                    foreach (var tableCard in tableCards.CardsOnTable)
                    {
                        if (tableCard.IsFaceCard() && tableCard.Rank == card.Rank)
                        {
                            allCombinationsFromCurrentHand.Add(new List<Card> { card, tableCard });
                            Console.WriteLine($"Found matching face card combination: {card} and {tableCard}");
                        }
                    }
                    continue;
                }
                var combinationsForCard = FindCombinationsForTargetScore(tableCards, card);
                foreach (var combination in combinationsForCard)
                {
                    allCombinationsFromCurrentHand.Add(combination);
                }
            }

            return allCombinationsFromCurrentHand;
        }

        public static IList<IList<Card>> FindCombinationsForTargetScore(TableCards tableCards, Card card)
        {
            if (card.IsFaceCard())
            {
                Console.WriteLine($"The played card {card} is a face card. No combinations possible.");
                return new List<IList<Card>>();
            }

            IList<IList<Card>> allCombinations = new List<IList<Card>>();
            IList<IList<Card>> validCombinations = new List<IList<Card>>();

            var excludedFaceCards = tableCards.CardsOnTable.Where(c => !c.IsFaceCard()).ToList();

            for (int combinationLength = 1; combinationLength <= excludedFaceCards.Count; combinationLength++)
            {
                allCombinations = GetAllCombinations(excludedFaceCards, combinationLength, card);
                if (allCombinations.Count > 0)
                {
                    Console.WriteLine("Found combinations that make 11 with the played card:");
                    foreach (var combination in allCombinations)
                    {
                        validCombinations.Add(combination);
                        Console.WriteLine(string.Join(", ", combination));
                    }
                }
            }

            return validCombinations;
        }

        public static void GenerateCombinations(IList<Card> list,
            int length,
            int startPosition,
            IList<Card> currentCombination,
            IList<IList<Card>> allCombinations, Card salt)
        {
            if (currentCombination.Count == length)
            {
                int sum = currentCombination.Aggregate(0, (sum, item) => sum + item.Value()) + salt.Value();
                if (sum == TargetScore)
                {
                    var saltIncludedCombination = new List<Card>(currentCombination) { salt };
                    allCombinations.Add(new List<Card>(saltIncludedCombination));
                }
                return;
            }
            for (int i = startPosition; i < list.Count; i++)
            {
                currentCombination.Add(list[i]);
                GenerateCombinations(list, length, i + 1, currentCombination, allCombinations, salt);
                currentCombination.RemoveAt(currentCombination.Count - 1);
            }
        }

        public static IList<IList<Card>> GetAllCombinations(IList<Card> list, int combinationLength, Card salt)
        {
            var allCombinations = new List<IList<Card>>();
            GenerateCombinations(list, combinationLength, 0, new List<Card>(), allCombinations, salt);
            return allCombinations;
        }

        public static IList<Card> ChooseBestCombination(IList<IList<Card>> combinations)
        {
            if (combinations == null || combinations.Count == 0)
            {
                Console.WriteLine("No combinations provided.");
                return new List<Card>();
            }

            IList<Card> bestCombination;

            IList<Card> largestCombination = combinations[0];
            foreach (var combination in combinations)
            {
                if (combination.Count > largestCombination.Count())
                {
                    largestCombination = combination;
                }
            }

            IList<Card> combinationWithMostDiamonds = new List<Card>();
            foreach (var combination in combinations)
            {
                var diamondCombination = combination.Where(c => c.Suit.Symbol == Symbol.Diamonds).ToList();
                if (diamondCombination.Count > combinationWithMostDiamonds.Count())
                {
                    combinationWithMostDiamonds = diamondCombination;
                }
            }

            bestCombination = largestCombination.CalculateHandWeight() >= combinationWithMostDiamonds.CalculateHandWeight()
                ? largestCombination
                : combinationWithMostDiamonds;

            return bestCombination;
        }
    }
}
