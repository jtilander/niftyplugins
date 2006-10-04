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
				EditSolution(application.Solution, pane);
			}

			public static bool EditSolution(Solution solution, OutputWindowPane pane)
			{
				if (null == solution || "" == solution.FullName)
					return false;
				return P4Operations.EditFile(pane, solution.FullName);
			}
		}
	}
}
