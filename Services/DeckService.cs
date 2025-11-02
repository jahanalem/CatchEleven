using CatchEleven.Models;
using CatchEleven.Models.Symbols;

namespace CatchEleven.Services
{
    public class DeckService
    {
        public Deck Deck { get; private set; }

        private static readonly ISuit[] _allSuits = new ISuit[]
        {
            new Clubs(),
            new Diamonds(),
            new Hearts(),
            new Spades()
        };

        private static readonly Random _random = new Random();
        public DeckService()
        {
            Deck = new Deck();
            ResetDeck();
        }

        public void ResetDeck()
        {
            Console.WriteLine("Initializing a new, ordered deck of cards...");
            Deck.Cards.Clear();

            foreach (var suit in _allSuits)
            {
                foreach (var rank in suit.Ranks)
                {
                    Deck.Cards.Add(new Card(rank, suit));
                }
            }
        }

        public void DisplayDeck()
        {
            Console.WriteLine("Current deck of cards:");
            Console.BackgroundColor = ConsoleColor.White;

            int i = 1;
            foreach (var card in Deck.Cards)
            {
                if (card.Suit.Symbol == "♥" || card.Suit.Symbol == "♦")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.Write($"{card}, ");

                if (i % 13 == 0)
                {
                    Console.WriteLine();
                }
                i++;
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        public void PerformShuffle()
        {
            Console.WriteLine("Shuffling the deck...");
            int n = Deck.Cards.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                Card temp = Deck.Cards[n];
                Deck.Cards[n] = Deck.Cards[k];
                Deck.Cards[k] = temp;
            }
        }

        public Card? DrawCard()
        {
            if (Deck.Cards.Count == 0)
            {
                Console.WriteLine("The deck is empty!");
                return null;
            }
            Card drawnCard = Deck.Cards[0];
            Deck.Cards.RemoveAt(0);

            return drawnCard;
        }

        public Card GetCard(ISuit suit, string rank)
        {
            if (!suit.Ranks.Contains(rank))
            {
                throw new ArgumentException("Invalid rank provided.");
            }
            return new Card(rank, suit);
        }

        public Card GetRandomCardFromSuit(ISuit suit)
        {
            string randomRank = suit.Ranks[_random.Next(suit.Ranks.Length)];
            return GetCard(suit, randomRank);
        }
    }
}
