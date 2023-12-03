using Akka.Actor;
using System;
using WinTail.Messages;

namespace WinTail
{
  /// <summary>
  /// Actor responsible for reading FROM the console.
  /// Also responsible for calling <see cref="ActorSystem.Terminate"/>.
  /// </summary>
  public sealed class ConsoleReaderActor : UntypedActor
  {
    public const string StartReadingCommand = "start";
    private const string ExitCommand = "exit";

    private readonly IActorRef _consoleWriterActor;

    public ConsoleReaderActor(IActorRef consoleWriterActor)
    {
      _consoleWriterActor = consoleWriterActor ?? throw new ArgumentNullException(nameof(consoleWriterActor));
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
      else if (message is InputError inputError)
      {
        _consoleWriterActor.Tell(inputError);
      }
      else if (message is InputSuccess inputSuccess)
      {
        _consoleWriterActor.Tell(inputSuccess);
      }

      ReadUserInputFromConsole();
    }

    private static void PrintInstructions()
    {
      Console.WriteLine("Write whatever you want into the console!");
      Console.WriteLine("Some entries will pass validation and some won't...");
      Console.WriteLine();
      Console.WriteLine();
      Console.WriteLine("Type 'exit' to quit this application at any time.");
      Console.WriteLine();
    }

    private void ReadUserInputFromConsole()
    {
      var userInput = Console.ReadLine();

      if (string.IsNullOrWhiteSpace(userInput))
      {
        this.Self.Tell(new NullInputError());
        return;
      }

      if (string.Equals(userInput, ExitCommand, StringComparison.OrdinalIgnoreCase))
      {
        this.Self.Tell(new ShutdownActorSystem());
        return;
      }

      if (IsValidUserInput(userInput))
      {
        this.Self.Tell(new InputSuccess($"Valid input was provided: {userInput}"));
      }
      else
      {
        this.Self.Tell(new ValidationInputError("Invalid input: the provided input contains an odd number of characters"));
      }

      static bool IsValidUserInput(string input) => input.Length % 2 == 0;
    }
  }
}