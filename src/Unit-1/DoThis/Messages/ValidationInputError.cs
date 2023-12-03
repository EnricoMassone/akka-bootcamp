namespace WinTail.Messages
{
  public sealed record ValidationInputError : InputError
  {
    public ValidationInputError(string Reason) : base(Reason)
    {
    }
  }
}
