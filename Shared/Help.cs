using System;
using System.IO;

namespace Aurora
{
	public static class Help
	{
		public static string FindFileInPath(string filename)
		{
			string pathenv = Environment.GetEnvironmentVariable("PATH");
			string[] items = pathenv.Split(';');

			foreach(string item in items)
			{
				string candidate = Path.Combine(item, filename);
				if(System.IO.File.Exists(candidate))
					return candidate;
			}
			
			return null;
		}
	}
}
