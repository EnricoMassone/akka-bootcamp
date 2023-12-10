using Akka.Actor;
using System;
using System.IO;
using WinTail.Messages;

namespace WinTail.Actors
{
  public sealed class FileValidationActor : UntypedActor
  {
    private readonly IActorRef _consoleWriterActor;
    private readonly IActorRef _tailCoordinatorActor;

    public FileValidationActor(IActorRef consoleWriterActor, IActorRef tailCoordinatorActor)
    {
      _consoleWriterActor = consoleWriterActor ?? throw new ArgumentNullException(nameof(consoleWriterActor));
      _tailCoordinatorActor = tailCoordinatorActor ?? throw new ArgumentNullException(nameof(tailCoordinatorActor));
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
        Context.Sender.Tell(new ContinueProcessing());
        return;
      }

      if (IsValidFilePath(userInput))
      {
        _consoleWriterActor.Tell(new InputSuccess($"Start processing file {userInput}"));
        _tailCoordinatorActor.Tell(new StartTailFile(userInput, _consoleWriterActor));
      }
      else
      {
        _consoleWriterActor.Tell(
          new ValidationInputError($"Invalid input. {userInput} is not a valid file path.")
        );
        Context.Sender.Tell(new ContinueProcessing());
      }

      static bool IsValidFilePath(string filePath) => File.Exists(filePath);
    }
  }
}
