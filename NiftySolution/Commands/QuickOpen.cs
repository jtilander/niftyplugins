// Copyright (C) 2006-2008 Jim Tilander. See COPYING for and README for more details.
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

			public QuickOpen()
			{
			}

			public override void OnCommand(DTE2 application, OutputWindowPane pane)
			{
				if(null == mFiles)
				{
					Log.Info("First time fast open is run, scanning solution for files");
					mFiles = new SolutionFiles(application);
					mFiles.Refresh();
				}
				
				if(null == mDialog)
					mDialog = new QuickOpenDialog(mFiles);
				
				if(mDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					string name = mDialog.FileToOpen;
					if(name.Length > 0 )
						application.DTE.ExecuteCommand("File.OpenFile", name);


					// TODO: Each time here we could save off the window position into the registry and 
					//       use it when we open the window the next time around.
				}
			}

			public override bool IsEnabled(DTE2 application)
			{
				return true;
			}
		}
	}
}
