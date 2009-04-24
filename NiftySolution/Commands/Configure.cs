// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
using System;
using System.Collections.Generic;
using System.IO;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftySolution
	{
		public class Configure : CommandBase
		{
			override public int IconIndex { get { return 3; } }

			public Configure(Plugin plugin)
				: base("Configure", plugin, "Configures the plugin")
			{
			}

			public override bool OnCommand()
			{
				Log.Debug("Launching the configure tool");

				Options options = (Options)Plugin.Options;
				options.Save();
				
				ConfigDialog dlg = new ConfigDialog();
				dlg.propertyGrid1.SelectedObject = options;

				if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
				{
					Plugin.Options = Options.Load(options.mFileName);
				}
				else
				{
					options.Save();
				}

				return true;
			}

			public override bool IsEnabled()
			{
				return true;
			}
		}
	}
}
