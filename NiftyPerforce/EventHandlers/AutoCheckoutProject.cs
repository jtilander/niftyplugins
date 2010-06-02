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
		class AutoCheckoutProject : PreCommandFeature
		{
			public AutoCheckoutProject(Plugin plugin)
				: base(plugin, "AutoCheckoutProject", "Automatically checks out the project files")
			{
				if( !Singleton<Config>.Instance.autoCheckoutProject )
					return;

				Log.Info("Adding handlers for automatically checking out .vcproj files when you do changes to the project");
				RegisterHandler("ClassViewContextMenus.ClassViewProject.Properties", OnCheckoutSelectedProjects);
				RegisterHandler("ClassViewContextMenus.ClassViewMultiselectProjectreferencesItems.Properties", OnCheckoutSelectedProjects);
				RegisterHandler("Project.Properties", OnCheckoutSelectedProjects);
				RegisterHandler("Project.AddNewItem", OnCheckoutSelectedProjects);
				RegisterHandler("Project.AddExistingItem", OnCheckoutSelectedProjects);
			}

			private void OnCheckoutSelectedProjects(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
			{
				bool ignoreReadOnly = ((Config)mPlugin.Options).ignoreReadOnlyOnEdit;

				foreach(Project project in (Array)mPlugin.App.ActiveSolutionProjects)
				{
					P4Operations.EditFileImmediate(mPlugin.OutputPane, project.FullName, ignoreReadOnly);
				}
			}
		}
	}
}
