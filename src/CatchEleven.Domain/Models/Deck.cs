namespace CatchEleven.Domain.Models
{
    public class Deck
    {
        public List<Card> Cards { get; private set; }

        public Deck()
        {
            Cards = new List<Card>(52);
        }
    }
}
