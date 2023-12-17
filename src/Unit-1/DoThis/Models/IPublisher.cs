using System;

namespace WinTail.Models
{
  public interface IPublisher<TValue, TError>
  {
    IDisposable Subscribe(ISubscriber<TValue, TError> subscriber);
  }
}
