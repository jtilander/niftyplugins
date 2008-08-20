// Copyright (C) 2006-2008 Jim Tilander. See COPYING for and README for more details.
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
