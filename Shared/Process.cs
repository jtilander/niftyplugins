// Copyright (C) 2006-2010 Jim Tilander. See COPYING for and README for more details.
using System;

namespace Aurora
{
	public static class Process
	{
		public class Error : System.Exception
		{
			public string info;
			public Error(string info_, params object[] vaargs_) { info = string.Format(info_, vaargs_); }
		};

		public static string Execute(string executable, string arguments, params object[] vaargs)
		{
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.FileName = executable;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.Arguments = string.Format(arguments, vaargs);
			
			if(!process.Start())
			{
				throw new Error("{0}: Failed to start {1}.", executable, process.StartInfo.Arguments);
			}
			process.WaitForExit();

			string stdOut = process.StandardOutput.ReadToEnd();
			string stdErr = process.StandardError.ReadToEnd();

			if(0 != process.ExitCode)
			{
				throw new Error("Failed to execute {0} {1}, exit code was {2}", executable, process.StartInfo.Arguments, process.ExitCode);
			}
			return stdOut + "\n" + stdErr;
		}
	}
}

