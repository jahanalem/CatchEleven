using CatchEleven.Application.Interfaces;
using CatchEleven.Domain.Models;
using System.Text;

namespace CatchEleven.Infrastructure.Services
{
    public class ConsoleInputService : IUserInputService
    {
        public int AskHumanToChooseCombination(IList<IList<Card>> combinations, Card playedCard)
        {
            var options = new StringBuilder();
            options.AppendLine($"You have {combinations.Count} options for {playedCard}:");

            for (int i = 0; i < combinations.Count; i++)
            {
                var combo = combinations[i];
                var tableCards = combo.Take(combo.Count - 1);
                options.AppendLine($"{i + 1}. Capture: {string.Join(", ", tableCards)}");
            }

            Console.Write(options.ToString());

            int choice = -1;
            while (choice < 1 || choice > combinations.Count)
            {
                Console.Write($"Enter number (1-{combinations.Count}): ");
                var input = Console.ReadLine();
                if (!int.TryParse(input, out choice)) { choice = -1; }
            }

            return choice;
        }

        public Card AskHumanToPlayCard(IList<Card> hand)
        {
            var options = new StringBuilder();
            options.AppendLine("Your Hand:");

            // Display hand as a numbered list
            for (int i = 0; i < hand.Count; i++)
            {
                options.AppendLine($"{i + 1}. {hand[i]}");
            }
            Console.Write(options.ToString());

            int choice = -1;
            // Loop until a valid number is entered
            while (choice < 1 || choice > hand.Count)
            {
                Console.Write($"Which card do you want to play? (1-{hand.Count}): ");
                var input = Console.ReadLine();
                if (!int.TryParse(input, out choice))
                {
                    choice = -1; // Invalid input, reset choice
                }
            }

            return hand[choice - 1];
        }

        public string WaitForEnter()
        {
            return Console.ReadLine();
        }
    }
}
