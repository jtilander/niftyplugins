// Copyright (C) 2006-2010 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using Microsoft.VisualStudio.Shell;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class P4DiffSolution : CommandBase
		{
			public P4DiffSolution(Plugin plugin, string canonicalName)
				: base("DiffSolution", canonicalName, plugin, "Opens the diff for the current solution")
			{ 
			}

			override public int IconIndex { get { return 3; } }

            public override bool RegisterGUI(OleMenuCommand vsCommand, CommandBar vsCommandbar, bool toolBarOnly)
            {
                if (!toolBarOnly)
				{
					_RegisterGuiContext(vsCommand, "Solution");
				}
				return true;
			}

			public override bool OnCommand()
			{
				if(Plugin.App.Solution != null && Plugin.App.Solution.FullName != string.Empty)
				{
					P4Operations.DiffFile(Plugin.OutputPane, Plugin.App.Solution.FullName);
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
