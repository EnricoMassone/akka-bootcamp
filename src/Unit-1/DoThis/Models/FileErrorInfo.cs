using System;

namespace WinTail.Models
{
  public sealed record FileErrorInfo(string FullPath, Exception Exception);
}
