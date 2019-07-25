namespace [custom]
{
  /*
    This class abstracts the different kind of responses that are returned to the Views
    */
    public class CommandResult
    {
        private CommandResult() { }

        private CommandResult(object data)
        {
            this.Data = data;
        }

        private CommandResult(string failureReason)
        {
            this.FailureReason = failureReason;
        }

        public object Data { get; }
        public string FailureReason { get; }
        public bool IsSuccess => string.IsNullOrEmpty(FailureReason);

        public static CommandResult Success()
        {
            return new CommandResult();
        }

        public static CommandResult Success(object data)
        {
            return new CommandResult(data);
        }

        public static CommandResult Fail(string reason)
        {
            return new CommandResult(reason);
        }

        public static implicit operator bool(CommandResult result)
        {
            return result.IsSuccess;
        }
    }
}
