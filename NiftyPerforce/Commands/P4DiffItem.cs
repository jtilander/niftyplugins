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
		class P4DiffItem : ItemCommandBase
		{
			public P4DiffItem(Plugin plugin, string canonicalName)
				: base("DiffItem", canonicalName, plugin, "Opens diff on an item", true, true)
			{
			}

			override public int IconIndex { get { return 3; } }

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
                P4Operations.DiffFile(pane, fileName);
            }
		}
	}
}
