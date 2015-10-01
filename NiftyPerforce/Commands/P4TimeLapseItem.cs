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
		class P4TimeLapseItem : ItemCommandBase
		{
			private bool mMainLine;
			
			public P4TimeLapseItem(Plugin plugin, string canonicalName, bool inMainLine)
				: base("TimeLapseItem", canonicalName, plugin, "Shows the Time Lapse View for an item" + (inMainLine ? " (mainline)" : ""), true, true)
			{
				mMainLine = inMainLine;
			}

			override public int IconIndex { get { return 7; } }

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
				
				if( mMainLine )
				{
					Config options = (Config)Plugin.Options;
					fileName = P4Operations.RemapToMain(fileName, options.MainLinePath);
				}
				
				P4Operations.TimeLapseView(pane, dirname, fileName);
			}
		}
	}
}
