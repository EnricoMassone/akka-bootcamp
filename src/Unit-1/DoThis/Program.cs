﻿using Akka.Actor;
using WinTail.Actors;

namespace WinTail
{
  public static class Program
  {
    private const string ActorSystemName = "my-actor-system";
    private const string WriterActorName = "console-writer";
    private const string ValidationActorName = "validation-actor";
    private const string ReaderActorName = "console-reader";
    private const string TailCoordinatorActorName = "tail-coordinator-actor";

    private static ActorSystem? MyActorSystem;

    public static void Main()
    {
      MyActorSystem = ActorSystem.Create(ActorSystemName);

      var consoleWriterActor = MyActorSystem.ActorOf(
        Props.Create(() => new ConsoleWriterActor()),
        WriterActorName);

      var tailCoordinatorActor = MyActorSystem.ActorOf(
        Props.Create(() => new TailCoordinatorActor()),
        TailCoordinatorActorName
      );

      var validationActor = MyActorSystem.ActorOf(
        Props.Create(() => new FileValidationActor(consoleWriterActor, tailCoordinatorActor)),
        ValidationActorName
      );

      var consoleReaderActor = MyActorSystem.ActorOf(
        Props.Create(() => new ConsoleReaderActor(validationActor)),
        ReaderActorName);

      consoleReaderActor.Tell(ConsoleReaderActor.StartReadingCommand);

      // blocks the main thread from exiting until the actor system is shut down
      MyActorSystem.WhenTerminated.Wait();
    }
  }
}
