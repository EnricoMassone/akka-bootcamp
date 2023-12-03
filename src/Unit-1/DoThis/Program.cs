using Akka.Actor;

namespace WinTail
{
  public static class Program
  {
    private const string ActorSystemName = "my-actor-system";
    private const string WriterActorName = "console-writer";
    private const string ReaderActorName = "console-reader";

    private static ActorSystem? MyActorSystem;

    public static void Main()
    {
      MyActorSystem = ActorSystem.Create(ActorSystemName);

      var consoleWriterActor = MyActorSystem.ActorOf(
        Props.Create(() => new ConsoleWriterActor()),
        WriterActorName);

      var consoleReaderActor = MyActorSystem.ActorOf(
        Props.Create(() => new ConsoleReaderActor(consoleWriterActor)),
        ReaderActorName);

      consoleReaderActor.Tell(ConsoleReaderActor.StartReadingCommand);

      // blocks the main thread from exiting until the actor system is shut down
      MyActorSystem.WhenTerminated.Wait();
    }
  }
}
