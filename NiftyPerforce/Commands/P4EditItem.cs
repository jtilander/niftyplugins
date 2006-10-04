using System;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class P4EditItem
		{
			private const string m_fileItemGUID = "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}";

			public void OnCommand(DTE2 application, OutputWindowPane pane)
			{
				foreach (SelectedItem sel in application.SelectedItems)
				{
					EditItem(sel.ProjectItem, pane);
					EditProject(sel.Project, pane);
				}
			}

			public static bool EditItem(ProjectItem item, OutputWindowPane pane)
			{
				if (null == item)
					return false;

				if (m_fileItemGUID != item.Kind)
					return false;

				return P4Operations.EditFile(pane, item.get_FileNames(0));
			}

			public static bool EditProject(Project project, OutputWindowPane pane)
			{
				if (null == project)
					return false;
				return P4Operations.EditFile(pane, project.FullName);
			}
		}
	}
}
