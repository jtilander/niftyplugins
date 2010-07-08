// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class P4EditItem : ItemCommandBase
		{
			public P4EditItem(Plugin plugin)
				: base("EditItem", plugin, "Opens an item for edit", true, true)
			{
			}

			public override int IconIndex { get { return 1; } }

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
					_RegisterGuiContext(vsCommand, "Cross Project Multi Item");
					_RegisterGuiContext(vsCommand, "Cross Project Multi Project");
				}
				return true;
			}

            public override void OnExecute(SelectedItem item, string fileName, OutputWindowPane pane)
            {
				P4Operations.EditFile(pane, fileName);
            }
		}
	}
}
