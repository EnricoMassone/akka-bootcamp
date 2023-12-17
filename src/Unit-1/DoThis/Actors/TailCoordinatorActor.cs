using Akka.Actor;
using System;
using WinTail.Messages;

namespace WinTail.Actors
{
  public sealed class TailCoordinatorActor : UntypedActor
  {
    protected override void OnReceive(object message)
    {
      if (message is StartTailFile startTailFile)
      {
        var (filePath, reporterActor) = startTailFile;

        Context.ActorOf(
          Props.Create(() => new TailActor(filePath, reporterActor)),
          BuildTailActorName(filePath)
        );
      }

      static string BuildTailActorName(string filePath) => Uri.EscapeDataString($"{filePath} tail actor");
    }
  }
}
