using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging
{
	/// <summary>
	/// Standard endpoint for messaging
	/// </summary>
	public class Endpoint : IRoutingEndpoint
	{
		readonly string _name;

		/// <summary>
		/// Create an endpoint for a given name
		/// </summary>
		public Endpoint(string name)
		{
			_name = name;
		}

		/// <summary>
		/// Convert to string
		/// </summary>
		public override string ToString()
		{
			return _name;
		}

		/// <summary>
		/// Convert to string
		/// </summary>
		public static implicit operator Endpoint(string value)
		{
			return new Endpoint(value);
		}

		#region Equality members

#pragma warning disable 1591
		public bool Equals(Endpoint other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other._name, _name);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Endpoint)) return false;
			return Equals((Endpoint) obj);
		}

		public override int GetHashCode()
		{
			return (_name != null ? _name.GetHashCode() : 0);
		}
#pragma warning restore 1591

		#endregion
	}
}