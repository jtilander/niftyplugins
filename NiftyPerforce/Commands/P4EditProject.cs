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
					if (null == sel.Project)
						continue;

					P4Operations.EditFile(pane, sel.Project.FullName);
				}
			}
		}
	}
}
