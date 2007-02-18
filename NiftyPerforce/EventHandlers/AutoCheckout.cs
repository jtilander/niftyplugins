// Copyright (C) 2006-2007 Jim Tilander. See COPYING for and README for more details.
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
			private EnvDTE.TextEditorEvents m_editorEvents;
			 
			public AutoCheckout(DTE2 application, OutputWindowPane outputPane)
			{
				m_application = application;
				m_outputPane = outputPane;

				m_events = ((EnvDTE80.Events2)m_application.Events).get_TextDocumentKeyPressEvents(null);
				m_events.BeforeKeyPress += new _dispTextDocumentKeyPressEvents_BeforeKeyPressEventHandler(OnBeforeKeyPress);

				m_editorEvents = ((EnvDTE80.Events2)m_application.Events).get_TextEditorEvents(null);
				m_editorEvents.LineChanged += new _dispTextEditorEvents_LineChangedEventHandler(OnLineChanged);
			}

			void OnBeforeKeyPress(string Keypress, EnvDTE.TextSelection Selection, bool InStatementCompletion, ref bool CancelKeypress)
			{
				if (Singleton<Config>.Instance.autoCheckout && m_application.ActiveDocument.ReadOnly)
				    P4Operations.EditFile(m_outputPane, m_application.ActiveDocument.FullName);
			}
			
			// [jt] This handler checks for things like paste operations. In theory we should be able to remove the handler above, but
			// I can't get this one to fire reliably... Wonder how much these handlers will slow down the IDE?
			void OnLineChanged( TextPoint StartPoint, TextPoint EndPoint, int Hint )
			{
				if ((Hint & (int)vsTextChanged.vsTextChangedNewline) == 0 &&
					(Hint & (int)vsTextChanged.vsTextChangedMultiLine) == 0 &&
					(Hint & (int)vsTextChanged.vsTextChangedNewline) == 0 &&
					(Hint != 0) )
					return;
				if (Singleton<Config>.Instance.autoCheckout && m_application.ActiveDocument.ReadOnly)
					P4Operations.EditFile(m_outputPane, m_application.ActiveDocument.FullName);
			}
			
		}
	}
}
