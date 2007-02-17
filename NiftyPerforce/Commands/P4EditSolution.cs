using System;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class P4EditSolution : CommandBase
		{
			public override void OnCommand(DTE2 application, OutputWindowPane pane)
			{
                if (application.Solution != null && application.Solution.FullName != string.Empty)
                    P4Operations.EditFile(pane, application.Solution.FullName);
			}

            public override bool IsEnabled(DTE2 application)
            {
                return application.Solution != null && application.Solution.FullName != string.Empty;
            }
		}
	}
}
