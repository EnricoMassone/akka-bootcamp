using Akka.Actor;
using System;

namespace WinTail.Messages
{
  public sealed class TailActor : UntypedActor
  {
    private readonly string _filePath;
    private readonly IActorRef _fileEventsProcessorActor;

    public TailActor(string filePath, IActorRef fileEventsProcessorActor)
    {
      if (string.IsNullOrWhiteSpace(filePath))
      {
        throw new ArgumentException("File path cannot be null or whitespace.", nameof(filePath));
      }

      _filePath = filePath;
      _fileEventsProcessorActor = fileEventsProcessorActor ?? throw new ArgumentNullException(nameof(fileEventsProcessorActor));
    }

    protected override void OnReceive(object message)
    {
      throw new NotImplementedException();
    }
  }
}
