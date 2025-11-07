namespace CatchEleven.Models.Players
{
    public interface IBasePlayer
    {
        IList<Card> Hand { get; set; }
        IList<Card> CollectedCards { get; set; }
        Card PlayCard(Card? cardToPlay);
    }
}
