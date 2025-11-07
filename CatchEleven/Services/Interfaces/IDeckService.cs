using CatchEleven.Models;
using CatchEleven.Models.Symbols;

namespace CatchEleven.Services.Interfaces
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
