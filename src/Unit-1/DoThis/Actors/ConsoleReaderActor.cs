using Akka.Actor;
using System;
using WinTail.Messages;

namespace WinTail.Actors
{
  /// <summary>
  /// Actor responsible for reading FROM the console.
  /// Also responsible for calling <see cref="ActorSystem.Terminate"/>.
  /// </summary>
  public sealed class ConsoleReaderActor : UntypedActor
  {
    public const string StartReadingCommand = "start";
    private const string ExitCommand = "exit";

    private readonly IActorRef _validationActor;

    public ConsoleReaderActor(IActorRef validationActor)
    {
      _validationActor = validationActor ?? throw new ArgumentNullException(nameof(validationActor));
    }

    protected override void OnReceive(object message)
    {
      if (message is ShutdownActorSystem)
      {
        Context.System.Terminate();
        return;
      }

      if (Equals(message, StartReadingCommand))
      {
        PrintInstructions();
      }

      ReadUserInputFromConsole();
    }

    private static void PrintInstructions()
    {
      Console.WriteLine("Please provide the URI to a log file on your local computer.");
      Console.WriteLine();
      Console.WriteLine();
      Console.WriteLine("Type 'exit' to quit this application at any time.");
      Console.WriteLine();
    }

    private void ReadUserInputFromConsole()
    {
      var userInput = Console.ReadLine();

      if (string.Equals(userInput, ExitCommand, StringComparison.OrdinalIgnoreCase))
      {
        this.Self.Tell(new ShutdownActorSystem());
        return;
      }

      _validationActor.Tell(new ConsoleInput(userInput));
    }
  }
}