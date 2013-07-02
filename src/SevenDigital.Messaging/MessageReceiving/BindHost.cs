using System;

namespace SevenDigital.Messaging.MessageReceiving
{
	/// <summary>
	/// A binding host that exists to wrestle with the .Net type system.
	/// </summary>
	/// <remarks>Don't look at the code, it's awful.</remarks>
	internal class BindHost : IMessageBinding
	{
		/// <summary>
		/// Bind a message type to a handler type
		/// </summary>
		/// <typeparam name="TMessage">Type of message to handle. This should be an interface that implements IMessage.</typeparam>
		/// <returns>A message binding, use this to specify the handler type</returns>
		IHandlerBinding<TMessage> IMessageBinding.Handle<TMessage>()
		{
			host = new InnerHost<TMessage>();
			return host as IHandlerBinding<TMessage>;
		}

		class InnerHost<TMessage> : IInnerHost, IHandlerBinding<TMessage>where TMessage : class, IMessage
		{
			Type boundHandlerType;
			public void With<THandler>() where THandler : IHandle<TMessage>
			{
				boundHandlerType = typeof(THandler);
				HasBinding = true;
			}

			public bool HasBinding { get; set; }
			public void Binding(out Type messageType, out Type handlerType)
			{
				messageType = typeof(TMessage);
				handlerType = boundHandlerType;
			}
		}

		interface IInnerHost
		{
			bool HasBinding { get; set; }
			void Binding(out Type messageType, out Type handlerType);
		}

		IInnerHost host;

		/// <summary>
		/// True if the host has a message-handler binding.
		/// </summary>
		public bool HasBinding { get { return host != null && host.HasBinding; }}

		/// <summary>
		/// Get the message-handler binding.
		/// </summary>
		public void Binding(out Type messageType, out Type handlerType)
		{
			host.Binding(out messageType, out handlerType);
		}
	}
}