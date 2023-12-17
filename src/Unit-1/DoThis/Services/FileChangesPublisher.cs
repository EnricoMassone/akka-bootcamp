using System;
using System.Collections.Generic;
using System.IO;
using WinTail.Exceptions;
using WinTail.Models;

namespace WinTail.Services
{
  public sealed class FileChangesPublisher : IPublisher<FileChangeInfo, FileErrorInfo>, IDisposable
  {
    private readonly List<ISubscriber<FileChangeInfo, FileErrorInfo>> _subscribers;
    private readonly string _directoryName;
    private readonly string _fileName;
    private readonly string _fileFullPath;
    private readonly FileSystemWatcher _watcher;

    public FileChangesPublisher(string fileFullPath)
    {
      if (!IsValidFileFullPath(fileFullPath))
      {
        throw NewInvalidFileFullPathException(fileFullPath);
      }

      _subscribers = new List<ISubscriber<FileChangeInfo, FileErrorInfo>>();
      _fileFullPath = fileFullPath;
      _fileName = Path.GetFileName(fileFullPath);
      _directoryName = Path.GetDirectoryName(fileFullPath)!;
      _watcher = new FileSystemWatcher(_directoryName, _fileName);

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
    }

    private void OnFileChange(FileSystemEventArgs args)
    {
      if (args.ChangeType != WatcherChangeTypes.Changed)
      {
        return;
      }

      foreach (var subscriber in _subscribers)
      {
        subscriber.OnValue(new FileChangeInfo(args.FullPath));
      }
    }

    private void OnFileError(ErrorEventArgs args)
    {
      var exception = args.GetException();

      foreach (var subscriber in _subscribers)
      {
        subscriber.OnError(new FileErrorInfo(_fileFullPath, exception));
      }
    }

    public IDisposable Subscribe(ISubscriber<FileChangeInfo, FileErrorInfo> subscriber)
    {
      ArgumentNullException.ThrowIfNull(subscriber);

      if (!_subscribers.Contains(subscriber))
      {
        _subscribers.Add(subscriber);
      }

      return new Subscription<FileChangeInfo, FileErrorInfo>(_subscribers, subscriber);
    }

    public void Dispose() => _watcher.Dispose();
  }
}
