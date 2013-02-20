using System;

namespace SevenDigital.Messaging.Unit.Tests._Helpers
{
	public static class Iam
	{
		 public static bool RunningMono()
		 {
             return Type.GetType ("Mono.Runtime") != null;
		 }
	}
}