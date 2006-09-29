using System;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class P4EditItem
		{
			public void OnCommand(DTE2 application, OutputWindowPane pane)
			{
				foreach (SelectedItem sel in application.SelectedItems)
				{
					if (null == sel.ProjectItem)
						continue;

					P4Operations.EditFile(pane, sel.ProjectItem.get_FileNames(0));
				}
			}
		}
	}
}
