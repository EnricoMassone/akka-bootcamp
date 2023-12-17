using Akka.Actor;
using System;
using System.IO;
using WinTail.Messages;
using WinTail.Services;

namespace WinTail.Actors
{
  public sealed class TailActor :
    UntypedActor,
    IDisposable
  {
    private readonly IActorRef _reporterActor;
    private readonly Stream _fileStream;
    private readonly StreamReader _fileReader;
    private readonly FileObserver _fileObserver;

    public TailActor(
      string filePath,
      IActorRef reporterActor)
    {
      if (string.IsNullOrWhiteSpace(filePath))
      {
        throw new ArgumentException(
          "The file path cannot be null or white space",
          nameof(filePath)
        );
      }

      _reporterActor = reporterActor ?? throw new ArgumentNullException(nameof(reporterActor));

      var fileFullPath = Path.GetFullPath(filePath);

      _fileObserver = new FileObserver(fileFullPath, this.Self);
      _fileObserver.Start();

      _fileStream = File.Open(fileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      _fileReader = new StreamReader(_fileStream);

      this.Self.Tell(new ReadFileContent(fileFullPath));
    }

    protected override void OnReceive(object message)
    {
      if (message is FileWritten)
      {
        var fileContent = _fileReader.ReadToEnd();
        _reporterActor.Tell(fileContent);
      }
      else if (message is FileError fileError)
      {
        _reporterActor.Tell(
          $"Error occurred while tailing file '{fileError.FileFullPath}': {fileError.ErrorDetails}"
        );
      }
      else if (message is ReadFileContent)
      {
        var fileContent = _fileReader.ReadToEnd();
        _reporterActor.Tell(fileContent);
      }
    }

    public void Dispose()
    {
      // dispose subscription and publisher
      _fileObserver.Dispose();

      // dispose stream reader and stream
      _fileReader.Dispose();
      _fileStream.Dispose();
    }
  }
}
