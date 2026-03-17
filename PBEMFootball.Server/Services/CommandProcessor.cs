namespace PBEMFootball.Server.Services;

public class CommandProcessor
{
    private readonly GameEngine _gameEngine;

    public CommandProcessor(GameEngine engine)
    {
        _gameEngine = engine;
    }

    public string ProcessCommand(string command)
    {
        return "Comando ricevuto: " + command;
    }
}