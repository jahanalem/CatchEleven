namespace CatchEleven.Domain.Models.Symbols
{
    public interface ISuit
    {
        string Symbol { get; }
        string[] Ranks { get; set; }
    }
}
