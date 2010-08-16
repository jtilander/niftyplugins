// Copyright (C) 2006-2010 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftySolution
	{
		// TODO: What we really should do here is to make the population of the opendialog in the background.
		//       We should subscribe to the open and close solution as well as the project events and then
		//       trigger rebuilding of the dialog...
		public class QuickOpen : CommandBase
		{
			private SolutionFiles mFiles = null;
			QuickOpenDialog mDialog = null;

			override public int IconIndex { get { return 1; } }

			public QuickOpen(Plugin plugin) 
				: base("QuickOpen", plugin, "Quickly opens any file in the solution")
			{
			}

			override public void BindToKeyboard(Command vsCommand)
			{
				vsCommand.Bindings = "Global::ctrl+o";
			}

			public override bool OnCommand()
			{
				if(null == mFiles)
				{
					Log.Info("First time fast open is run, scanning solution for files");
					mFiles = new SolutionFiles(Plugin);
					mFiles.Refresh();
				}
				
				if(null == mDialog)
					mDialog = new QuickOpenDialog(mFiles);
				
				if(mDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					string name = mDialog.FileToOpen;
					if(name.Length > 0 )
						Plugin.App.DTE.ExecuteCommand("File.OpenFile", string.Format("\"{0}\"", name));

					// TODO: Each time here we could save off the window position into the registry and 
					//       use it when we open the window the next time around.
				}

				return true;
			}

			public override bool IsEnabled()
			{
				return true;
			}
		}
	}
}
