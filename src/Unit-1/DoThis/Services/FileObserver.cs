using Akka.Actor;
using System;
using System.IO;
using WinTail.Exceptions;
using WinTail.Messages;

namespace WinTail.Services
{
  public sealed class FileObserver : IDisposable
  {
    private readonly string _fileFullPath;
    private readonly FileSystemWatcher _watcher;
    private readonly IActorRef _tailActor;

    public FileObserver(string fileFullPath, IActorRef tailActor)
    {
      if (!IsValidFileFullPath(fileFullPath))
      {
        throw NewInvalidFileFullPathException(fileFullPath);
      }

      _tailActor = tailActor ?? throw new ArgumentNullException(nameof(tailActor));
      _fileFullPath = fileFullPath;
      _watcher = BuildFileSystemWatcher(
        fileFullPath,
        OnFileChange,
        OnFileError);

      static bool IsValidFileFullPath(string path)
      {
        if (string.IsNullOrWhiteSpace(path))
        {
          return false;
        }

        if (!Path.IsPathFullyQualified(path))
        {
          return false;
        }

        if (string.IsNullOrWhiteSpace(Path.GetFileName(path)))
        {
          return false;
        }

        return true;
      }

      static InvalidFileFullPathException NewInvalidFileFullPathException(string path) =>
        new($"'{path}' is not a valid file full path");

      static FileSystemWatcher BuildFileSystemWatcher(
        string fileFullPath,
        FileSystemEventHandler onFileChange,
        ErrorEventHandler onError)
      {
        var fileName = Path.GetFileName(fileFullPath);
        var directoryName = Path.GetDirectoryName(fileFullPath)!;

        var watcher = new FileSystemWatcher(directoryName, fileName)
        {
          NotifyFilter = NotifyFilters.LastWrite,
        };

        watcher.Changed += onFileChange;
        watcher.Error += onError;

        return watcher;
      }
    }

    public void Start()
    {
      _watcher.EnableRaisingEvents = true;
    }

    public void Stop()
    {
      _watcher.EnableRaisingEvents = false;
    }

    public void Dispose() => _watcher.Dispose();

    private void OnFileChange(object sender, FileSystemEventArgs args)
    {
      if (args.ChangeType != WatcherChangeTypes.Changed)
      {
        return;
      }

      _tailActor.Tell(new FileWritten(args.FullPath), ActorRefs.NoSender);
    }

    private void OnFileError(object sender, ErrorEventArgs args)
    {
      var exception = args.GetException();
      _tailActor.Tell(new FileError(_fileFullPath, exception.Message), ActorRefs.NoSender);
    }
  }
}
