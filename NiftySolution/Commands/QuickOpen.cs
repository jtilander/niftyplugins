// Copyright (C) 2006-2007 Jim Tilander. See COPYING for and README for more details.
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
			private SolutionFiles m_files;

			public QuickOpen()
			{
				m_files = null;
			}

			public override void OnCommand(DTE2 application, OutputWindowPane pane)
			{
				if(null == m_files)
				{
					Log.Info("First time fast open is run, scanning solution for files");
					m_files = new SolutionFiles(application);
					m_files.Refresh();
					Log.Info("Scanning done ({0} files)", m_files.Count);
				}

				QuickOpenDialog dialog = new QuickOpenDialog(m_files);
				if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					string name = dialog.FileToOpen;
					if(name.Length > 0 )
						application.DTE.ExecuteCommand("File.OpenFile", name);
				}
			}

			public override bool IsEnabled(DTE2 application)
			{
				return true;
			}
		}
	}
}
