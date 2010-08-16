// Copyright (C) 2006-2010 Jim Tilander. See COPYING for and README for more details.
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
				foreach(SelectedItem sel in mPlugin.App.SelectedItems)
				{
					if(sel.Project != null)
						P4Operations.EditFileImmediate(mPlugin.OutputPane, sel.Project.FullName);
					else if(sel.ProjectItem != null)
						P4Operations.EditFileImmediate(mPlugin.OutputPane, sel.ProjectItem.Document.FullName);
					else
						P4Operations.EditFileImmediate(mPlugin.OutputPane, mPlugin.App.Solution.FullName);
				}
			}

			private void OnSaveAll(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
			{
				if(!mPlugin.App.Solution.Saved)
					P4Operations.EditFileImmediate(mPlugin.OutputPane, mPlugin.App.Solution.FullName);

				foreach(Document doc in mPlugin.App.Documents)
				{
					if(doc.Saved)
						continue;
					P4Operations.EditFileImmediate(mPlugin.OutputPane, doc.FullName);
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
				if(!p.Saved)
					P4Operations.EditFileImmediate(mPlugin.OutputPane, p.FullName);

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
						for(short i = 0; i <= pi.FileCount; i++)
							P4Operations.EditFileImmediate(mPlugin.OutputPane, pi.get_FileNames(i));
					}
				}
			}
		}
	}
}
