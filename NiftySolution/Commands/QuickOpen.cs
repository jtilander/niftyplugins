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
		class QuickOpen : CommandBase
		{
			private OpenDialog m_openDialog;

			public override void OnCommand(DTE2 application, OutputWindowPane pane)
			{
				if (null == m_openDialog)
					m_openDialog = new OpenDialog(application);
				m_openDialog.ShowDialog();

				foreach (string name in m_openDialog.filesToOpen)
					application.DTE.ExecuteCommand("File.OpenFile", name);

				m_openDialog.filesToOpen.Clear();
			}

			public override bool IsEnabled(DTE2 application)
			{
				return true;
			}
		}
	}
}
