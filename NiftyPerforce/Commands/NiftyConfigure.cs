// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftyPerforce
	{
        class NiftyConfigure : CommandBase
		{
			public NiftyConfigure(Plugin plugin)
				: base("Configure", plugin, "Opens the configuration dialog")
			{
			}

			override public int IconIndex { get { return 2; } }

			public override bool OnCommand()
			{
				Log.Debug("Launching the configure tool");

				Config options = (Config)Plugin.Options;
				options.Save();

				ConfigDialog dlg = new ConfigDialog();
				dlg.propertyGrid1.SelectedObject = options;

				if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
				{
					Plugin.Options = Config.Load(options.mFileName);
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
