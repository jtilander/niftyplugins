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
		class AutoAddDelete : Feature
		{
			private ProjectItemsEvents m_projectEvents;
			private SolutionEvents m_solutionEvents;
			private Plugin m_plugin;

			public AutoAddDelete(Plugin plugin)
				: base("AutoAddDelete", "Automatically adds and deletes files matching project add/delete")
			{
				m_plugin = plugin;

				m_projectEvents = ((EnvDTE80.Events2)m_plugin.App.Events).ProjectItemsEvents;
				m_solutionEvents = ((EnvDTE80.Events2)m_plugin.App.Events).SolutionEvents;

				if(((Config)m_plugin.Options).autoAdd)
				{
					m_projectEvents.ItemAdded += new _dispProjectItemsEvents_ItemAddedEventHandler(OnItemAdded);
					m_solutionEvents.ProjectAdded += new _dispSolutionEvents_ProjectAddedEventHandler(OnProjectAdded);
				}

				if(((Config)m_plugin.Options).autoDelete)
				{
					m_projectEvents.ItemRemoved += new _dispProjectItemsEvents_ItemRemovedEventHandler(OnItemRemoved);
					m_solutionEvents.ProjectRemoved += new _dispSolutionEvents_ProjectRemovedEventHandler(OnProjectRemoved);
				}
			}

			public void OnItemAdded(ProjectItem item)
			{
				P4Operations.EditFile(m_plugin.OutputPane, item.ContainingProject.FullName);

				for (int i = 0; i < item.FileCount; i++)
				{
					string name = item.get_FileNames((short)i);
					P4Operations.AddFile(m_plugin.OutputPane, name);
				}
			}

			public void OnItemRemoved(ProjectItem item)
			{
				P4Operations.EditFile(m_plugin.OutputPane, item.ContainingProject.FullName);

				for (int i = 0; i < item.FileCount; i++)
				{
					string name = item.get_FileNames((short)i);
					P4Operations.DeleteFile(m_plugin.OutputPane, name);
				}
			}

			private void OnProjectAdded(Project project)
			{
				P4Operations.EditFile(m_plugin.OutputPane, m_plugin.App.Solution.FullName);
				P4Operations.AddFile(m_plugin.OutputPane, project.FullName);
				// TODO: [jt] We should if the operation is not a add new project but rather a add existing project
				//       step through all the project items and add them to perforce. Or maybe we want the user
				//       to do this herself?
			}

			private void OnProjectRemoved(Project project)
			{
				P4Operations.EditFile(m_plugin.OutputPane, m_plugin.App.Solution.FullName);
				P4Operations.DeleteFile(m_plugin.OutputPane, project.FullName);
				// TODO: [jt] Do we want to automatically delete the items from perforce here?
			}
		}
	}
}
