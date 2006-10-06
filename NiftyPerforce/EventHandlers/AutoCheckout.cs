using System;
using System.IO;
using Extensibility;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftyPerforce
	{
		// Simple wrapper around the automatic checkout feature
		// Feature inspired by code from Dave Etherton.
		class AutoCheckout
		{
			private OutputWindowPane m_outputPane;
			private DTE2 m_application;
			private EnvDTE80.TextDocumentKeyPressEvents m_events;
			
			public AutoCheckout(DTE2 application, OutputWindowPane outputPane)
			{
				m_application = application;
				m_outputPane = outputPane;

				if (!Singleton<Config>.Instance.autoCheckout)
					return;

				m_events = ((EnvDTE80.Events2)m_application.Events).get_TextDocumentKeyPressEvents(null);
				m_events.BeforeKeyPress += new _dispTextDocumentKeyPressEvents_BeforeKeyPressEventHandler(OnBeforeKeyPress);
				m_outputPane.OutputString("> Registered auto checkout handler\n");
			}

			void OnBeforeKeyPress(string Keypress, EnvDTE.TextSelection Selection, bool InStatementCompletion, ref bool CancelKeypress)
			{
				if (!m_application.ActiveDocument.ReadOnly)
					return;
				P4Operations.EditFile(m_outputPane, m_application.ActiveDocument.FullName);
			}
		}
	}
}
