using CatchEleven.Domain.Models;

namespace CatchEleven.Application.Interfaces
{
    public interface IGamePresenter
    {
        void DisplayMessage(string message);
        void DisplayCards(string message, IList<Card> cards);
        void ClearScreen();
    }
}
