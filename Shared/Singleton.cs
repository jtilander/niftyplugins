using System;

namespace Aurora
{
	public sealed class Singleton<T> where T : class, new()
	{
		private Singleton() {}
		public static readonly T Instance = new T();
	}
}
