namespace CatchEleven.Models.Players
{
    public interface IBasePlayer
    {
        Card[] Hand { get; set; }
        IList<Card> CollectedCards { get; set; }
    }
}
