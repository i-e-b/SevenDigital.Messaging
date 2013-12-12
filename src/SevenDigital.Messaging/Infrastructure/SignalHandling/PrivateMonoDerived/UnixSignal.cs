using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace SevenDigital.Messaging.Infrastructure.SignalHandling.PrivateMonoDerived
{
	static class UnsafeNativeMethods
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int Mono_Posix_RuntimeIsShuttingDown();

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromSignum")]
		public static extern int FromSignum(Signum value, out int rval);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToSignum")]
		public static extern int ToSignum(int value, out Signum rval);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromRealTimeSignum")]
		public static extern int FromRealTimeSignum(int offset, out int rval);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Unix_UnixSignal_install", SetLastError = true)]
		public static extern IntPtr install(int signum);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Unix_UnixSignal_uninstall")]
		public static extern int uninstall(IntPtr info);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Unix_UnixSignal_WaitAny")]
		public static extern int WaitAny(IntPtr[] infos, int count, int timeout, Mono_Posix_RuntimeIsShuttingDown shutting_down);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_SIGRTMIN")]
		public static extern int GetSIGRTMIN();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_SIGRTMAX")]
		public static extern int GetSIGRTMAX();
	}

	class UnixSignal : WaitHandle
	{


		public static bool TryToSignum(int value, out Signum rval)
		{
			return UnsafeNativeMethods.ToSignum(value, out rval) == 0;
		}
		public static Signum ToSignum(int value)
		{
			Signum result;
			if (UnsafeNativeMethods.ToSignum(value, out result) == -1)
			{
				throw new ArgumentException("value " + value + " is not an acceptable signum");
			}
			return result;
		}

		public static RealTimeSignum ToRealTimeSignum(int offset)
		{
			return new RealTimeSignum(offset);
		}
		public static bool TryFromSignum(Signum value, out int rval)
		{
			return UnsafeNativeMethods.FromSignum(value, out rval) == 0;
		}
		public static int FromSignum(Signum value)
		{
			int result;
			if (UnsafeNativeMethods.FromSignum(value, out result) == -1)
			{
				throw new ArgumentException("value " + value + " is not an acceptable signum");
			}
			return result;
		}
		public static int FromRealTimeSignum(RealTimeSignum sig)
		{
			int result;
			if (UnsafeNativeMethods.FromRealTimeSignum(sig.Offset, out result) == -1)
			{
				throw new ArgumentException("sig.Offset " + sig.Offset + " is not an acceptable offset");
			}
			return result;
		}

#pragma warning disable 169
		// ReSharper disable FieldCanBeMadeReadOnly.Local, MemberCanBePrivate.Local
		[StructLayout(LayoutKind.Sequential)]
		private struct SignalInfo
		{
			public int signum;
			public int count;
			public int read_fd;
			public int write_fd;
			public int have_handler;
			public int pipecnt;
			public IntPtr handler;
		}
		// ReSharper restore FieldCanBeMadeReadOnly.Local, MemberCanBePrivate.Local
#pragma warning restore 169

		private readonly int signum;
		private IntPtr signal_info;
		private static readonly UnsafeNativeMethods.Mono_Posix_RuntimeIsShuttingDown ShuttingDown = RuntimeShuttingDownCallback;
		public Signum Signum
		{
			get
			{
				if (IsRealTimeSignal)
				{
					throw new InvalidOperationException("This signal is a RealTimeSignum");
				}
				return ToSignum(signum);
			}
		}
		public RealTimeSignum RealTimeSignum
		{
			get
			{
				if (!IsRealTimeSignal)
				{
					throw new InvalidOperationException("This signal is not a RealTimeSignum");
				}
				return ToRealTimeSignum(signum - UnsafeNativeMethods.GetSIGRTMIN());
			}
		}
		public bool IsRealTimeSignal
		{
			get
			{
				AssertValid();
				int sIGRTMIN = UnsafeNativeMethods.GetSIGRTMIN();
				return sIGRTMIN != -1 && signum >= sIGRTMIN;
			}
		}
		private unsafe SignalInfo* Info
		{
			get
			{
				AssertValid();
				return (SignalInfo*)((void*)signal_info);
			}
		}
		public bool IsSet
		{
			get
			{
				return Count > 0;
			}
		}
		public unsafe int Count
		{
			get
			{
				return Info->count;
			}
			set
			{
				Interlocked.Exchange(ref Info->count, value);
			}
		}
		public UnixSignal(Signum signum)
		{
			this.signum = FromSignum(signum);
			signal_info = UnsafeNativeMethods.install(this.signum);
			if (signal_info == IntPtr.Zero)
			{
				throw new ArgumentException("Unable to handle signal", "signum");
			}
		}
		private static int RuntimeShuttingDownCallback()
		{
			return (!Environment.HasShutdownStarted) ? 0 : 1;
		}
		private void AssertValid()
		{
			if (signal_info == IntPtr.Zero)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
		}
		public unsafe bool Reset()
		{
			int num = Interlocked.Exchange(ref Info->count, 0);
			return num != 0;
		}
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (signal_info == IntPtr.Zero)
			{
				return;
			}
			UnsafeNativeMethods.uninstall(signal_info);
			signal_info = IntPtr.Zero;
		}
		public override bool WaitOne()
		{
			return WaitOne(-1, false);
		}
		public override bool WaitOne(TimeSpan timeout, bool exitContext)
		{
			var num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			return WaitOne((int)num, exitContext);
		}
		public override bool WaitOne(int millisecondsTimeout, bool exitContext)
		{
			AssertValid();
			if (exitContext)
			{
				throw new InvalidOperationException("exitContext is not supported");
			}
			return WaitAny(new[]
			{
				this
			}, millisecondsTimeout) == 0;
		}
		public static int WaitAny(UnixSignal[] signals)
		{
			return WaitAny(signals, -1);
		}
		public static int WaitAny(UnixSignal[] signals, TimeSpan timeout)
		{
			var num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			return WaitAny(signals, (int)num);
		}
		public static int WaitAny(UnixSignal[] signals, int millisecondsTimeout)
		{
			if (signals == null)
			{
				throw new ArgumentNullException("signals");
			}
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			var array = new IntPtr[signals.Length];
			for (int i = 0; i < signals.Length; i++)
			{
				array[i] = signals[i].signal_info;
				if (array[i] == IntPtr.Zero)
				{
					throw new InvalidOperationException("Disposed UnixSignal");
				}
			}
			return UnsafeNativeMethods.WaitAny(array, array.Length, millisecondsTimeout, ShuttingDown);
		}
	}
}
