using Akka.Actor;
using System;

namespace WinTail
{
  /// <summary>
  /// Actor responsible for reading FROM the console. 
  /// Also responsible for calling <see cref="ActorSystem.Terminate"/>.
  /// </summary>
  public sealed class ConsoleReaderActor : UntypedActor
  {
    public const string ExitCommand = "exit";
    public const string ContinueReadingCommand = "continue";
    public const string StartReadingCommand = "start";

    private readonly IActorRef _consoleWriterActor;

    public ConsoleReaderActor(IActorRef consoleWriterActor)
    {
      _consoleWriterActor = consoleWriterActor ?? throw new ArgumentNullException(nameof(consoleWriterActor));
    }

    protected override void OnReceive(object message)
    {
      var consoleInput = Console.ReadLine();

      if (string.Equals(consoleInput, ExitCommand, StringComparison.OrdinalIgnoreCase))
      {
        // shut down the system (acquire handle to system via
        // this actors context)
        Context.System.Terminate();
        return;
      }

      // send input to the console writer to process and print
      _consoleWriterActor.Tell(consoleInput);

      // continue reading messages from the console
      this.Self.Tell(ContinueReadingCommand);
    }
  }
}