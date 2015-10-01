// Copyright (C) 2006-2010 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.IO;
using Microsoft.VisualStudio.Shell;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class P4RevisionHistorySolution : CommandBase
		{
			public P4RevisionHistorySolution(Plugin plugin, string canonicalName)
				: base("RevisionHistorySolution", canonicalName, plugin, "Shows the revision history for the solution")
			{ 
			}

			override public int IconIndex { get { return 10; } }

            public override bool RegisterGUI(OleMenuCommand vsCommand, CommandBar vsCommandbar, bool toolBarOnly)
			{
				if(!toolBarOnly)
				{
					_RegisterGuiContext(vsCommand, "Solution");
				}
				return true;
			}

			public override bool OnCommand()
			{
				if(Plugin.App.Solution != null && Plugin.App.Solution.FullName != string.Empty)
				{
					P4Operations.RevisionHistoryFile(Plugin.OutputPane, Path.GetDirectoryName(Plugin.App.Solution.FullName), Plugin.App.Solution.FullName);
					return true;
				}
				return false;

			}

            public override bool IsEnabled()
            {
				return Plugin.App.Solution != null && Plugin.App.Solution.FullName != string.Empty;
            }
		}
	}
}
