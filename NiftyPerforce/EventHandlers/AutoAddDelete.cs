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
			
			public AutoAddDelete(DTE2 application, OutputWindowPane outputPane)
			{
				m_application = application;
				m_outputPane = outputPane;

				m_projectEvents = ((EnvDTE80.Events2)m_application.Events).ProjectItemsEvents;

				if (Singleton<Config>.Instance.autoAdd)
					m_projectEvents.ItemAdded += new _dispProjectItemsEvents_ItemAddedEventHandler(OnItemAdded);

				if (Singleton<Config>.Instance.autoDelete)
					m_projectEvents.ItemRemoved += new _dispProjectItemsEvents_ItemRemovedEventHandler(OnItemRemoved);

				m_solutionEvents = ((EnvDTE80.Events2)m_application.Events).SolutionEvents;
				if (Singleton<Config>.Instance.autoAdd)
					m_solutionEvents.ProjectAdded += new _dispSolutionEvents_ProjectAddedEventHandler(OnProjectAdded);
				
				if (Singleton<Config>.Instance.autoDelete)
					m_solutionEvents.ProjectRemoved += new _dispSolutionEvents_ProjectRemovedEventHandler(OnProjectRemoved);

				if (Singleton<Config>.Instance.autoAdd)
					m_outputPane.OutputString("> Registered auto add handler\n");

				if (Singleton<Config>.Instance.autoDelete)
					m_outputPane.OutputString("> Registered auto delete handler\n");
			}

			public void OnItemAdded(ProjectItem item)
			{
				P4EditItem.EditProject(item.ContainingProject,m_outputPane);

				for (int i = 0; i < item.FileCount; i++)
				{
					string name = item.get_FileNames((short)i);
					P4Operations.AddFile(m_outputPane, name);
				}
			}

			public void OnItemRemoved(ProjectItem item)
			{
				P4EditItem.EditProject(item.ContainingProject,m_outputPane);

				for (int i = 0; i < item.FileCount; i++)
				{
					string name = item.get_FileNames((short)i);
					P4Operations.DeleteFile(m_outputPane, name);
				}
			}

			private void OnProjectAdded(Project project)
			{
				P4EditSolution.EditSolution(m_application.Solution, m_outputPane);
				P4Operations.AddFile(m_outputPane, project.FullName);
				// TODO: [jt] We should if the operation is not a add new project but rather a add existing project
				//       step through all the project items and add them to perforce. Or maybe we want the user
				//       to do this herself?
			}
			
			private void OnProjectRemoved(Project project)
			{
				P4EditSolution.EditSolution(m_application.Solution, m_outputPane);
				P4Operations.DeleteFile(m_outputPane, project.FullName);
				// TODO: [jt] Do we want to automatically delete the items from perforce here?
			}
		}
	}
}
