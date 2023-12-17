using System;

namespace WinTail.Exceptions
{
  public class InvalidFileFullPathException : Exception
  {
    public InvalidFileFullPathException()
    {
    }

    public InvalidFileFullPathException(
      string? message) : base(message)
    {
    }

    public InvalidFileFullPathException(
      string? message,
      Exception? innerException) : base(message, innerException)
    {
    }
  }
}
