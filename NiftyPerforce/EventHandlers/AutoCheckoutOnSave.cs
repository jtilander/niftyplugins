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
		class AutoCheckoutOnSave : PreCommandFeature
		{
			public AutoCheckoutOnSave(Plugin plugin)
				: base(plugin, "AutoCheckoutOnSave", "Automatically checks out files on save")
			{
				if(!Singleton<Config>.Instance.autoCheckoutOnSave)
					return;

				Log.Info("Adding handlers for automatically checking out dirty files when you save");
				RegisterHandler("File.SaveSelectedItems", OnSaveSelected);
				RegisterHandler("File.SaveAll", OnSaveAll);
			}

			private void OnSaveSelected(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
			{
				Config cfg = mPlugin.Options as Config;

				foreach(SelectedItem sel in mPlugin.App.SelectedItems)
				{
					if(sel.Project != null)
						P4Operations.EditFileImmediate(mPlugin.OutputPane, sel.Project.FullName, cfg.ignoreReadOnlyOnEdit);
					else if(sel.ProjectItem != null)
						P4Operations.EditFileImmediate(mPlugin.OutputPane, sel.ProjectItem.Document.FullName, cfg.ignoreReadOnlyOnEdit);
					else
						P4Operations.EditFileImmediate(mPlugin.OutputPane, mPlugin.App.Solution.FullName, cfg.ignoreReadOnlyOnEdit);
				}
			}

			private void OnSaveAll(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
			{
				Config cfg = mPlugin.Options as Config;

				if(!mPlugin.App.Solution.Saved)
					P4Operations.EditFileImmediate(mPlugin.OutputPane, mPlugin.App.Solution.FullName, cfg.ignoreReadOnlyOnEdit);

				foreach(Document doc in mPlugin.App.Documents)
				{
					if(doc.Saved)
						continue;
					P4Operations.EditFileImmediate(mPlugin.OutputPane, doc.FullName, cfg.ignoreReadOnlyOnEdit);
				}

				if(mPlugin.App.Solution.Projects == null)
					return;

				foreach(Project p in mPlugin.App.Solution.Projects)
				{
					EditProjectRecursive(p);
				}
			}

			private void EditProjectRecursive(Project p)
			{
				Config cfg = mPlugin.Options as Config;

				if(!p.Saved)
					P4Operations.EditFileImmediate(mPlugin.OutputPane, p.FullName, cfg.ignoreReadOnlyOnEdit);

				if(p.ProjectItems == null)
					return;

				foreach(ProjectItem pi in p.ProjectItems)
				{
					if(pi.SubProject != null)
					{
						EditProjectRecursive(pi.SubProject);
					}
					else if(!pi.Saved)
					{
						for(short i = 1; i <= pi.FileCount; i++)
							P4Operations.EditFileImmediate(mPlugin.OutputPane, pi.get_FileNames(i), cfg.ignoreReadOnlyOnEdit);
					}
				}
			}
		}
	}
}
