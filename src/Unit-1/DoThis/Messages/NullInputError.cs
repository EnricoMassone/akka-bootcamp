namespace WinTail.Messages
{
  public sealed record NullInputError : InputError
  {
    public NullInputError() : base("No input received. Please provide an input string.")
    {
    }
  }
}
