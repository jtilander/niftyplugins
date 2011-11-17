// Copyright (C) 2006-2010 Jim Tilander. See COPYING for and README for more details.
using System;
using System.Threading;

namespace Aurora
{
	public static class Process
	{
		public class Error : System.Exception
		{
			public string info;
			public Error(string info_, params object[] vaargs_) { info = string.Format(info_, vaargs_); }
		};

		// Helper class to capture output correctly and send an event once we've reached the end of the file.
		public class Handler : IDisposable
		{
			public string buffer;
			public ManualResetEvent sentinel;
			
			public Handler()
			{
				buffer = "";
				sentinel = new ManualResetEvent(false);
			}
			
			public void Dispose()
			{
				sentinel.Close();
			}
			
			public void OnOutput(object sender, System.Diagnostics.DataReceivedEventArgs e)
			{
				if( e.Data == null )
				{
					sentinel.Set();
				}
				else
				{
					buffer = buffer + e.Data + "\n";
				}
			}
		};
		
		public static string Execute(string executable, string workingdir, string arguments, params object[] vaargs)
		{
			using( System.Diagnostics.Process process = new System.Diagnostics.Process() )
			{
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.FileName = executable;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.WorkingDirectory = workingdir;
				process.StartInfo.Arguments = string.Format(arguments, vaargs);
				
				if(!process.Start())
				{
					throw new Error("{0}: Failed to start {1}.", executable, process.StartInfo.Arguments);
				}
				
				using( Handler stderr = new Handler(), stdout = new Handler() )
				{
					process.OutputDataReceived += stdout.OnOutput;
					process.BeginOutputReadLine();
					
					process.ErrorDataReceived += stderr.OnOutput;
					process.BeginErrorReadLine();
					
					process.WaitForExit();

					if(0 != process.ExitCode)
					{
						throw new Error("Failed to execute {0} {1}, exit code was {2}", executable, process.StartInfo.Arguments, process.ExitCode);
					}
					
					stderr.sentinel.WaitOne();
					stdout.sentinel.WaitOne();
					
					return stdout.buffer + "\n" + stderr.buffer;
				}
			}
		}
	}
}

