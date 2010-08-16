// Copyright (C) 2006-2010 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using EnvDTE80;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualStudio.CommandBars;

namespace Aurora
{
	namespace NiftyPerforce
	{
        class P4RenameItem : ItemCommandBase
		{
			public P4RenameItem(Plugin plugin)
                :  base("RenameItem", plugin, "Renames an item", true, false)
            {
            }

            public override void OnExecute(SelectedItem item, string fileName, OutputWindowPane pane)
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Title = "Source: " + fileName;
                dlg.Multiselect = false;
                dlg.CheckFileExists = false;
                dlg.CheckPathExists = false;
                dlg.InitialDirectory = Path.GetDirectoryName(fileName);
                dlg.FileName = Path.GetFileName(fileName);

                if (DialogResult.OK == dlg.ShowDialog())
                {
                    string newName = dlg.FileName;
                    P4Operations.IntegrateFile(pane, newName, fileName);
					P4Operations.EditFile(pane, item.ProjectItem.ContainingProject.FullName);
                    item.ProjectItem.Collection.AddFromFile(newName);
                    item.ProjectItem.Delete();
                    P4Operations.DeleteFile(pane, fileName);
                }
            }
		}
	}
}
