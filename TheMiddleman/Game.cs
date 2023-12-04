using System;

public class Game
{
    private readonly MarketService marketService = null!;

    public Game()
    {
        marketService = new MarketService();
    }

    public void Run()
    {
        new ConsoleUI(marketService);
        marketService.RunSimulation();
    }
}