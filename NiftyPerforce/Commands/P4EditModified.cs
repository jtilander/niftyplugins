// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class P4EditModified : CommandBase
		{
			public P4EditModified(Plugin plugin)
				: base("EditModified", plugin, "Opens all the currently modifed files for edit")
			{
			}

			public override bool OnCommand()
			{
				Log.Info("Got build solution command, now checking {0} documents for modification", Plugin.App.Documents.Count);

				bool ignoreReadOnly = ((Config)(Plugin.Options)).ignoreReadOnlyOnEdit;

				foreach (Document doc in Plugin.App.Documents)
				{
					if(doc.Saved)
						continue;
					if(!ignoreReadOnly && !doc.ReadOnly)
						continue;
					P4Operations.EditFile(Plugin.OutputPane, doc.FullName, ignoreReadOnly);
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
