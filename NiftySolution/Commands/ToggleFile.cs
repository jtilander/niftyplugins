// Copyright (C) 2006-2008 Jim Tilander. See COPYING for and README for more details.
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

			public ToggleFile()
			{
				m_knownExtensions = new Dictionary<string, string[]>();
				m_knownExtensions.Add(".h", new string[] { ".cpp", ".c", ".inl", ".cxx" });
				m_knownExtensions.Add(".c", new string[] { ".h" });
				m_knownExtensions.Add(".cpp", new string[] { ".h", ".hxx", ".hpp" });
			}

			public override void OnCommand(DTE2 application, OutputWindowPane pane)
			{
				if (null == application.DTE.ActiveDocument)
					return;

				string fullPath = application.DTE.ActiveDocument.FullName;
				string extension = Path.GetExtension(fullPath);
				string filename = Path.Combine(Path.GetDirectoryName(fullPath), Path.GetFileNameWithoutExtension(fullPath));

				try
				{
					string[] candidates = m_knownExtensions[extension];
					foreach (string candidate in candidates)
					{
						string candidatePath = filename + candidate;
						if (File.Exists(candidatePath))
						{
							application.DTE.ExecuteCommand("File.OpenFile", candidatePath);
							break;
						}
					}
				}
				catch (KeyNotFoundException)
				{
				}
			}

			public override bool IsEnabled(DTE2 application)
			{
				return true;
			}
		}
	}
}
