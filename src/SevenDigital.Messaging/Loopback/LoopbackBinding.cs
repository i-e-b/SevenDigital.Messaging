using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SevenDigital.Messaging.Loopback
{
	/// <summary>
	/// Binding list for loopback mode.
	/// </summary>
	public class LoopbackBinding : ILoopbackBinding
	{
		readonly Dictionary<Type, ConcurrentBag<Type>> _bagOfHolding;

		/// <summary>
		/// Create a new binding container
		/// </summary>
		public LoopbackBinding()
		{
			_bagOfHolding = new Dictionary<Type, ConcurrentBag<Type>>();
		}

		/// <summary>
		/// List all handlers that have been registered on this node.
		/// </summary>
		public IEnumerable<Type> ForMessage<T>()
		{
			var key = typeof(T);
			return _bagOfHolding.ContainsKey(key) ? _bagOfHolding[key].ToArray() : new Type[0];
		}

		/// <summary>
		/// Return registered handlers for the exact message type
		/// </summary>
		public ConcurrentBag<Type> this[Type msg]
		{
			get { return _bagOfHolding[msg]; }
			set { _bagOfHolding[msg] = value; }
		}

		/// <summary>
		/// List all message types registered
		/// </summary>
		public IEnumerable<Type> MessagesRegistered { get 
		{ 
		return _bagOfHolding.Keys;
		}
		}

		/// <summary>
		/// Test if the exact message type has been registered
		/// </summary>
		public bool IsMessageRegistered(Type msg)
		{
			return _bagOfHolding.ContainsKey(msg);
		}

		/// <summary>
		/// Add a new message type with no handler bindings
		/// </summary>
		public void AddMessageType(Type msg)
		{
			_bagOfHolding.Add(msg, new ConcurrentBag<Type>());
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		public IEnumerator<KeyValuePair<Type, ConcurrentBag<Type>>> GetEnumerator()
		{
			return _bagOfHolding.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
