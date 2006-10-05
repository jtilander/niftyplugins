using System;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class P4EditModified
		{
			public void OnCommand(DTE2 application, OutputWindowPane pane)
			{
				foreach (Document doc in application.Documents)
				{
					if (doc.Saved || !doc.ReadOnly)
						continue;
					P4Operations.EditFile(pane, doc.FullName);
				}
			}
		}
	}
}
