using System;
using EnvDTE;
using EnvDTE80;
using System.Windows.Forms;
using System.IO;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class P4RenameItem
		{
			public void OnCommand(DTE2 application, OutputWindowPane pane)
			{
				foreach (SelectedItem sel in application.SelectedItems)
				{
					if (null == sel.ProjectItem)
						continue;

					string oldName = sel.ProjectItem.get_FileNames(0);

					OpenFileDialog dlg = new OpenFileDialog();
					dlg.Title = "Source: " + oldName;
					dlg.Multiselect = false;
					dlg.CheckFileExists = false;
					dlg.CheckPathExists = false;
					dlg.InitialDirectory = Path.GetDirectoryName(oldName);
					dlg.FileName = Path.GetFileName(oldName);

					if (DialogResult.OK != dlg.ShowDialog())
						continue;

					string newName = dlg.FileName;
					P4Operations.IntegrateFile(pane, newName, oldName);
					P4Operations.EditFile(pane, sel.ProjectItem.ContainingProject.FullName);
					sel.ProjectItem.Collection.AddFromFile(newName);
					sel.ProjectItem.Delete();
					P4Operations.DeleteFile(pane, oldName);
				}
			}
		}
	}
}
