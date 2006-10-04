using System;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class P4Edit
		{
			public void OnCommand(DTE2 application, OutputWindowPane pane)
			{
				// Check if we've selected stuff in the solution explorer and we currently have this as the active window.
				if ("Tool" == application.ActiveWindow.Kind && application.ActiveWindow.Caption.StartsWith("Solution Explorer"))
				{
					int checkoutItems = 0;
					foreach (SelectedItem sel in application.SelectedItems)
					{
						if (null != sel.ProjectItem)
						{
							if (P4EditItem.EditItem(sel.ProjectItem, pane))
								checkoutItems++;
						}

						if (null != sel.Project)
						{
							if (P4EditItem.EditProject(sel.Project, pane))
								checkoutItems++;
						}
					}

					if (checkoutItems > 0)
						return;
				}

				// Finally, let's just see if the text editor is active
				if ("Document" == application.ActiveWindow.Kind && application.ActiveDocument != null)
				{
					string fullName = application.ActiveDocument.FullName;
					
					if( !application.ActiveDocument.ReadOnly )
					{
						pane.OutputString( fullName + " is already opened for edit (is writeable on disk at least)\n" );
					}
					else
					{
						P4Operations.EditFile(pane, fullName);
					}
				}
			}
		}
	}
}
