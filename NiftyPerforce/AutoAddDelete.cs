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

			public AutoAddDelete(DTE2 application, OutputWindowPane outputPane)
			{
				m_application = application;
				m_outputPane = outputPane;

				ProjectItemsEvents projectEvents = ((EnvDTE80.Events2)m_application.Events).ProjectItemsEvents;

				if (Singleton<Config>.Instance.autoAdd)
					projectEvents.ItemAdded += new _dispProjectItemsEvents_ItemAddedEventHandler(OnItemAdded);

				if (Singleton<Config>.Instance.autoDelete)
					projectEvents.ItemRemoved += new _dispProjectItemsEvents_ItemRemovedEventHandler(OnItemRemoved);

				if (Singleton<Config>.Instance.autoAdd)
					m_outputPane.OutputString("> Registered auto add handler\n");

				if (Singleton<Config>.Instance.autoDelete)
					m_outputPane.OutputString("> Registered auto delete handler\n");
			}

			public void OnItemAdded(ProjectItem item)
			{
				CheckoutProject(item.ContainingProject);

				for (int i = 0; i < item.FileCount; i++)
				{
					string name = item.get_FileNames((short)i);
					P4Operations.AddFile(m_outputPane, name);
				}
			}

			public void OnItemRemoved(ProjectItem item)
			{
				CheckoutProject(item.ContainingProject);

				for (int i = 0; i < item.FileCount; i++)
				{
					string name = item.get_FileNames((short)i);
					P4Operations.DeleteFile(m_outputPane, name);
				}
			}

			private void CheckoutProject(Project project)
			{
				string fullName = project.FullName;

				System.IO.FileInfo info = new System.IO.FileInfo(fullName);
				if (!info.IsReadOnly)
					return;

				P4Operations.EditFile(m_outputPane, fullName);
			}
		}
	}
}
