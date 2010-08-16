// Copyright (C) 2006-2010 Jim Tilander. See COPYING for and README for more details.
using System;
using System.Collections.Generic;
using System.IO;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftySolution
	{
		// This tries to quickly switch between the .h and the .cpp file pair
		// TODO: this could be done better, by searching through and making a list
		//       of all the pairs given all the files in the projects.	
		public class ToggleFile : CommandBase
		{
			private Dictionary<string, string[]> m_knownExtensions;

			override public int IconIndex { get { return 2; } }

			public ToggleFile(Plugin plugin)
				: base("ToggleFile", plugin, "Toggles between the header and the cpp file")
			{
				m_knownExtensions = new Dictionary<string, string[]>();
				m_knownExtensions.Add(".h", new string[] { ".inl", ".cpp", ".c", ".cxx", ".mm", ".m" });
				m_knownExtensions.Add(".c", new string[] { ".h" });
				m_knownExtensions.Add(".cpp", new string[] { ".h", ".hxx", ".hpp" });
				m_knownExtensions.Add(".mm", new string[] { ".h", ".hxx", ".hpp" });
				m_knownExtensions.Add(".m", new string[] { ".h", ".hxx", ".hpp" });
				m_knownExtensions.Add(".inl", new string[] { ".cpp", ".c", ".cxx", ".mm", ".m" });
			}

			override public void BindToKeyboard(Command vsCommand)
			{
				//object[] bindings = new object[1];
				//bindings[0] = "Global::Ctrl+G";
				//bindings[0] = "Text Editor::Ctrl+Return";
				//vsCommand.Bindings = bindings;
			}

			public override bool OnCommand()
			{
				if (null == Plugin.App.DTE.ActiveDocument)
					return false;

				string fullPath = Plugin.App.DTE.ActiveDocument.FullName;
				string extension = Path.GetExtension(fullPath);
				string filename = Path.Combine(Path.GetDirectoryName(fullPath), Path.GetFileNameWithoutExtension(fullPath));

				// TODO: This needs to cycle though the indices based on the current extension

				try
				{
					string[] candidates = m_knownExtensions[extension];
					foreach (string candidate in candidates)
					{
						string candidatePath = filename + candidate;
						if (System.IO.File.Exists(candidatePath))
						{
							Plugin.App.DTE.ExecuteCommand("File.OpenFile", candidatePath);
							break;
						}
					}
				}
				catch (KeyNotFoundException)
				{
					return false;
				}

				return true;
			}

			public override bool IsEnabled()
			{
				return true;
			}
		}
	}
}
