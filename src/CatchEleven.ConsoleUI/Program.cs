using CatchEleven.Application.Interfaces;
using CatchEleven.Application.Services;
using CatchEleven.Application.Services.Interfaces;
using CatchEleven.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;

namespace CatchEleven.ConsoleUI
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
                    // Core Application Services
                    services.AddSingleton<IDeckService, DeckService>();
                    services.AddTransient<IGameService, GameService>();

                    // Infrastructure Services (implementing Application interfaces)
                    services.AddSingleton<IGamePresenter, ConsolePresenter>();
                    services.AddSingleton<IUserInputService, ConsoleInputService>();
                })
                .Build();

            var gameService = host.Services.GetRequiredService<IGameService>();

            gameService.StartGame();
            gameService.StopGame();

            Console.WriteLine("\n🏁 Game session finished.");
        }
    }
}
