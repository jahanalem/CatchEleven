using CatchEleven.Domain.Models;

namespace CatchEleven.Application.Interfaces
{
    public interface IUserInputService
    {
        Card AskHumanToPlayCard(IList<Card> hand);
        int AskHumanToChooseCombination(IList<IList<Card>> combinations, Card playedCard);
        string WaitForEnter();
    }
}
