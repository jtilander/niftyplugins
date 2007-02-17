using System;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class P4EditModified : CommandBase
		{
			public override void OnCommand(DTE2 application, OutputWindowPane pane)
			{
				foreach (Document doc in application.Documents)
				{
					if (!doc.Saved && doc.ReadOnly)
					    P4Operations.EditFile(pane, doc.FullName);
				}
			}

            public override bool IsEnabled(DTE2 application)
            {
                return true;
            }
		}
	}
}
