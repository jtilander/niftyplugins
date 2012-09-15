// Copyright (C) 2006-2010 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using System.Threading;
using System.Collections.Generic;

namespace Aurora
{
	public static class AsyncProcess
	{
        private static int s_defaultTimeout = 30000; // in ms

        public delegate void OnDone(bool ok, object arg0);

        public static void Init()
		{
			m_helperThread = new System.Threading.Thread(new ThreadStart(ThreadMain));
			m_helperThread.Start();
		}

		public static void Term()
		{
			m_helperThread.Abort();
		}

        public static bool Run(OutputWindowPane output, string executable, string commandline, string workingdir, OnDone callback, object callbackArg)
		{
			int timeout = 1000;

            if (!RunCommand(output, executable, commandline, workingdir, timeout))
            {
                Log.Debug("Failed to run immediate (process hung?), trying again on a remote thread: " + commandline);
                return Schedule(output, executable, commandline, workingdir, callback, callbackArg);
            }
            else
            {
                if (null != callback)
                {
                    callback(true, callbackArg);
                }
            }

			return true;
		}

        public static bool Schedule(OutputWindowPane output, string executable, string commandline, string workingdir, OnDone callback, object callbackArg)
        {
            return Schedule(output, executable, commandline, workingdir, callback, callbackArg, s_defaultTimeout);
        }

        public static bool Schedule(OutputWindowPane output, string executable, string commandline, string workingdir, OnDone callback, object callbackArg, int timeout)
		{
			CommandThread cmd = new CommandThread();
			cmd.output = output;
			cmd.executable = executable;
			cmd.commandline = commandline;
			cmd.workingdir = workingdir;
            cmd.callback = callback;
            cmd.callbackArg = callbackArg;
            cmd.timeout = timeout;

			try
			{
				m_queueLock.WaitOne();
				m_commandQueue.Enqueue(cmd);
			}
			finally
			{
				m_queueLock.ReleaseMutex();
			}

			m_startEvent.Release();
			Log.Debug("Scheduled {0} {1}\n", cmd.executable, cmd.commandline);
			return true;
		}

		// ---------------------------------------------------------------------------------------------------------------------------------------------
		//
		// BEGIN INTERNALS
		//
		static private Mutex m_queueLock = new Mutex();
		static private Semaphore m_startEvent = new Semaphore(0, 9999);
		static private Queue<CommandThread> m_commandQueue = new Queue<CommandThread>();
		static private System.Threading.Thread m_helperThread;

		static private void ThreadMain()
		{
			while(true)
			{
				m_startEvent.WaitOne();
				CommandThread cmd = null;

				try
				{
					m_queueLock.WaitOne();
					cmd = m_commandQueue.Dequeue();
				}
				finally
				{
					m_queueLock.ReleaseMutex();
				}

				try
				{
					System.Threading.Thread thread = new System.Threading.Thread(new ThreadStart(cmd.Run));
					thread.Start();
				}
				catch
				{
				}
			}
		}


		private class CommandThread
		{
			public string executable = "";
			public string commandline = "";
			public string workingdir = "";
			public OutputWindowPane output = null;
            public OnDone callback = null;
            public object callbackArg = null;
            public int timeout = 10000;
			public void Run()
			{
                try
                {

                    bool ok = RunCommand(output, executable, commandline, workingdir, timeout);

                    if (null != callback)
                    {
                        callback(ok, callbackArg);
                    }
                }
                catch
                {
                    Log.Error("Caught unhandled exception in async process -- supressing so that we don't bring down Visual Studio");
                }
			}
		}

		static private bool RunCommand(OutputWindowPane output, string executable, string commandline, string workingdir, int timeout)
		{
			try
			{
				System.Diagnostics.Process process = new System.Diagnostics.Process();
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.FileName = executable;
				if( 0 == timeout )
				{
					// We are not for these processes reading the stdout and thus they could if they wrote more
					// data on the output line hang.
					process.StartInfo.RedirectStandardOutput = false;
					process.StartInfo.RedirectStandardError = false;
				}
				else
				{
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.RedirectStandardError = true;
				}
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.WorkingDirectory = workingdir;
				process.StartInfo.Arguments = commandline;

				Log.Debug("executableName : " + executable);
				Log.Debug("workingDirectory : " + workingdir);
				Log.Debug("command : " + commandline);

				if(!process.Start())
				{
					Log.Error("{0}: {1} Failed to start. Is Perforce installed and in the path?\n", executable, commandline);
					return false;
				}

                if (0 == timeout)
                {
                    // Fire and forget task.
                    return true;
                }
				
				bool exited = false;
				string alloutput = "";
				using (Process.Handler stderr = new Process.Handler(), stdout = new Process.Handler())
				{
					process.OutputDataReceived += stdout.OnOutput;
					process.BeginOutputReadLine();

					process.ErrorDataReceived += stderr.OnOutput;
					process.BeginErrorReadLine();

					exited = process.WaitForExit(timeout);

					/*
                     * This causes the plugin to unexpectedly crash, since it brings the entire thread down, and thus the entire environment?!?
                     * 
                    
                    if (0 != process.ExitCode)
					{
						throw new Process.Error("Failed to execute {0} {1}, exit code was {2}", executable, process.StartInfo.Arguments, process.ExitCode);
					}*/

					stderr.sentinel.WaitOne();
					stdout.sentinel.WaitOne();
					alloutput = stdout.buffer + "\n" + stderr.buffer;
				}
				
				if(!exited)
				{
					Log.Info("{0}: {1} timed out ({2} ms)", executable, commandline, timeout);
                    process.Kill();
					return false;
				}
				else
				{
					if(null != output)
					{
						output.OutputString(executable + ": " + commandline + "\n");
						output.OutputString(alloutput);
					}

					System.Diagnostics.Debug.WriteLine(commandline + "\n");
					System.Diagnostics.Debug.WriteLine(alloutput);

					if(0 != process.ExitCode)
					{
						Log.Debug("{0}: {1} exit code {2}", executable, commandline, process.ExitCode);
						return false;
					}
				}

				return true;
			}
			catch(System.ComponentModel.Win32Exception e)
			{
				Log.Error("{0}: {1} failed to spawn: {2}", executable, commandline, e.ToString());
				return false;
			}
		}

		//
		// END INTERNALS
		//
		// ---------------------------------------------------------------------------------------------------------------------------------------------
	}
}
