using System;

public class Game
{
    private readonly ConsoleUI _consoleUI = null!;

    public Game()
    {
        _consoleUI = new ConsoleUI(new MarketService());
    }

    public void Run()
    {
        _consoleUI.StartSimulation();
    }
}