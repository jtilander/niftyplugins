// Copyright (C) 2006-2010 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class P4EditSolution : CommandBase
		{
			public P4EditSolution(Plugin plugin)
				: base("EditSolution", plugin, "Opens the solution for edit")
			{
			}

			public override int IconIndex { get { return 1; } }

			public override bool RegisterGUI(Command vsCommand, CommandBar vsCommandbar, bool toolBarOnly)
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
					P4Operations.EditFile(Plugin.OutputPane, Plugin.App.Solution.FullName);
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
