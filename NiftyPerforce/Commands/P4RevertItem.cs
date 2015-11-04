// Copyright (C) 2006-2010 Jim Tilander. See COPYING for and README for more details.
using System;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using Microsoft.VisualStudio.Shell;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class P4RevertItem : ItemCommandBase
		{
			private bool mOnlyUnchanged;

			public P4RevertItem(Plugin plugin, string canonicalName, bool onlyUnchanged)
				: base("RevertItem", canonicalName, plugin, "Reverts an opened item" + (onlyUnchanged ? ", only if unchanged" : string.Empty), true, true)
			{
				mOnlyUnchanged = onlyUnchanged;
			}

			override public int IconIndex { get { return 4; } }

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
					_RegisterGuiContext(vsCommand, "Cross Project Multi Item");
					_RegisterGuiContext(vsCommand, "Cross Project Multi Project");
				}
				return true;
			}

			public override void OnExecute(SelectedItem item, string fileName, OutputWindowPane pane)
			{
				if (!mOnlyUnchanged)
				{
					string message = "You are about to revert the file '" + fileName + "'. Do you want to do this?";
					if(MessageBox.Show(message, "Revert File?", MessageBoxButtons.YesNo) != DialogResult.Yes)
					{
						return;
					}
				}
				P4Operations.RevertFile(pane, fileName, mOnlyUnchanged);
			}
		}
	}
}
