namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.EventHandlers
{

    /// <summary>
    /// GenericEventHandler is an abstract class that inherits from EventArgs and is used to create event handlers with a generic argument.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GenericEventHandler<T> : EventArgs
    {
        /// <summary>
        /// GenericEventHandler constructor that takes a generic argument of type T.
        /// </summary>
        /// <param name="arg"></param>
        public GenericEventHandler(T arg)
        {
            Argument = arg;
        }

        /// <summary>
        /// Argument of type T that is passed to the event handler.
        /// </summary>
        public T? Argument { get; private set; }
    }
}
