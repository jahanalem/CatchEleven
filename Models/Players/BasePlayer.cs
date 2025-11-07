namespace CatchEleven.Models.Players
{
    public abstract class BasePlayer : IBasePlayer
    {
        public IList<Card> Hand { get; set; } = new List<Card>(4);
        public IList<Card> CollectedCards { get; set; } = [];
    }
}
