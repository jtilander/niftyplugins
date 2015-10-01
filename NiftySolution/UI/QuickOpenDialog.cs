// Copyright (C) 2006-2010 Jim Tilander. See COPYING for and README for more details.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Aurora
{
	namespace NiftySolution
	{
		public partial class QuickOpenDialog : Form
		{
			SolutionFiles mFiles;
			string mFileToOpen = "";

			public string FileToOpen
			{
				get { return mFileToOpen; }
			}

			public QuickOpenDialog(SolutionFiles files)
			{
				mFiles = files;
				InitializeComponent();

				mSearchResults.SearchSpace = mFiles;
				mStatusLabel.Text = string.Format("{0} hits", mSearchResults.CandidateCount);

				Resize += new EventHandler(searchResultControl1_Resize);
			}

			private void searchResultControl1_Resize(object sender, EventArgs e)
			{

            }

            private void textBox1_KeyDown(object sender, KeyEventArgs e)
			{
				switch(e.KeyCode)
				{
					case Keys.Escape:
						DialogResult = DialogResult.Cancel;
						this.Close();
						break;

					case Keys.Enter:
						mFileToOpen = mSearchResults.Selected.fullPath;
						DialogResult = DialogResult.OK;
						this.Close();
						break;

					case Keys.Down:
						mSearchResults.NextMatch();
						e.Handled = true;
						mSelectionInfo.Text = mSearchResults.Selected.fullPath;
						break;

					case Keys.Up:
						mSearchResults.PrevMatch();
						e.Handled = true;
						mSelectionInfo.Text = mSearchResults.Selected.fullPath;
						break;
					
					case Keys.F5:
						mFiles.Refresh();
						break;
				}
			}

			private void textBox1_TextChanged(object sender, EventArgs e)
			{
				mSearchResults.QueryString = mInputText.Text;
				mStatusLabel.Text = string.Format("{0} hits", mSearchResults.CandidateCount);
			}

			private void QuickOpenDialog_Shown(object sender, EventArgs e)
			{
				if(0 == mInputText.Text.Length)
					return;
				mInputText.SelectAll();
			}
		}
	}
}
