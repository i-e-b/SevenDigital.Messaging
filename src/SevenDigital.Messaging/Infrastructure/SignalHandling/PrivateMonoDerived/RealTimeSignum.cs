using System;

namespace SevenDigital.Messaging.Infrastructure.SignalHandling.PrivateMonoDerived
{
	struct RealTimeSignum : IEquatable<RealTimeSignum>
	{
		private readonly int rt_offset;
		private static readonly int MaxOffset = UnsafeNativeMethods.GetSIGRTMAX() - UnsafeNativeMethods.GetSIGRTMIN() - 1;
		public static readonly RealTimeSignum MinValue = new RealTimeSignum(0);
		public static readonly RealTimeSignum MaxValue = new RealTimeSignum(MaxOffset);
		public int Offset
		{
			get
			{
				return rt_offset;
			}
		}
		public RealTimeSignum(int offset)
		{
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Offset cannot be negative");
			}
			if (offset > MaxOffset)
			{
				throw new ArgumentOutOfRangeException("offset", "Offset greater than maximum supported SIGRT");
			}
			rt_offset = offset;
		}
		public override int GetHashCode()
		{
			return rt_offset.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			return obj != null && !(obj.GetType() != GetType()) && Equals((RealTimeSignum)obj);
		}
		public bool Equals(RealTimeSignum value)
		{
			return Offset == value.Offset;
		}
		public static bool operator ==(RealTimeSignum lhs, RealTimeSignum rhs)
		{
			return lhs.Equals(rhs);
		}
		public static bool operator !=(RealTimeSignum lhs, RealTimeSignum rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}
