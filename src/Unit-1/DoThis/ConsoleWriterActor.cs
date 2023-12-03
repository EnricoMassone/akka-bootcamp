using Akka.Actor;
using System;

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
      var text = message as string;

      // make sure we got a message
      if (string.IsNullOrWhiteSpace(text))
      {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("Please provide an input.\n");
        Console.ResetColor();
        return;
      }

      // if message has even # characters, display in red; else, green
      var isEven = text.Length % 2 == 0;
      var color = isEven ? ConsoleColor.Red : ConsoleColor.Green;
      var alert = isEven ?
        $"Your string had an even # of characters.{Environment.NewLine}" :
        $"Your string had an odd # of characters.{Environment.NewLine}";
      Console.ForegroundColor = color;
      Console.WriteLine(alert);
      Console.ResetColor();
    }
  }
}
