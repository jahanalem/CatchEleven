namespace CatchEleven.Domain.Models
{
    public class TableCards
    {
        public IList<Card> CardsOnTable { get; set; } = new List<Card>();
    }
}
