namespace CatchEleven.Domain.Models.Players
{
    public class Robot : BasePlayer
    {
        public ISet<Card> KnownCards { get; private set; } = new HashSet<Card>();

        public void AddKnownCard(Card card)
        {
            KnownCards.Add(card);
        }

        public void AddKnownCards(IEnumerable<Card> cards)
        {
            foreach (var card in cards)
            {
                KnownCards.Add(card);
            }
        }
    }
}
