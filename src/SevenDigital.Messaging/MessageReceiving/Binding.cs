using System;
using System.Collections.Generic;

namespace SevenDigital.Messaging
{
	/// <summary>
	/// Binding helper for receiver configuration
	/// </summary>
	public class Binding:IMessageBinding
	{
		readonly List<Tuple<Type, Type>> _bindings;

		/// <summary>
		/// Start a binding list
		/// </summary>
		public Binding()
		{
			_bindings = new List<Tuple<Type,Type>>();
		}


		/// <summary>
		/// Handle a message type. Must complete With&lt;&gt;() to bind to a handler.
		/// </summary>
		public IHandlerBinding<TMessage> Handle<TMessage>() where TMessage : IMessage
		{
			if (!typeof(TMessage).IsInterface) throw new ArgumentException("Message type must be an interface that implements IMessage");
			return new Inner<TMessage>(this);
		}

		void Add(Type messageType, Type handlerType)
		{
			_bindings.Add(new Tuple<Type,Type>(messageType, handlerType));
		}

		/// <summary>
		/// Return all completed bindings
		/// </summary>
		public IEnumerable<Tuple<Type,Type>> AllBindings()
		{
			return _bindings;
		}

		/// <summary>
		/// Inner binding helper
		/// </summary>
		public class Inner<TMessage> : IHandlerBinding<TMessage> where TMessage : IMessage
		{
			readonly Binding _src;

			/// <summary>
			/// Inner binding helper
			/// </summary>
			public Inner(Binding src)
			{
				_src = src;
			}

			/// <summary>
			/// Bind a handler to the selected message
			/// </summary>
			public IMessageBinding With<THandler>() where THandler : IHandle<TMessage>
			{
				_src.Add(typeof(TMessage), typeof(THandler));
				return _src;
			}
		}
	}
}