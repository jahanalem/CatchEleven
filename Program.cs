using CatchEleven.Services;
using System.Text;

namespace CatchEleven
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Welcome to Catch Eleven!");
            var deckService = new DeckService();
            deckService.DisplayDeck();
            Console.WriteLine("---- Perform Shuffle ----");
            deckService.PerformShuffle();
            deckService.DisplayDeck();
        }
    }
}
