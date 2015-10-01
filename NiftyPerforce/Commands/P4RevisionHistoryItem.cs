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
		class P4RevisionHistoryItem : ItemCommandBase
		{
			private bool mMainLine;
				
			public P4RevisionHistoryItem(Plugin plugin, string canonicalName, bool mainLine)
				: base("RevisionHistoryItem", canonicalName, plugin, "Shows the revision history for an item" + (mainLine ? " (mainline)" : ""), true, true)
			{
				mMainLine = mainLine;
			}

			override public int IconIndex { get { return 10; } }

            public override bool RegisterGUI(OleMenuCommand vsCommand, CommandBar vsCommandbar, bool toolBarOnly)
			{
				if(toolBarOnly)
				{
					_RegisterGUIBar(vsCommand, vsCommandbar);
				}
				else
				{
					_RegisterGuiContext(vsCommand, "Project");
					_RegisterGuiContext(vsCommand, "Item");
					_RegisterGuiContext(vsCommand, "Easy MDI Document Window");
				}
				return true;
			}

			public override void OnExecute(SelectedItem item, string fileName, OutputWindowPane pane)
			{
				string dirname = Path.GetDirectoryName(fileName);

				if (mMainLine)
				{
					Config options = (Config)Plugin.Options;
					fileName = P4Operations.RemapToMain(fileName, options.MainLinePath);
				}

				P4Operations.RevisionHistoryFile(pane, dirname, fileName);
			}
		}
	}
}
