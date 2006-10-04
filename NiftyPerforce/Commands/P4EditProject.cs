using System;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class P4EditProject
		{
			public void OnCommand(DTE2 application, OutputWindowPane pane)
			{
				foreach (SelectedItem sel in application.SelectedItems)
				{
					EditProject(sel.Project, pane);
				}
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
