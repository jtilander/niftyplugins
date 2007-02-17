using System;
using EnvDTE;

namespace Aurora
{
	namespace NiftyPerforce
	{
		// Simplification wrapper around running perforce commands.
		class P4Operations
		{
			public static bool IntegrateFile(OutputWindowPane output, string filename, string oldName)
			{
                return RunCommand(output, "p4.exe", "integrate \"" + oldName + "\" \"" + filename + "\"", System.IO.Path.GetDirectoryName(filename));
			}

			public static bool DeleteFile(OutputWindowPane output, string filename)
			{
                return RunCommand(output, "p4.exe", "delete \"" + filename + "\"", System.IO.Path.GetDirectoryName(filename));
			}

			public static bool AddFile(OutputWindowPane output, string filename)
			{
                return RunCommand(output, "p4.exe", "add \"" + filename + "\"", System.IO.Path.GetDirectoryName(filename));
			}

			public static bool EditFile(OutputWindowPane output, string filename)
			{
                return RunCommand(output, "p4.exe", "edit \"" + filename + "\"", System.IO.Path.GetDirectoryName(filename));
			}

            public static bool RevertFile(OutputWindowPane output, string filename)
            {
                return RunCommand(output, "p4.exe", "revert \"" + filename + "\"", System.IO.Path.GetDirectoryName(filename));
            }

            public static bool DiffFile(OutputWindowPane output, string filename)
            {
                return RunCommand(output, "p4win.exe", "-D \"" + filename + "\"", System.IO.Path.GetDirectoryName(filename));
            }

            public static bool RevisionHistoryFile(OutputWindowPane output, string filename)
            {
                return RunCommand(output, "p4win.exe", " \"" + filename + "\"", System.IO.Path.GetDirectoryName(filename));
            }

            private static bool RunCommand(OutputWindowPane output, string executableName, string command, string workingDirectory)
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
						output.OutputString("Failed to start " + executableName + ". Is Perforce installed and in the path?\n");
					}
					return false;
				}
				process.WaitForExit();

				string stdOut = process.StandardOutput.ReadToEnd();
				string stdErr = process.StandardError.ReadToEnd();

				if (null != output)
				{
					output.OutputString("> " + executableName + " " + command + "\n");
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
                        output.OutputString("Process exit code was " + process.ExitCode + ".\n");
					}
					return false;
				}
				return true;
			}
		}
	}

}
