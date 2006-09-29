using System;
using System.Collections.Generic;
using System.IO;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftySolution
	{
		class ToggleFile
		{
			private DTE2 m_application;
			private Dictionary<string, string[]> m_knownExtensions;

			public ToggleFile(DTE2 application)
			{
				m_application = application;
				m_knownExtensions = new Dictionary<string, string[]>();
				m_knownExtensions.Add(".h", new string[] { ".cpp", ".c", ".inl", ".cxx" });
				m_knownExtensions.Add(".c", new string[] { ".h" });
				m_knownExtensions.Add(".cpp", new string[] { ".h", ".hxx", ".hpp" });
			}

			public void OnCommand()
			{
				if (null == m_application.DTE.ActiveDocument)
					return;

				string fullPath = m_application.DTE.ActiveDocument.FullName;
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
							m_application.DTE.ExecuteCommand("File.OpenFile", candidatePath);
							break;
						}
					}
				}
				catch (KeyNotFoundException)
				{
				}
			}
		}
	}
}
