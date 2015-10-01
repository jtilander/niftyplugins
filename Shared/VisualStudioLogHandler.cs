// Copyright (C) 2006-2010 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using System.Text;

namespace Aurora
{
	public class VisualStudioLogHandler : Log.Handler
	{
        private Plugin mPlugin;

		public VisualStudioLogHandler(Plugin plugin)
		{
            mPlugin = plugin;
		}

		public void OnMessage(Log.Level level, string message, string formattedLine)
		{
            OutputWindowPane pane = mPlugin.OutputPane;
			if (null == pane)
				return;

			pane.OutputString(formattedLine);
		}
	}
}
