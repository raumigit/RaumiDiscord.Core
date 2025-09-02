namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.EventHandlers
{
    public class GenericEventHandler<T> : EventArgs
    {
        public GenericEventHandler(T arg)
        {
            Argument = arg;
        }

        public T? Argument { get; private set; }
    }
}
