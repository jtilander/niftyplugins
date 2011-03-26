// Copyright (C) 2006-2010 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;


// How to call p4v from the command line: http://blog.perforce.com/blog/?p=1928
//
//

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
			private static bool g_p4customdiff = false;

            private static Dictionary<string, bool> g_opsInFlight = new Dictionary<string, bool>();

            private static bool LockOp(string token)
            {
                try
                {
                    lock (g_opsInFlight)
                    {
                        g_opsInFlight.Add(token, true);
                    }
                    Log.Debug("## Locked \"" + token + "\"" );
                    return true;
                }
                catch(ArgumentException)
                {
                    //Log.Debug("!! Failed to lock \"" + token + "\"");
                    Log.Error(token + " already in progress");
                    return false;
                }
            }

            private static void UnlockOp(bool ok, object token_)
            {
                string token = (string)token_;
                try
                {
                    lock (g_opsInFlight)
                    {
                        if (g_opsInFlight.Remove(token))
                        {
                            Log.Debug("## Unlocked \"" + token + "\"");
                        }
                        else
                        {
                            Log.Debug("!! Failed to unlock \"" + token + "\"");
                        }
                    }
                }
                catch (ArgumentNullException)
                {
                }
            }

            private static string FormatToken(string operation, string filename)
            {
                string token = operation + " " + Path.GetFullPath(filename).ToLower();
                return token;
            }

            public delegate bool CheckoutCallback(OutputWindowPane output, string filename);

			public static bool IntegrateFile(OutputWindowPane output, string filename, string oldName)
			{
				if(filename.Length == 0)
					return false;
				if(!g_p4installed)
					return NotifyUser("could not find p4 exe installed in perforce directory");

                string token = FormatToken("integrate", filename);
                if (!LockOp(token))
                    return false;

				return AsyncProcess.Schedule(output, "p4.exe", GetUserInfoString() + "integrate \"" + oldName + "\" \"" + filename + "\"", Path.GetDirectoryName(filename), new AsyncProcess.OnDone(UnlockOp), token);
			}

			public static bool DeleteFile(OutputWindowPane output, string filename)
			{
				if(filename.Length == 0)
					return false;
				if(!g_p4installed)
					return NotifyUser("could not find p4 exe installed in perforce directory");

                string token = FormatToken("delete", filename);
                if (!LockOp(token))
                    return false;
                return AsyncProcess.Schedule(output, "p4.exe", GetUserInfoString() + "delete \"" + filename + "\"", Path.GetDirectoryName(filename), new AsyncProcess.OnDone(UnlockOp), token);
			}

			public static bool AddFile(OutputWindowPane output, string filename)
			{
				if(filename.Length == 0)
					return false;
				if(!g_p4installed)
					return NotifyUser("could not find p4 exe installed in perforce directory");

                string token = FormatToken("add", filename);
                if (!LockOp(token))
                    return false;
                return AsyncProcess.Schedule(output, "p4.exe", GetUserInfoString() + "add \"" + filename + "\"", Path.GetDirectoryName(filename), new AsyncProcess.OnDone(UnlockOp), token);
			}

            public static bool EditFile(OutputWindowPane output, string filename)
            {
                return Internal_CheckEditFile(new CheckoutCallback(Internal_EditFile), output, filename);
            }

            public static bool EditFileImmediate(OutputWindowPane output, string filename)
            {
                return Internal_CheckEditFile(new CheckoutCallback(Internal_EditFileImmediate), output, filename);
            }

            private static bool Internal_CheckEditFile(CheckoutCallback callback, OutputWindowPane output, string filename)
            {
                bool result = callback(output, filename);

                string ext = Path.GetExtension(filename).ToLower();
                if (ext == ".vcxproj")
                {
                    callback(output, filename + ".filters");
                }

                if (ext == ".settings" || ext == ".resx")
                {
                    callback(output, Path.ChangeExtension(filename, ".Designer.cs"));
                }
                
                if (ext == ".cs")
                {
                    callback(output, Path.ChangeExtension(filename, ".Designer.cs"));
                    callback(output, Path.ChangeExtension(filename, ".resx"));
                }

                return result;
            }

            private static bool Internal_EditFile(OutputWindowPane output, string filename)
			{
				if(filename.Length == 0)
					return false;
                if (!System.IO.File.Exists(filename))
                    return false;
				if(0 == (System.IO.File.GetAttributes(filename) & FileAttributes.ReadOnly))
					return false;
				if(!g_p4installed)
					return NotifyUser("could not find p4 exe installed in perforce directory");

                Log.Debug("EditFile : " + filename);
                string token = FormatToken("edit", filename);
                if (!LockOp(token))
                    return false;
                return AsyncProcess.Schedule(output, "p4.exe", GetUserInfoString() + "edit \"" + filename + "\"", Path.GetDirectoryName(filename), new AsyncProcess.OnDone(UnlockOp), token);
			}

            private static bool Internal_EditFileImmediate(OutputWindowPane output, string filename)
			{
				if(filename.Length == 0)
					return false;
                if (!System.IO.File.Exists(filename))
                    return false;
                if (0 == (System.IO.File.GetAttributes(filename) & FileAttributes.ReadOnly))
					return false;
				if(!g_p4installed)
					return NotifyUser("could not find p4 exe installed in perforce directory");

                Log.Debug("EditFileImmediate : " + filename);
                string token = FormatToken("edit", filename);
                if (!LockOp(token))
                    return false;
                return AsyncProcess.Run(output, "p4.exe", GetUserInfoString() + "edit \"" + filename + "\"", Path.GetDirectoryName(filename), new AsyncProcess.OnDone(UnlockOp), token);
			}

			public static bool RevertFile(OutputWindowPane output, string filename)
			{
				if(filename.Length == 0)
					return false;
				if(!g_p4installed)
					return NotifyUser("could not find p4 exe installed in perforce directory");

                string token = FormatToken("revert", filename);
                if (!LockOp(token))
                    return false;
                return AsyncProcess.Schedule(output, "p4.exe", GetUserInfoString() + "revert \"" + filename + "\"", Path.GetDirectoryName(filename), new AsyncProcess.OnDone(UnlockOp), token);
			}

			public static bool DiffFile(OutputWindowPane output, string filename)
			{
				if(filename.Length == 0)
					return false;

                string token = FormatToken("diff", filename);
                if (!LockOp(token))
                    return false;

                if (g_p4wininstalled /*&& !Singleton<Config>.Instance.preferVisualClient*/)
                {
                    return AsyncProcess.Schedule(output, "p4win.exe", GetUserInfoString() + " -D \"" + filename + "#have\"", Path.GetDirectoryName(filename), new AsyncProcess.OnDone(UnlockOp), token);
                }

                // NOTE: this doesn't work since it leaves zombie p4v processes around!
                /*
                if (g_p4vinstalled)
                {
                    string arguments = " -win 0 ";
				    arguments += GetUserInfoStringFull(true, Path.GetDirectoryName(filename));
				    arguments += " -cmd \"prevdiff \"" + filename + "\"";
                    return AsyncProcess.Schedule(output, "p4v.exe", arguments, Path.GetDirectoryName(filename), new AsyncProcess.OnDone(UnlockOp), token);
                }*/

				if(g_p4installed)
				{
					// Let's figure out if the user has some custom diff tool installed. Then we just send whatever we have without any fancy options.
					if(g_p4customdiff)
					{
                        return AsyncProcess.Schedule(output, "p4.exe", GetUserInfoString() + " diff \"" + filename + "#have\"", Path.GetDirectoryName(filename), new AsyncProcess.OnDone(UnlockOp), token);
					}
					else
					{
						// Otherwise let's show a unified diff in the outputpane.
                        return AsyncProcess.Schedule(output, "p4.exe", GetUserInfoString() + " diff -du \"" + filename + "#have\"", Path.GetDirectoryName(filename), new AsyncProcess.OnDone(UnlockOp), token);
					}
				}
				return NotifyUser("could not find p4win.exe/p4.exe installed in perforce directory");
			}

			public static bool RevisionHistoryFile(OutputWindowPane output, string filename)
			{
				if(filename.Length == 0)
					return false;
                string token = FormatToken("history", filename);
                if (!LockOp(token))
                    return false;
                if (g_p4wininstalled && !Singleton<Config>.Instance.preferVisualClient)
                    return AsyncProcess.Schedule(output, "p4win.exe", GetUserInfoStringFull(true, Path.GetDirectoryName(filename)) + " \"" + filename + "\"", Path.GetDirectoryName(filename), new AsyncProcess.OnDone(UnlockOp), token, 0);
				if(g_p4vinstalled)
                    return AsyncProcess.Schedule(output, "p4v.exe", " -win 0 " + GetUserInfoStringFull(true, Path.GetDirectoryName(filename)) + " -cmd \"history " + filename + "\"", Path.GetDirectoryName(filename), new AsyncProcess.OnDone(UnlockOp), token, 0);
				return NotifyUser("could not find p4win.exe/p4v.exe installed in perforce directory");
			}

			public static bool P4WinShowFile(OutputWindowPane output, string filename)
			{
				if(filename.Length == 0)
					return false;
                if (g_p4wininstalled && !Singleton<Config>.Instance.preferVisualClient)
                {
                    return AsyncProcess.Schedule(output, "p4win.exe", GetUserInfoStringFull(true, Path.GetDirectoryName(filename)) + " -q -s \"" + filename + "\"", Path.GetDirectoryName(filename), null, null, 0);
                }
                if (g_p4vinstalled)
                {
                    return AsyncProcess.Schedule(output, "p4v.exe", " -win 0 " + GetUserInfoStringFull(true, Path.GetDirectoryName(filename)) + " -cmd \"open " + filename + "\"", Path.GetDirectoryName(filename), null, null, 0);
                }
                
                return NotifyUser("could not find p4win.exe or p4v.exe installed in perforce directory");
			}

			private static string GetUserInfoString()
			{
				return GetUserInfoStringFull(false, "");
			}

			private static string GetUserInfoStringFull(bool lookup, string dir)
			{
				// NOTE: This to allow the user to have a P4CONFIG variable and connect to multiple perforce servers seamlessly.
				if(Singleton<Config>.Instance.useSystemEnv)
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

							string ret = string.Format(" -p {0} -u {1} -c {2} ", port, username, client);

							Log.Debug("GetUserInfoStringFull : " + ret);

							return ret;
						}
						catch(Process.Error e)
						{
							Log.Error("Failed to execute info string discovery: {0}", e.info);
						}
					}

					return "";
				}

				string arguments = "";
				arguments += " -p " + Singleton<Config>.Instance.port;
				arguments += " -u " + Singleton<Config>.Instance.username;
				arguments += " -c " + Singleton<Config>.Instance.client;
				arguments += " ";

				Log.Debug("GetUserInfoStringFull : " + arguments);

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


                string token = FormatToken("timelapse", filename);
                if (!LockOp(token))
                    return false;
                return AsyncProcess.Schedule(output, "p4v.exe", arguments, Path.GetDirectoryName(filename), new AsyncProcess.OnDone(UnlockOp), token);
			}

            public static bool RevisionGraph(OutputWindowPane output, string filename)
            {
                if (!g_p4vinstalled)
                    return NotifyUser("could not find p4v exe installed in perforce directory");
                // NOTE: The timelapse view uses the undocumented feature for bringing up the timelapse view. The username, client and port needs to be given in a certain order to work (straight from perforce).
                string arguments = " -win 0 ";
                arguments += GetUserInfoStringFull(true, Path.GetDirectoryName(filename));
                arguments += " -cmd \"tree -i " + filename + "\"";


                string token = FormatToken("revisiongraph", filename);
                if (!LockOp(token))
                    return false;
                return AsyncProcess.Schedule(output, "p4v.exe", arguments, Path.GetDirectoryName(filename), new AsyncProcess.OnDone(UnlockOp), token);
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

			public static string GetRegistryValue(string key, string value, bool global)
			{
				Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.LocalMachine;
				if(!global)
					hklm = Microsoft.Win32.Registry.CurrentUser;
				hklm = hklm.OpenSubKey(key);
				if(null == hklm)
				{
					Log.Debug("Could not find registry key " + key);
					return null;
				}
				Object regValue = hklm.GetValue(value);
				if(null == regValue)
				{
					Log.Debug("Could not find registry value " + value + " in " + key);
					return null;
				}

				return (string)regValue;
			}

			public static void CheckInstalledFiles()
			{
				Log.Debug("Looking for installed files...");
				g_p4installed = false;
				g_p4wininstalled = false;
				g_p4vinstalled = false;
				g_p4customdiff = false;
				string p4diff = null;
				string installRoot = GetRegistryValue("SOFTWARE\\Perforce\\Environment", "P4INSTROOT", true); ;

                if (null == installRoot)
                {
                    // Perhaps it's an older installation?
                    // http://code.google.com/p/niftyplugins/issues/detail?id=47&can=1&q=path
                    installRoot = GetRegistryValue("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths", "p4.exe", true);
                }
				
				if(null != installRoot)
				{
					Log.Info("Found perforce installation at {0}", installRoot);

					if(System.IO.File.Exists(Path.Combine(installRoot, "p4.exe")))
					{
						g_p4installed = true;
						Log.Info("Found p4.exe");
					}
					if(System.IO.File.Exists(Path.Combine(installRoot, "p4win.exe")))
					{
						g_p4wininstalled = true;
						Log.Info("Found p4win.exe");
					}
					if(System.IO.File.Exists(Path.Combine(installRoot, "p4v.exe")))
					{
						g_p4vinstalled = true;
						Log.Info("Found p4v.exe");
					}

					p4diff = GetRegistryValue("SOFTWARE\\Perforce\\Environment", "P4DIFF", true);
					if(null != p4diff && p4diff.Length > 0)
					{
						Log.Info("Found p4 custom diff");
						g_p4customdiff = true;
					}
					p4diff = GetRegistryValue("SOFTWARE\\Perforce\\Environment", "P4DIFF", false);
					if(null != p4diff && p4diff.Length > 0)
					{
						Log.Info("Found p4 custom diff");
						g_p4customdiff = true;
					}
				}
				else
				{
					// Let's try to find the executables through the path variable instead.
					if(null != Help.FindFileInPath("p4.exe"))
					{
						g_p4installed = true;
						Log.Info("Found p4 in path");
					}

					if(null != Help.FindFileInPath("p4win.exe"))
					{
						g_p4wininstalled = true;
						Log.Info("Found p4win in path");
					}

					if(null != Help.FindFileInPath("p4v.exe"))
					{
						g_p4vinstalled = true;
						Log.Info("Found p4v in path");
					}

					Log.Warning("Could not find any peforce installation in the registry!!!");

					p4diff = System.Environment.GetEnvironmentVariable("P4DIFF");
					if(null != p4diff)
					{
						Log.Info("Found p4 custom diff");
						g_p4customdiff = true;
					}
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
