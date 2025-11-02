namespace CatchEleven.Models.Symbols
{
    public abstract class BaseSuit : ISuit
    {
        public abstract string Symbol { get; }
        public string[] Ranks { get; set; } = new string[]
        {
            "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A"
        };
    }
}
