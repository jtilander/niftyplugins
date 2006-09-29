using System;
using System.IO;
using System.Diagnostics;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Windows.Forms;

namespace Aurora
{

	namespace NiftyPerforce
	{
		// Direct perforce operations that can be called on the currently active document.
		class Perforce
		{
			private OutputWindowPane m_outputPane;
			private DTE2 m_application;

			public Perforce(DTE2 application, AddIn addIn, OutputWindowPane outputPane)
			{
				m_application = application;
				m_outputPane = outputPane;

				AddCommand(addIn, "P4Edit", "Opens up the current item for edit in perforce", "");
				AddCommand(addIn, "P4EditItem", "Opens up the current item for edit in perforce", "Item");
				AddCommand(addIn, "P4RenameItem", "Renames the current item", "Item");
				AddCommand(addIn, "P4EditProject", "Opens up the selected project for edit in perforce", "Project");
				AddCommand(addIn, "P4EditSolution", "Opens up the current solution for edit in perforce", "Solution");
				AddCommand(addIn, "NiftyPerforceConfig", "Opens up the configuration dialog", "Tools");
			}

			public void OnQueryStatus(ref vsCommandStatus status, string commandName)
			{
				if (!commandName.EndsWith(".P4Edit") &&
					!commandName.EndsWith(".P4EditItem") &&
					!commandName.EndsWith(".P4RenameItem") &&
					!commandName.EndsWith(".P4EditProject") &&
					!commandName.EndsWith(".P4EditSolution") &&
					!commandName.EndsWith(".NiftyPerforceConfig")
					)
					return;

				status = vsCommandStatus.vsCommandStatusEnabled | vsCommandStatus.vsCommandStatusSupported;
			}

			public bool OnExecuteCommand(string commandName)
			{
				bool handled = true;

				//TODO: Add more commands here, like History, Diff against depot etc...
				if (commandName.EndsWith(".P4Edit"))
				{
					OnP4Edit();
				}
				else if (commandName.EndsWith(".P4EditItem"))
				{
					OnItemEdit();
				}
				else if (commandName.EndsWith(".P4RenameItem"))
				{
					OnRenameItem();
				}
				else if (commandName.EndsWith(".P4EditProject"))
				{
					OnProjectEdit();
				}
				else if (commandName.EndsWith(".P4EditSolution"))
				{
					OnSolutionEdit();
				}
				else if (commandName.EndsWith(".NiftyPerforceConfig"))
				{
					OnEditConfig();
				}
				else
				{
					handled = false;
				}

				return handled;
			}

			private void AddCommand(AddIn addIn, string commandName, string description, string menuName)
			{
				object[] contextGuids = new object[] { };
				Commands2 commands = (Commands2)m_application.Commands;
				try
				{
					int commandStatus = (int)vsCommandStatus.vsCommandStatusSupported +
										(int)vsCommandStatus.vsCommandStatusEnabled;

					int commandStyle = (int)vsCommandStyle.vsCommandStyleText;
					vsCommandControlType controlType = vsCommandControlType.vsCommandControlTypeButton;

					Command command = commands.AddNamedCommand2(addIn,
													commandName,
													commandName,
													description,
													true,
													59,
													ref contextGuids,
													commandStatus,
													commandStyle,
													controlType);


					if ("" != menuName)
					{
						command.AddControl(((CommandBars)m_application.CommandBars)[menuName], 1);
					}
				}
				catch (System.ArgumentException)
				{
				}
			}

			public void OnP4Edit()
			{
				string fullName = m_application.ActiveDocument.FullName;
				P4Operations.EditFile(m_outputPane, fullName);
			}

			public void OnItemEdit()
			{
				foreach (SelectedItem sel in m_application.SelectedItems)
				{
					if (null == sel.ProjectItem)
						continue;

					P4Operations.EditFile(m_outputPane, sel.ProjectItem.get_FileNames(0));
				}
			}

			public void OnRenameItem()
			{
				foreach (SelectedItem sel in m_application.SelectedItems)
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
					P4Operations.IntegrateFile(m_outputPane, newName, oldName);
					P4Operations.EditFile(m_outputPane, sel.ProjectItem.ContainingProject.FullName);
					sel.ProjectItem.Collection.AddFromFile(newName);
					sel.ProjectItem.Delete();
					P4Operations.DeleteFile(m_outputPane, oldName);
				}

			}

			public void OnProjectEdit()
			{
				foreach (SelectedItem sel in m_application.SelectedItems)
				{
					if (null == sel.Project)
						continue;

					P4Operations.EditFile(m_outputPane, sel.Project.FullName);
				}
			}

			public void OnSolutionEdit()
			{
				if ("" == m_application.Solution.FullName)
					return;
				P4Operations.EditFile(m_outputPane, m_application.Solution.FullName);
			}

			public void OnEditConfig()
			{
				Singleton<Config>.Instance.ShowDialog();
			}
		}
	}

}