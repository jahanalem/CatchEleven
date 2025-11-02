using System.Text;

namespace CatchEleven.Models
{
    public class Deck
    {
        public List<Card> Cards { get; private set; }

        public Deck()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Cards = new List<Card>(52);
        }
    }
}
