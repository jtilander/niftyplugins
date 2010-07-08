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
				if(!Singleton<Config>.Instance.autoCheckoutProject)
				{
					return;
				}

				Log.Info("Adding handlers for automatically checking out .vcproj files when you do changes to the project");
				RegisterHandler("ClassViewContextMenus.ClassViewProject.Properties", OnCheckoutSelectedProjects);
				RegisterHandler("ClassViewContextMenus.ClassViewMultiselectProjectreferencesItems.Properties", OnCheckoutSelectedProjects);
				RegisterHandler("File.Properties", OnCheckoutSelectedProjects);
				RegisterHandler("View.PropertiesWindow", OnCheckoutSelectedProjects);
				RegisterHandler("Project.Properties", OnCheckoutSelectedProjects);
				RegisterHandler("Project.AddNewItem", OnCheckoutSelectedProjects);
				RegisterHandler("Project.AddExistingItem", OnCheckoutSelectedProjects);

				// hmm : removing a file from Solution Explorer is just Edit.Delete !?
				RegisterHandler("Edit.Delete", OnCheckoutSelectedProjects);
				//RegisterHandler("5EFC7975-14BC-11CF-9B2B-00AA00573819:17", OnCheckoutSelectedProjects);

				RegisterHandler("File.Remove", OnCheckoutSelectedProjects); // I don't think this actually does anything
			}

			private void OnCheckoutSelectedProjects(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
			{
				// when I get Edit.Delete :
				if(Guid == "{5EFC7975-14BC-11CF-9B2B-00AA00573819}" && ID == 17)
				{
					// see if the active window is SolutionExplorer :
					Window w = mPlugin.App.ActiveWindow;
					if(w.Type != EnvDTE.vsWindowType.vsWindowTypeSolutionExplorer)
					{
						// it's just a delete in the text window, get out !
						return;
					}
				}

				foreach(Project project in (Array)mPlugin.App.ActiveSolutionProjects)
				{
					P4Operations.EditFileImmediate(mPlugin.OutputPane, project.FullName);
				}
			}
		}
	}
}
