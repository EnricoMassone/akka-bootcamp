using Akka.Actor;
using System;
using WinTail.Messages;

namespace WinTail.Actors
{
  public sealed class ValidationActor : UntypedActor
  {
    private readonly IActorRef _consoleWriterActor;

    public ValidationActor(IActorRef consoleWriterActor)
    {
      _consoleWriterActor = consoleWriterActor ?? throw new ArgumentNullException(nameof(consoleWriterActor));
    }

    protected override void OnReceive(object message)
    {
      if (message is not ConsoleInput consoleInput)
      {
        // simply ignore any unexpected message type
        return;
      }

      var userInput = consoleInput.Value;

      if (string.IsNullOrWhiteSpace(userInput))
      {
        _consoleWriterActor.Tell(new NullInputError());
        return;
      }

      if (IsValidUserInput(userInput))
      {
        _consoleWriterActor.Tell(new InputSuccess($"Valid input was provided: {userInput}"));
      }
      else
      {
        _consoleWriterActor.Tell(new ValidationInputError("Invalid input: the provided input contains an odd number of characters"));
      }

      Context.Sender.Tell(new ContinueProcessing());

      static bool IsValidUserInput(string input) => input.Length % 2 == 0;
    }
  }
}
