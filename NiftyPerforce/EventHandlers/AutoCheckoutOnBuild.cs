// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
using System;
using System.IO;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class AutoCheckoutOnBuild : PreCommandFeature
		{
			public AutoCheckoutOnBuild(Plugin plugin)
				: base(plugin, "AutoCheckoutOnBuild", "Automatically checks out the source when building")
			{
				if(!Singleton<Config>.Instance.autoCheckoutOnBuild)
					return;

				RegisterHandler("Build.BuildSolution", OnCheckoutModifiedSource);
				RegisterHandler("Build.Compile", OnCheckoutModifiedSource);
				RegisterHandler("ClassViewContextMenus.ClassViewProject.Build", OnCheckoutModifiedSource);
				RegisterHandler("Build.BuildOnlyProject", OnCheckoutModifiedSource);
			}

			private void OnCheckoutModifiedSource(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
			{
				foreach(Document doc in mPlugin.App.Documents)
				{
					if(!doc.Saved && doc.ReadOnly)
						P4Operations.EditFileImmediate(mPlugin.OutputPane, doc.FullName);
				}
			}
		}
	}
}
