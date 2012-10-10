using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging
{
	public class Endpoint : IRoutingEndpoint
	{
		readonly string _name;

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

		public override string ToString()
		{
			return _name;
		}

		public Endpoint(string name)
		{
			_name = name;
		}

		public static implicit operator Endpoint(string value)
		{
			return new Endpoint(value);
		} 
	}
}