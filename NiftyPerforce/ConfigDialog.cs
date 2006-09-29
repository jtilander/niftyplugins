using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Aurora
{

	namespace NiftyPerforce
	{
		public partial class ConfigDialog : Form
		{
			public ConfigDialog()
			{
				InitializeComponent();
			}

			private void button1_Click(object sender, EventArgs e)
			{
				wasCancelled = false;
				Close();
			}

			private void button2_Click(object sender, EventArgs e)
			{
				wasCancelled = true;
				Close();
			}
		}
	}

}
