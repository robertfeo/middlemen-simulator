using System;

public class Game
{
    private readonly MarketService marketService = null!;
    private readonly ConsoleUI consoleUI = null!;

    public Game()
    {
        marketService = new MarketService();
        consoleUI = new ConsoleUI(marketService);
    }

    public void Run()
    {
        consoleUI.RunSimulation();
    }
}