using CatchEleven.Domain.Models;
using CatchEleven.Domain.Models.Symbols;

namespace CatchEleven.Application.Services.Interfaces
{
    public interface IDeckService
    {
        Deck Deck { get; }
        void ResetDeck();
        void DisplayDeck();
        void PerformShuffle();
        Card? DrawCard();
        Card GetCard(ISuit suit, string rank);
        Card GetRandomCardFromSuit(ISuit suit);
    }
}
