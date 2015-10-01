// Copyright (C) 2006-2010 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class P4EditModified : CommandBase
		{
			public P4EditModified(Plugin plugin, string canonicalName)
				: base("EditModified", canonicalName, plugin, "Opens all the currently modifed files for edit")
			{
			}

			override public int IconIndex { get { return 5; } }

            public override bool OnCommand()
			{
				Log.Info("P4EditModified : now checking {0} documents for modification", Plugin.App.Documents.Count);

				if(!Plugin.App.Solution.Saved)
				{
					P4Operations.EditFile(Plugin.OutputPane, Plugin.App.Solution.FullName);
				}

				foreach(Project p in Plugin.App.Solution.Projects)
				{
					if(!p.Saved)
					{
						P4Operations.EditFile(Plugin.OutputPane, p.FullName);
					}
				}

				foreach(Document doc in Plugin.App.Documents)
				{
					if(!doc.Saved)
					{
						P4Operations.EditFile(Plugin.OutputPane, doc.FullName);
					}
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
