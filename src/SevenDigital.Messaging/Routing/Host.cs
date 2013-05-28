using System;

namespace SevenDigital.Messaging.Routing
{
	/// <summary>
	/// Container for a messaging host definition
	/// </summary>
	public class Host: IMessagingHost, IEquatable<Host>
	{
		readonly string _machineName;

		/// <summary>
		/// Define a host by it's machine name
		/// </summary>
		public Host(string machineName)
		{
			_machineName = machineName;
		}

		/// <summary>
		/// String representation of host
		/// </summary>
		public override string ToString()
		{
			return _machineName;
		}

		#region Equality members

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(Host other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other._machineName, _machineName);
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Host)) return false;
			return Equals((Host) obj);
		}

		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		public override int GetHashCode()
		{
			return (_machineName != null ? _machineName.GetHashCode() : 0);
		}

		#endregion
	}
}
