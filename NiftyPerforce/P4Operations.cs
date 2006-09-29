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
				return RunCommand(output, "integrate \"" + oldName + "\" \"" + filename + "\"", System.IO.Path.GetDirectoryName(filename));
			}

			public static bool DeleteFile(OutputWindowPane output, string filename)
			{
				return RunCommand(output, "delete \"" + filename + "\"", System.IO.Path.GetDirectoryName(filename));
			}

			public static bool AddFile(OutputWindowPane output, string filename)
			{
				return RunCommand(output, "add \"" + filename + "\"", System.IO.Path.GetDirectoryName(filename));
			}

			public static bool EditFile(OutputWindowPane output, string filename)
			{
				return RunCommand(output, "edit \"" + filename + "\"", System.IO.Path.GetDirectoryName(filename));
			}

			private static bool RunCommand(OutputWindowPane output, string command, string workingDirectory)
			{
				System.Diagnostics.Process process = new System.Diagnostics.Process();
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.FileName = "p4.exe";
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.WorkingDirectory = workingDirectory;
				process.StartInfo.Arguments = command;
				if (!process.Start())
				{
					output.OutputString("Failed to start p4.exe. Is perforce installed and in the path?\n");
					return false;
				}
				process.WaitForExit();

				string stdOut = process.StandardOutput.ReadToEnd();
				string stdErr = process.StandardError.ReadToEnd();

				output.OutputString("> " + command + "\n");
				output.OutputString(stdOut);
				output.OutputString(stdErr);

				System.Diagnostics.Debug.WriteLine(command + "\n");
				System.Diagnostics.Debug.WriteLine(stdOut);
				System.Diagnostics.Debug.WriteLine(stdErr);

				if (0 != process.ExitCode)
				{
					output.OutputString("Process exitcode was " + process.ExitCode);
					return false;
				}
				return true;
			}
		}
	}

}
