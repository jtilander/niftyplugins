using System;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class NiftyConfigure
		{
			public void OnCommand(DTE2 application, OutputWindowPane pane)
			{
				Singleton<Config>.Instance.ShowDialog();
			}
		}
	}
}
