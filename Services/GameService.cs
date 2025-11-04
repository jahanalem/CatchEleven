namespace CatchEleven.Services
{
    public class GameService
    {
        public GameService() { }
        public void StartGame()
        {
            Console.WriteLine("Game started!");
        }
        public void StopGame()
        {
            Console.WriteLine("Game stopped!");
        }
        public void Test()
        {
            var combinations = CombinationService.FindCombinationsForTargetScore(new Models.TableCards
            {
                CardsOnTable = new List<Models.Card>
                {
                    new Models.Card("2", new Models.Symbols.Hearts()),
                    new Models.Card("3", new Models.Symbols.Spades()),
                    new Models.Card("4", new Models.Symbols.Diamonds()),
                    new Models.Card("Q", new Models.Symbols.Spades()),
                    new Models.Card("K", new Models.Symbols.Clubs()),
                    new Models.Card("10", new Models.Symbols.Diamonds()),
                    new Models.Card("9", new Models.Symbols.Diamonds()),
                    new Models.Card("J", new Models.Symbols.Clubs())
                }
            }, new Models.Card("2", new Models.Symbols.Hearts()));

            var combination = CombinationService.ChooseBestCombination(combinations);
            Console.WriteLine("The best combination:");
            Console.WriteLine(string.Join(", ", combination));
        }
    }
}
