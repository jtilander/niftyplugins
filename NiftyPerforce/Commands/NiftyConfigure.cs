using System;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftyPerforce
	{
        class NiftyConfigure : CommandBase
		{
			public override void OnCommand(DTE2 application, OutputWindowPane pane)
			{
				Singleton<Config>.Instance.ShowDialog();
			}

            public override bool IsEnabled(DTE2 application)
            {
                return true;
            }
		}
	}
}
