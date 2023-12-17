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
    protected override void OnReceive(object message)
    {
      switch (message)
      {
        case InputSuccess inputSuccess:
          PrintMessage(inputSuccess.Message + Environment.NewLine, ConsoleColor.Green);
          break;

        case InputError inputError:
          PrintMessage(inputError.Reason + Environment.NewLine, ConsoleColor.Red);
          break;

        default:
          PrintMessage(message?.ToString());
          break;
      }
    }

    private static void PrintMessage(
      string? message,
      ConsoleColor consoleColor = ConsoleColor.White)
    {
      Console.ForegroundColor = consoleColor;
      Console.Write(message);
      Console.ResetColor();
    }
  }
}
