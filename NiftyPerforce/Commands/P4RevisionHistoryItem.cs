// Copyright (C) 2006-2008 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class P4RevisionHistoryItem : ItemCommandBase
		{
			public P4RevisionHistoryItem(Plugin plugin)
				: base("RevisionHistoryItem", plugin, "Shows the revision history for an item", true, true)
			{
			}

			override public int IconIndex { get { return 6; } }

			public override bool RegisterGUI(Command vsCommand, CommandBar vsCommandbar, bool toolBarOnly)
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
				P4Operations.RevisionHistoryFile(pane, fileName);
			}
		}
	}
}
