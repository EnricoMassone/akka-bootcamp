namespace WinTail.Models
{
  public interface ISubscriber<TValue, TError>
  {
    void OnError(TError errorInfo);
    void OnValue(TValue value);
  }
}
