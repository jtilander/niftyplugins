using System;
using EnvDTE;
using EnvDTE80;


namespace Aurora
{
	namespace NiftyPerforce
	{
		// This is our internal master class that holds everything. 
		// TODO: This class should really have a configuration that it reads, pop up an UI etc.
		class Plugin
		{
			private DTE2 m_application = null;
			private AddIn m_addin = null;
			private OutputWindowPane m_outputPane = null;
			private const string m_paneName = "NiftyPerforce";

			private AutoCheckout m_autoCheckout = null;
			private AutoAddDelete m_autoAddDelete = null;
			public Perforce m_perforce = null;

			public Plugin(DTE2 application, AddIn addin)
			{
				m_application = application;
				m_addin = addin;

				AquireOutputPane(m_application);

				m_autoCheckout = new AutoCheckout(m_application, m_outputPane);
				m_autoAddDelete = new AutoAddDelete(m_application, m_outputPane);
				m_perforce = new Perforce(application, addin, m_outputPane);
			}

			private void AquireOutputPane(DTE2 application)
			{
				OutputWindow outputWindow = (OutputWindow)application.Windows.Item(Constants.vsWindowKindOutput).Object;
				OutputWindowPanes panes = outputWindow.OutputWindowPanes;

				foreach (OutputWindowPane pane in panes)
				{
					if (m_paneName != pane.Name)
						continue;

					m_outputPane = pane;
					break;
				}

				if (null == m_outputPane)
					m_outputPane = panes.Add(m_paneName);
			}
		}
	}
}
