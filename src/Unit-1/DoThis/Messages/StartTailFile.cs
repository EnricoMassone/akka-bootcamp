using Akka.Actor;

namespace WinTail.Messages
{
  public sealed record StartTailFile(string FilePath, IActorRef FileEventsProcessor);
}
