using CatchEleven.Application.Interfaces;
using CatchEleven.Domain.Models;

namespace CatchEleven.Infrastructure.Services
{
    public class ConsolePresenter : IGamePresenter
    {
        public void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void DisplayCards(string message, IList<Card> cards)
        {
            Console.WriteLine(message);
            foreach (var card in cards)
            {
                Console.Write($"{card}  ");
            }
            Console.WriteLine();
        }

        public void ClearScreen()
        {
            throw new NotImplementedException();
        }
    }
}
