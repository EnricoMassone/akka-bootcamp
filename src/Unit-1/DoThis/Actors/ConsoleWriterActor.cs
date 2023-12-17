using Akka.Actor;
using System;
using WinTail.Messages;

namespace WinTail.Actors
{
  /// <summary>
  /// Actor responsible for serializing message writes to the console.
  /// (write one message at a time, champ :)
  /// </summary>
  public sealed class ConsoleWriterActor : UntypedActor
  {
    private const bool AppendNewline = true;

    protected override void OnReceive(object message)
    {
      switch (message)
      {
        case InputSuccess inputSuccess:
          PrintMessage(inputSuccess.Message, ConsoleColor.Green, AppendNewline);
          break;

        case InputError inputError:
          PrintMessage(inputError.Reason, ConsoleColor.Red, AppendNewline);
          break;

        default:
          PrintMessage(message?.ToString());
          break;
      }
    }

    private static void PrintMessage(
      string? message,
      ConsoleColor consoleColor = ConsoleColor.White,
      bool appendNewline = false)
    {
      Console.ForegroundColor = consoleColor;

      if (appendNewline)
      {
        Console.WriteLine(message);
      }
      else
      {
        Console.Write(message);
      }

      Console.ResetColor();
    }
  }
}
