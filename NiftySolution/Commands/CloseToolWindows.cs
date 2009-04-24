// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
using System;
using System.Collections.Generic;
using System.IO;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftySolution
	{
		// This command is useful for cleaning up all those pesky tool windows that take up all that space
		// if you have a small monitor. The alternatives are either to buy a 30" or to hide them all every
		// now and then.
		public class CloseToolWindow : CommandBase
		{
			override public int IconIndex { get { return 4; } }

			public CloseToolWindow(Plugin plugin)
				: base("CloseToolWindow", plugin, "Closes all active tool windows")
			{
			}

			override public void BindToKeyboard(Command vsCommand)
			{
				object[] bindings = new object[2];
				bindings[0] = "Global::ctrl+del";
				bindings[1] = "Text Editor::ctrl+del";
				vsCommand.Bindings = bindings;
			}

			public override bool OnCommand()
			{
				CloseToolWindows(Plugin.App);
				
				// Sometimes this really doesn't "bite", so we need to run through the list again.
				CloseToolWindows(Plugin.App);

				return true;
			}

			public override bool IsEnabled()
			{
				return true;
			}
			
			private void CloseToolWindows(DTE2 application)
			{
				foreach (Window w in application.Windows)
				{
					if (!w.Visible)
						continue;

					if (w.Kind == "Document")
						continue;

					w.Close(EnvDTE.vsSaveChanges.vsSaveChangesNo);
				}
			}
		}
	}
}
