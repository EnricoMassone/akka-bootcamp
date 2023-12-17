﻿using Akka.Actor;
using WinTail.Messages;

namespace WinTail.Actors
{
  public sealed class TailCoordinatorActor : UntypedActor
  {
    protected override void OnReceive(object message)
    {
      if (message is StartTailFile startTailFile)
      {
        var (filePath, fileEventsProcessorActor) = startTailFile;

        Context.ActorOf(
          Props.Create(() => new TailActor(filePath, fileEventsProcessorActor)),
          BuildTailActorName(filePath)
        );
      }

      static string BuildTailActorName(string filePath) => $"{filePath} tail actor";
    }
  }
}