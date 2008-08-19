// Copyright (C) 2006-2007 Jim Tilander. See COPYING for and README for more details.
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
			public CloseToolWindow()
			{
			}

			public override void OnCommand(DTE2 application, OutputWindowPane pane)
			{
				CloseToolWindows(application);
				
				// Sometimes this really doesn't "bite", so we need to run through the list again.
				CloseToolWindows(application);
			}

			public override bool IsEnabled(DTE2 application)
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
