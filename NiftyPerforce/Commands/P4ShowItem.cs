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
		class P4ShowItem : ItemCommandBase
		{
			public P4ShowItem(Plugin plugin, string canonicalName)
				: base("ShowItem", canonicalName, plugin, "Shows the item in p4win", true, true)
			{
			}

			public override int IconIndex { get { return 8; } }

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
				P4Operations.P4WinShowFile(pane, fileName);
			}
		}
	}
}
