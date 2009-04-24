// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
using System;
using System.IO;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftyPerforce
	{
		// Handles registration and events for add/delete files and projects.
		class AutoAddDelete
		{
			private OutputWindowPane m_outputPane;
			private DTE2 m_application;
			private ProjectItemsEvents m_projectEvents;
			private SolutionEvents m_solutionEvents;
			private Plugin m_plugin;

			public AutoAddDelete(DTE2 application, OutputWindowPane outputPane, Plugin plugin)
			{
				m_plugin = plugin;
				m_application = application;
				m_outputPane = outputPane;

				m_projectEvents = ((EnvDTE80.Events2)m_application.Events).ProjectItemsEvents;
				m_solutionEvents = ((EnvDTE80.Events2)m_application.Events).SolutionEvents;

				m_projectEvents.ItemAdded += new _dispProjectItemsEvents_ItemAddedEventHandler(OnItemAdded);
				m_projectEvents.ItemRemoved += new _dispProjectItemsEvents_ItemRemovedEventHandler(OnItemRemoved);
				m_solutionEvents.ProjectAdded += new _dispSolutionEvents_ProjectAddedEventHandler(OnProjectAdded);
				m_solutionEvents.ProjectRemoved += new _dispSolutionEvents_ProjectRemovedEventHandler(OnProjectRemoved);
			}

			public void OnItemAdded(ProjectItem item)
			{
				if(!((Config)m_plugin.Options).autoAdd)
					return;
				P4Operations.EditFile(m_outputPane, item.ContainingProject.FullName);

				for (int i = 0; i < item.FileCount; i++)
				{
					string name = item.get_FileNames((short)i);
					P4Operations.AddFile(m_outputPane, name);
				}
			}

			public void OnItemRemoved(ProjectItem item)
			{
				if(!((Config)m_plugin.Options).autoDelete)
					return;
					
				P4Operations.EditFile(m_outputPane, item.ContainingProject.FullName);

				for (int i = 0; i < item.FileCount; i++)
				{
					string name = item.get_FileNames((short)i);
					P4Operations.DeleteFile(m_outputPane, name);
				}
			}

			private void OnProjectAdded(Project project)
			{
				if(!((Config)m_plugin.Options).autoAdd)
					return;
				P4Operations.EditFile(m_outputPane, m_application.Solution.FullName);
				P4Operations.AddFile(m_outputPane, project.FullName);
				// TODO: [jt] We should if the operation is not a add new project but rather a add existing project
				//       step through all the project items and add them to perforce. Or maybe we want the user
				//       to do this herself?
			}

			private void OnProjectRemoved(Project project)
			{
				if(!((Config)m_plugin.Options).autoDelete)
					return;
					
				P4Operations.EditFile(m_outputPane, m_application.Solution.FullName);
				P4Operations.DeleteFile(m_outputPane, project.FullName);
				// TODO: [jt] Do we want to automatically delete the items from perforce here?
			}
		}
	}
}
