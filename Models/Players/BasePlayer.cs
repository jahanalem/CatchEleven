namespace CatchEleven.Models.Players
{
    public abstract class BasePlayer : IBasePlayer
    {
        public Card[] Hand { get; set; } = new Card[4];
        public IList<Card> CollectedCards { get; set; } = [];
    }
}
