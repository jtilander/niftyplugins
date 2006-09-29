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
		class QuickOpen
		{
			private DTE2 m_application;
			private OpenDialog m_openDialog;

			public QuickOpen(DTE2 application)
			{
				m_application = application;
			}

			public void OnCommand()
			{
				if (null == m_openDialog)
					m_openDialog = new OpenDialog(m_application);
				m_openDialog.ShowDialog();

				foreach (string name in m_openDialog.filesToOpen)
					m_application.DTE.ExecuteCommand("File.OpenFile", name);

				m_openDialog.filesToOpen.Clear();
			}
		}
	}
}
