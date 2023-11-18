using System;

public class Game
{
    private readonly MarketService marketService = null!;

    public Game()
    {
        marketService = new MarketService();
        new ConsoleUI(marketService);
    }

    public void Run()
    {
        marketService.RunSimulation();
    }
}