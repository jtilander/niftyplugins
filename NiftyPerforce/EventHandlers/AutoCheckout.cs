// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
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
			// File.SaveSelectedItems
			const string saveSelectedGuid = "{5EFC7975-14BC-11CF-9B2B-00AA00573819}";
			const int saveSelectedID = 0x14b;
			// File.SaveAll
			const string saveAllGuid = "{5EFC7975-14BC-11CF-9B2B-00AA00573819}";
			const int saveAllID = 0xe0;

			private OutputWindowPane m_outputPane;
			private DTE2 m_application;
			private EnvDTE.CommandEvents m_saveSelected;
			private EnvDTE.CommandEvents m_saveAll;

			private EnvDTE80.TextDocumentKeyPressEvents m_events;
			private EnvDTE.TextEditorEvents m_editorEvents;
			private Plugin m_plugin;

			private CommandEvents mEditSelectedProjectsEvent;

			public AutoCheckout(DTE2 application, OutputWindowPane outputPane, Plugin plugin)
			{
				m_plugin = plugin;
				m_application = application;
				m_outputPane = outputPane;

				m_saveSelected = m_application.Events.get_CommandEvents(saveSelectedGuid, saveSelectedID);
				m_saveSelected.BeforeExecute += new _dispCommandEvents_BeforeExecuteEventHandler(OnSaveSelected);

				m_saveAll = m_application.Events.get_CommandEvents(saveAllGuid, saveAllID);
				m_saveAll.BeforeExecute += new _dispCommandEvents_BeforeExecuteEventHandler(OnSaveAll);

				m_events = ((EnvDTE80.Events2)m_application.Events).get_TextDocumentKeyPressEvents(null);
				m_events.BeforeKeyPress += new _dispTextDocumentKeyPressEvents_BeforeKeyPressEventHandler(OnBeforeKeyPress);

				m_editorEvents = ((EnvDTE80.Events2)m_application.Events).get_TextEditorEvents(null);
				m_editorEvents.LineChanged += new _dispTextEditorEvents_LineChangedEventHandler(OnLineChanged);


				if(Singleton<Config>.Instance.autoCheckoutProject)
				{
					mEditSelectedProjectsEvent = m_plugin.FindCommandEvents("ClassViewContextMenus.ClassViewProject.Properties");

					if(null != mEditSelectedProjectsEvent)
						mEditSelectedProjectsEvent.BeforeExecute += OnEditSelectedProjectProperties;
				}
			}

			void OnSaveSelected(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
			{
				if(!((Config)m_plugin.Options).autoCheckoutOnSave)
					return;

				foreach (SelectedItem sel in m_application.SelectedItems)
				{
					if (sel.Project != null)
						tryEditFile(sel.Project.FullName);
					else if (sel.ProjectItem != null)
						tryEditFile(sel.ProjectItem.Document.FullName);
					else
						tryEditFile(m_application.Solution.FullName);
				}
			}

			void OnSaveAll(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
			{
				if(!((Config)m_plugin.Options).autoCheckoutOnSave)
					return;

				if (!m_application.Solution.Saved)
					tryEditFile(m_application.Solution.FullName);

				if (m_application.Solution.Projects == null)
					return;

				foreach (Project p in m_application.Solution.Projects)
					tryEditProjectRecursive(p);
			}

			void OnBuildSolution(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
			{
				foreach(Document doc in m_application.Documents)
				{
					if(!doc.Saved && doc.ReadOnly)
						P4Operations.EditFile(m_plugin.OutputPane, doc.FullName);
				}
			}

			void OnEditSelectedProjectProperties(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
			{
				foreach(Project project in (Array)m_application.ActiveSolutionProjects)
				{
					string path = project.FullName;
					tryEditFile(path);
				}
			}

			void tryEditFile(string sFileName)
			{
				if (0 != (System.IO.File.GetAttributes(sFileName) & FileAttributes.ReadOnly))
					P4Operations.EditFileImmediate(m_outputPane, sFileName);
			}

			void tryEditProjectRecursive(Project p)
			{
				if (!p.Saved)
					tryEditFile(p.FullName);

				if (p.ProjectItems == null)
					return;

				foreach (ProjectItem pi in p.ProjectItems)
				{
					if (pi.SubProject != null)
						tryEditProjectRecursive(pi.SubProject);
					else if (!pi.Saved)
						for (short i = 1; i <= pi.FileCount; i++)
							tryEditFile(pi.get_FileNames(i));
				}
			}

			void OnBeforeKeyPress(string Keypress, EnvDTE.TextSelection Selection, bool InStatementCompletion, ref bool CancelKeypress)
			{
				if(((Config)m_plugin.Options).autoCheckout && m_application.ActiveDocument.ReadOnly)
					P4Operations.EditFile(m_outputPane, m_application.ActiveDocument.FullName);
			}

			// [jt] This handler checks for things like paste operations. In theory we should be able to remove the handler above, but
			// I can't get this one to fire reliably... Wonder how much these handlers will slow down the IDE?
			void OnLineChanged(TextPoint StartPoint, TextPoint EndPoint, int Hint)
			{
				if ((Hint & (int)vsTextChanged.vsTextChangedNewline) == 0 &&
					(Hint & (int)vsTextChanged.vsTextChangedMultiLine) == 0 &&
					(Hint & (int)vsTextChanged.vsTextChangedNewline) == 0 &&
					(Hint != 0))
					return;
				if(((Config)m_plugin.Options).autoCheckout && m_application.ActiveDocument.ReadOnly)
					P4Operations.EditFile(m_outputPane, m_application.ActiveDocument.FullName);
			}

		}
	}
}
