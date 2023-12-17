using System;
using System.Collections.Generic;

namespace WinTail.Models
{
  public sealed class Subscription<TValue, TError> : IDisposable
  {
    private readonly ICollection<ISubscriber<TValue, TError>> _subscribers;
    private readonly ISubscriber<TValue, TError> _subscriber;

    public Subscription(
      ICollection<ISubscriber<TValue, TError>> subscribers,
      ISubscriber<TValue, TError> subscriber)
    {
      _subscribers = subscribers ?? throw new ArgumentNullException(nameof(subscribers));
      _subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
    }

    public void Dispose()
    {
      _subscribers.Remove(_subscriber);
    }
  }
}
