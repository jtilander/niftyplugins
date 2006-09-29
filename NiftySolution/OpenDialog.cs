using System;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Aurora
{

	namespace NiftySolution
	{
		public partial class OpenDialog : Form
		{
			public List<string> filesToOpen;

			public OpenDialog(DTE2 application)
			{
				InitializeComponent();
				m_application = application;
				m_solutionFiles = new List<string>();
				filesToOpen = new List<string>();
			}

			private void RefreshProjectItems()
			{
				m_solutionFiles.Clear();
				m_listView.Clear();

				foreach (Project project in m_application.Solution.Projects)
				{
					AddProjectItems(project.ProjectItems);
				}
			}

			private void AddProjectItems(ProjectItems projectItems)
			{
				// HACK: Horrible! But how will we know what not to include in the list?
				// CPP:   {6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}
				// H:     {6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}
				// Folder:{6BB5F8F0-4483-11D3-8BCF-00C04F8EC28C}

				foreach (ProjectItem item in projectItems)
				{
					if ("{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}" == item.Kind)
					{
						for (short i = 0; i < item.FileCount; i++)
						{
							string name = item.get_FileNames((short)i);
							m_solutionFiles.Add(name);

							ListViewItem listItem = new ListViewItem(Path.GetFileName(name));
							listItem.ImageIndex = m_solutionFiles.Count;
							m_solutionFiles.Add(name);
							m_listView.Items.Add(listItem);
						}
					}

					AddProjectItems(item.ProjectItems);
				}
			}

			private void m_listView_KeyPress(object sender, KeyPressEventArgs e)
			{
				if (char.IsControl(e.KeyChar))
					return;

				m_searchString += e.KeyChar;
				UpdateSearch();
			}

			private void m_listView_KeyUp(object sender, KeyEventArgs e)
			{
				switch (e.KeyCode)
				{
					case Keys.Escape:
						m_searchString = "";
						UpdateSearch();
						this.Close();
						return;

					case Keys.Return:
						if (OpenSelectedItems())
						{
							m_searchString = "";
							UpdateSearch();
							this.Close();
						}
						return;

					case Keys.Up:
						m_searchString = "";
						UpdateSearch();
						UpdateSelected();
						return;

					case Keys.Down:
						m_searchString = "";
						UpdateSearch();
						UpdateSelected();
						return;

					case Keys.Right:
						m_searchString = "";
						UpdateSearch();
						UpdateSelected();
						return;

					case Keys.Left:
						m_searchString = "";
						UpdateSearch();
						UpdateSelected();
						return;

					case Keys.Back:
						try
						{
							m_searchString = m_searchString.Remove(m_searchString.Length - 1);
							UpdateSearch();
							UpdateSelected();
						}
						catch (ArgumentOutOfRangeException)
						{
						}
						return;

					case Keys.F5:
						RefreshProjectItems();
						return;
				}
			}

			private bool OpenSelectedItems()
			{
				if (0 == m_listView.SelectedItems.Count)
					return false;

				foreach (ListViewItem item in m_listView.SelectedItems)
				{
					string path = m_solutionFiles[item.ImageIndex];
					filesToOpen.Add(path);
				}

				m_listView.SelectedItems.Clear();
				return true;
			}

			private void UpdateSearch()
			{
				this.Text = "\"" + m_searchString + "\"";

				if (0 == m_searchString.Length)
					return;

				ListViewItem item = m_listView.FindItemWithText(m_searchString);
				if (item == null)
					return;


				item.EnsureVisible();
				item.Selected = true;
				UpdateSelected();
			}

			private void UpdateSelected()
			{
				if (0 == m_listView.SelectedItems.Count)
					return;
				ListViewItem item = m_listView.SelectedItems[0];
				m_toolStripLabel.Text = m_solutionFiles[item.ImageIndex];
			}

			private void m_listView_MouseDoubleClick(object sender, MouseEventArgs e)
			{
				OpenSelectedItems();
				this.Close();
			}

			private void OpenDialog_Activated(object sender, EventArgs e)
			{
				if (0 == m_solutionFiles.Count)
					RefreshProjectItems();
			}

			private DTE2 m_application;
			private List<string> m_solutionFiles;
			private string m_searchString;

		}
	}
}
