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
				string fullName = application.ActiveDocument.FullName;
				P4Operations.EditFile(pane, fullName);
			}
		}
	}
}
