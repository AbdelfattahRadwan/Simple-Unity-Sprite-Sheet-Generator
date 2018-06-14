using System;
using System.Globalization;

/// <summary>
/// A class that's used to execute editing commands by code instead of using the GUI.
/// </summary>
public static class CommandManager
{
    /// <summary>
    /// The output of commands execution during runtime.
    /// </summary>
    public static string CommandLog { get; private set; }

    /// <summary>
    /// The current command string input.
    /// </summary>
    public static string CommandInput { get; set; }

    /// <summary>
    /// The 'Add' command keyword.
    /// </summary>
    private const string addCommand = "add";

    /// <summary>
    /// The 'Move' command keyword.
    /// </summary>
    private const string moveCommand = "mov";

    /// <summary>
    /// The 'Delete' command keyword.
    /// </summary>
    private const string deleteCommand = "del";

    /// <summary>
    /// Writes a log message.
    /// </summary>
    /// <param name="message"></param>
    public static void WriteToLog(string message)
    {
        CommandLog += string.Concat(message, Environment.NewLine);
    }

    /// <summary>
    /// Executes a modification command.
    /// </summary>
    public static void Execute(string command)
    {
        var strings = command.Split('\n');

        foreach (var str in strings)
        {
            str.Trim();
        }

        var cmd = strings[0];
        var args = strings[1].Split('~');

        switch (cmd)
        {
            case addCommand:
                CmdAdd(args);
                break;
            case moveCommand:
                CmdMove(args);
                break;
            case deleteCommand:
                CmdDelete(args);
                break;

            default:
                WriteToLog($"command {cmd} isn't defined!");
                break;
        }
    }

    private static void CmdAdd(string[] args)
    {
        var sheet = MainWindow.Instance.GetOpenSpriteSheet();

        var textures = MainWindow.Instance.GetLoadedTextures();

        var spriteNodes = sheet.SpriteNodes;

        var index = int.Parse(args[0], NumberStyles.Any);

        spriteNodes.Add(new SpriteNode(textures[index].name, 0f, 0f, 1f, 1f, textures[index]));
    }

    private static void CmdMove(string[] args)
    {
        var sheet = MainWindow.Instance.GetOpenSpriteSheet();

        var spriteNodes = sheet.SpriteNodes;

        var index = int.Parse(args[0], NumberStyles.Any);

        var x = float.Parse(args[1], NumberStyles.Any);
        var y = float.Parse(args[2], NumberStyles.Any);

        spriteNodes[index].X = x;
        spriteNodes[index].Y = y;
    }

    private static void CmdDelete(string[] args)
    {
        var sheet = MainWindow.Instance.GetOpenSpriteSheet();

        var spriteNodes = sheet.SpriteNodes;
    }
}
