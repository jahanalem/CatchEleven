namespace CatchEleven.Domain.Models.Symbols
{
    public abstract class BaseSuit : ISuit
    {
        public abstract string Symbol { get; }
        public string[] Ranks { get; set; } = new string[]
        {
            "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A"
        };

        public override bool Equals(object? obj)
        {
            return obj is ISuit other && Symbol == other.Symbol;
        }

        public override int GetHashCode()
        {
            return Symbol.GetHashCode();
        }
    }
}
