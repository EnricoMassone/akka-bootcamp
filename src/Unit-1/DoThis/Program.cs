using Akka.Actor;
using System;

namespace WinTail
{
  public static class Program
  {
    private const string WriterActorName = "console-writer";
    private const string ReaderActorName = "console-reader";

    private static ActorSystem MyActorSystem;

    static void Main(string[] args)
    {
      MyActorSystem = ActorSystem.Create(nameof(MyActorSystem));

      PrintInstructions();

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

    private static void PrintInstructions()
    {
      Console.WriteLine("Write whatever you want into the console!");
      Console.Write("Some lines will appear as");
      Console.ForegroundColor = ConsoleColor.DarkRed;
      Console.Write(" red ");
      Console.ResetColor();
      Console.Write(" and others will appear as");
      Console.ForegroundColor = ConsoleColor.Green;
      Console.Write(" green! ");
      Console.ResetColor();
      Console.WriteLine();
      Console.WriteLine();
      Console.WriteLine("Type 'exit' to quit this application at any time.\n");
    }
  }
}
