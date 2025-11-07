using CatchEleven.Services;
using CatchEleven.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;

namespace CatchEleven
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("🎴 Welcome to Catch Eleven!\n");

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IDeckService, DeckService>();

                    services.AddTransient<IGameService, GameService>();
                })
                .Build();

            var gameService = host.Services.GetRequiredService<IGameService>();

            gameService.StartGame();
            gameService.StopGame();

            Console.WriteLine("\n🏁 Game session finished.");
        }
    }
}