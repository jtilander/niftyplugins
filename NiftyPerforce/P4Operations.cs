// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Aurora
{
	namespace NiftyPerforce
	{
		// Simplification wrapper around running perforce commands.
		class P4Operations
		{
			private static bool g_p4installed = false;
			private static bool g_p4wininstalled = false;
			private static bool g_p4vinstalled = false;

			public static bool IntegrateFile(OutputWindowPane output, string filename, string oldName)
			{
				if(filename.Length == 0)
					return false;
				if(!g_p4installed)
					return NotifyUser("could not find p4 exe installed in perforce directory");
				return ScheduleRunCommand(output, "p4.exe", GetUserInfoString() + "integrate \"" + oldName + "\" \"" + filename + "\"", System.IO.Path.GetDirectoryName(filename));
			}

			public static bool DeleteFile(OutputWindowPane output, string filename)
			{
				if(filename.Length == 0)
					return false;
				if(!g_p4installed)
					return NotifyUser("could not find p4 exe installed in perforce directory");
				return ScheduleRunCommand(output, "p4.exe", GetUserInfoString() + "delete \"" + filename + "\"", System.IO.Path.GetDirectoryName(filename));
			}

			public static bool AddFile(OutputWindowPane output, string filename)
			{
				if(filename.Length == 0)
					return false;
				if(!g_p4installed)
					return NotifyUser("could not find p4 exe installed in perforce directory");
				return ScheduleRunCommand(output, "p4.exe", GetUserInfoString() + "add \"" + filename + "\"", System.IO.Path.GetDirectoryName(filename));
			}

			public static bool EditFile(OutputWindowPane output, string filename)
			{
				if(filename.Length == 0)
					return false;
				if(0 == (System.IO.File.GetAttributes(filename) & FileAttributes.ReadOnly))
					return false;
				if(!g_p4installed)
					return NotifyUser("could not find p4 exe installed in perforce directory");
				return ScheduleRunCommand(output, "p4.exe", GetUserInfoString() + "edit \"" + filename + "\"", System.IO.Path.GetDirectoryName(filename));
			}

			public static bool EditFileImmediate(OutputWindowPane output, string filename)
			{
				if(filename.Length == 0)
					return false;
				if(0 == (System.IO.File.GetAttributes(filename) & FileAttributes.ReadOnly))
					return false;
				if(!g_p4installed)
					return NotifyUser("could not find p4 exe installed in perforce directory");
				return RunCommand(output, "p4.exe", GetUserInfoString() + "edit \"" + filename + "\"", System.IO.Path.GetDirectoryName(filename), m_commandCount++);
			}

            public static bool RevertFile(OutputWindowPane output, string filename)
            {
				if(filename.Length == 0)
					return false;
				if(!g_p4installed)
					return NotifyUser("could not find p4 exe installed in perforce directory");
				return ScheduleRunCommand(output, "p4.exe", GetUserInfoString() + "revert \"" + filename + "\"", System.IO.Path.GetDirectoryName(filename));
            }

            public static bool DiffFile(OutputWindowPane output, string filename)
            {
				if(filename.Length == 0)
					return false;
				if(!g_p4wininstalled)
					return NotifyUser("could not find p4win exe installed in perforce directory");
				return ScheduleRunCommand(output, "p4win.exe", GetUserInfoString() + " -D \"" + filename + "\"", System.IO.Path.GetDirectoryName(filename));
            }

            public static bool RevisionHistoryFile(OutputWindowPane output, string filename)
            {
				if(filename.Length == 0)
					return false;
				if(!g_p4wininstalled)
					return NotifyUser("could not find p4win exe installed in perforce directory");
				return ScheduleRunCommand(output, "p4win.exe", GetUserInfoString() + " \"" + filename + "\"", System.IO.Path.GetDirectoryName(filename));
            }

			public static bool P4WinShowFile(OutputWindowPane output, string filename)
			{
				if(filename.Length == 0)
					return false;
				if(!g_p4wininstalled)
					return NotifyUser("could not find p4win exe installed in perforce directory");
				return ScheduleRunCommand(output, "p4win.exe", GetUserInfoString() + " -s \"" + filename + "\"", System.IO.Path.GetDirectoryName(filename));
			}

			private static string GetUserInfoString()
			{
				return GetUserInfoStringFull(false, "");
			}

			private static string GetUserInfoStringFull(bool lookup, string dir)
			{
				// NOTE: This to allow the user to have a P4CONFIG variable and connect to multiple perforce servers seamlessly.
				if( Singleton<Config>.Instance.useSystemEnv )
				{
					if(lookup)
					{
						try
						{
							string output = Process.Execute("p4", "-s -L \"{0}\" info", dir);
							Regex userpattern = new Regex(@"User name: (?<user>.*)$", RegexOptions.Compiled | RegexOptions.Multiline);
							Regex portpattern = new Regex(@"Server address: (?<port>.*)$", RegexOptions.Compiled | RegexOptions.Multiline);
							Regex clientpattern = new Regex(@"Client name: (?<client>.*)$", RegexOptions.Compiled | RegexOptions.Multiline);

							Match usermatch = userpattern.Match(output);
							Match portmatch = portpattern.Match(output);
							Match clientmatch = clientpattern.Match(output);

							string port = portmatch.Groups["port"].Value.Trim();
							string username = usermatch.Groups["user"].Value.Trim();
							string client = clientmatch.Groups["client"].Value.Trim();

							return string.Format( " -p {0} -u {1} -c {2} ", port, username, client);
						}
						catch(Process.Error e)
						{
							Log.Error( "Failed to execute info string discovery: {0}", e.info);
						}
					}

					return "";
				}
					
				string arguments = "";
				arguments += " -p " + Singleton<Config>.Instance.port;
				arguments += " -u " + Singleton<Config>.Instance.username;
				arguments += " -c " + Singleton<Config>.Instance.client;
				arguments += " ";
				return arguments;						
			}

			public static bool TimeLapseView(OutputWindowPane output, string filename)
			{
				if(!g_p4vinstalled)
					return NotifyUser("could not find p4v exe installed in perforce directory");
				// NOTE: The timelapse view uses the undocumented feature for bringing up the timelapse view. The username, client and port needs to be given in a certain order to work (straight from perforce).
				string arguments = " -win 0 ";
				arguments += GetUserInfoStringFull(true, Path.GetDirectoryName(filename));
				arguments += " -cmd \"annotate -i " + filename + "\"";
				return ScheduleRunCommand(output, "p4v.exe", arguments, System.IO.Path.GetDirectoryName(filename));
			}
            
            private static bool ScheduleRunCommand(OutputWindowPane output, string executableName, string command, string workingDirectory)
            {
				Command cmd = new Command();
				cmd.output = output;
				cmd.exe = executableName;
				cmd.arguments = command;
				cmd.workingDir = workingDirectory;
				cmd.sequence = m_commandCount++;
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
				output.OutputString(string.Format( "{0}: Scheduled {1} {2}\n", cmd.sequence, cmd.exe, cmd.arguments ) );
				return true;
            }
            
            public static bool RunCommand(OutputWindowPane output, string executableName, string command, string workingDirectory, int sequence)
			{
				System.Diagnostics.Process process = new System.Diagnostics.Process();
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.FileName = executableName;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.WorkingDirectory = workingDirectory;
				process.StartInfo.Arguments = command;
				if (!process.Start())
				{
					if (null != output)
					{
						output.OutputString(string.Format( "{0}: Failed to start {1}. Is Perforce installed and in the path?\n", sequence, executableName ));
					}
					return false;
				}
				process.WaitForExit();

				string stdOut = process.StandardOutput.ReadToEnd();
				string stdErr = process.StandardError.ReadToEnd();

				if (null != output)
				{
					output.OutputString(sequence.ToString() + ": " + executableName + " " + command + "\n");
					output.OutputString(stdOut);
					output.OutputString(stdErr);
				}

				System.Diagnostics.Debug.WriteLine(command + "\n");
				System.Diagnostics.Debug.WriteLine(stdOut);
				System.Diagnostics.Debug.WriteLine(stdErr);

				if (0 != process.ExitCode)
				{
					if (null != output)
					{
                        output.OutputString(sequence.ToString() + ": Process exit code was " + process.ExitCode + ".\n");
					}
					return false;
				}
				return true;
			}

			private class Command
			{
				public string exe = "";
				public string arguments = "";
				public string workingDir = "";
				public OutputWindowPane output = null;
				public int sequence = 0;
				
				
				public void Run()
				{
					P4Operations.RunCommand(output, exe, arguments, workingDir, sequence);
				}
			};
			
			static private Mutex m_queueLock = new Mutex();
			static private Semaphore m_startEvent = new Semaphore(0, 9999);
			static private Queue<Command> m_commandQueue = new Queue<Command>();
			static private System.Threading.Thread m_helperThread;
			static private int m_commandCount = 0;
			
			public static void InitThreadHelper()
			{
				m_helperThread = new System.Threading.Thread(new ThreadStart(ThreadMain));
				m_helperThread.Start();
			}

			public static void KillThreadHelper()
			{
				m_helperThread.Abort();
			}
			
			static public void ThreadMain()
			{
				while( true )
				{
					m_startEvent.WaitOne();
					Command cmd = null;
					
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
						System.Threading.Thread thread = new System.Threading.Thread( new ThreadStart( cmd.Run ) );
						thread.Start();
					}
					catch
					{
					}
				}
			}

			static public string ResolveFileNameWithCase(string fullpath)
			{
				string dirname = Path.GetDirectoryName(fullpath);
				string basename = Path.GetFileName(fullpath).ToLower();
				DirectoryInfo info = new DirectoryInfo(dirname);
				FileInfo[] files = info.GetFiles();

				foreach(FileInfo file in files)
				{
					if(file.Name.ToLower() == basename)
					{
						return file.FullName;
					}
				}
				
				// Should never happen...
				return fullpath;
			}

			public static void CheckInstalledFiles()
			{
				Log.Debug("Looking for installed files...");
				g_p4installed = false;
				g_p4wininstalled = false;
				g_p4vinstalled = false;

				// First find the perforce directory.
				Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.LocalMachine;
				hklm = hklm.OpenSubKey("SOFTWARE\\Perforce\\Environment12121");
				if(null == hklm)
				{
					Log.Error("Could not find any peforce installation in the registry!!!");
					return;
				}
				Object regValue = hklm.GetValue("P4INSTROOT");
				if(null == regValue)
				{
					Log.Error("Could not find any peforce installation in the registry!!!");
					return;
				}

				string installRoot = (string)regValue;

				Log.Info("Found perforce installation at {0}", installRoot);

				if(System.IO.File.Exists(System.IO.Path.Combine(installRoot, "p4.exe")))
				{
					g_p4installed = true;
					Log.Info("Found p4.exe");
				}
				if(System.IO.File.Exists(System.IO.Path.Combine(installRoot, "p4win.exe")))
				{
					g_p4wininstalled = true;
					Log.Info("Found p4win.exe");
				}
				if(System.IO.File.Exists(System.IO.Path.Combine(installRoot, "p4v.exe")))
				{
					g_p4vinstalled = true;
					Log.Info("Found p4v.exe");
				}
			}

			private static bool NotifyUser(string message)
			{
				System.Windows.Forms.MessageBox.Show(message, "NiftyPerforce Notice!", System.Windows.Forms.MessageBoxButtons.OK);
				return false;
			}
		}
	}

}
