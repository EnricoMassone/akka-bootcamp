using Akka.Actor;
using System;
using WinTail.Messages;

namespace WinTail
{
  /// <summary>
  /// Actor responsible for serializing message writes to the console.
  /// (write one message at a time, champ :)
  /// </summary>
  public sealed class ConsoleWriterActor : UntypedActor
  {
    protected override void OnReceive(object message)
    {
      switch (message)
      {
        case InputSuccess inputSuccess:
          PrintMessage(inputSuccess.Message, ConsoleColor.Green);
          break;

        case InputError inputError:
          PrintMessage(inputError.Reason, ConsoleColor.Red);
          break;

        default:
          PrintMessage(message?.ToString());
          break;
      }
    }

    private static void PrintMessage(string? message, ConsoleColor consoleColor = ConsoleColor.Black)
    {
      Console.ForegroundColor = consoleColor;
      Console.WriteLine(message);
      Console.WriteLine();
      Console.ResetColor();
    }
  }
}
