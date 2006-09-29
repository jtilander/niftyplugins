using System;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class P4EditSolution
		{
			public void OnCommand(DTE2 application, OutputWindowPane pane)
			{
				string solutionName = application.Solution.FullName;
				if ("" == solutionName)
					return;
				P4Operations.EditFile(pane, solutionName);
			}
		}
	}
}
